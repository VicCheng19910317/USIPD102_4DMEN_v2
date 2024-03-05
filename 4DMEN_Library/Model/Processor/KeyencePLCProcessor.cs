using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    public class KeyencePLCProcessor : KeyencePLCNetParam
    {
        #region 狀態屬性
        internal bool IsSend { get; set; }
        internal bool IsError { get; set; }
        internal bool IsIdle { get; set; }
        internal bool IsFinish { get; set; }
        internal bool IsInitialized { get; set; }
        internal bool IsChangedCassette { get; set; }
        /// <summary>
        /// 門檢是否開啟(true:開啟/false:關閉)
        /// </summary>
        private bool IsDoorOpen { get; set; } = false;
        private bool IsDoorStatus { get; set; } = false;
        private bool IsPlating { get; set; } = false;
        private static object lock_enum = new object();
        internal virtual int Status
        {
            set
            {
                IsChangedCassette = IsInitialized = IsSend = IsError = IsIdle = IsFinish = false;
                switch (value)
                {
                    case 1:
                        IsIdle = true;
                        break;
                    case 2:
                        IsSend = true;
                        break;
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                        IsFinish = true;
                        break;
                    case 8:
                        IsChangedCassette = true;
                        break;
                    case 91:
                        IsInitialized = true;
                        break;
                    case 99:
                    default:
                        IsError = true;
                        break;
                }
            }
        }
        internal int TimeDuration { get; set; } = 200;
        #endregion 狀態屬性
        #region 實作方法
        /// <summary>
        /// 取得狀態
        /// </summary>
        internal void GetStatus()
        {
            string send_data = $"RDS DM00{ReadAddress} 1";
            string status = plcNet.ReadData(send_data).Replace("E", "").Replace("\r\n", "");
            int tmp;
            if (int.TryParse(status, out tmp))
                Status = tmp;
            else
                Status = 99;
        }
        /// <summary>
        /// 發送單一指令訊號
        /// </summary>
        /// <param name="step">步驟流程</param>
        /// <returns>執行成功/失敗</returns>
        internal virtual bool SendData(int step, string match_data = "", bool init = false)
        {
            string send_data = $"WRS DM00{WriteAddress} 1 {step}";
            string response_message = plcNet.WriteData(send_data);
            if (!response_message.Contains(match_data))
            {
                Message = $"發送回傳訊號比對錯誤，發送訊號:{send_data}，比對訊號:{match_data}";
                var logger = MainPresenter.LogDatas();
                logger = LoggerData.Error(new Exception("錯誤"), $"發送回傳訊號比對錯誤，發送訊號:{send_data}，比對訊號:{match_data}");
                return false;
            }
            DateTime time = DateTime.Now;
            while ((DateTime.Now - time).TotalMilliseconds < Timeout)
            {
                GetStatus();
                if (IsFinish || IsError)
                    break;
                if (init && IsIdle)
                    break;
                System.Threading.Thread.Sleep(TimeDuration);
            }
            IsError = !IsFinish;
            if (init) IsError = !IsIdle;
            Message = IsError ? "發送訊號錯誤" : "發送訊號成功";
            if (IsError)
            {
                var logger = MainPresenter.LogDatas();
                logger = LoggerData.Error(new Exception("錯誤"), $"發送訊號錯誤，錯誤訊號:{send_data}");
            }

            else
            {
                var logger = MainPresenter.LogDatas();
                logger = LoggerData.Info($"發送訊號成功");
            }

            return init ? IsIdle : IsFinish;
        }
        /// <summary>
        /// 發送通訊關閉訊號(idle)
        /// </summary>
        /// <returns>執行成功/失敗</returns>
        internal virtual bool SendIdle()
        {
            string send_data = $"WRS DM00{WriteAddress} 1 0";
            string response_message = plcNet.WriteData(send_data);
            int time = 0;
            while (time < Timeout)
            {
                GetStatus();
                if (IsFinish || IsError || IsChangedCassette || IsIdle) break;
                time += TimeDuration;
            }
            IsError = !IsIdle;
            Message = IsError ? "發送關閉訊號錯誤" : "發送關閉訊號成功";
            if (IsError)
            {
                var logger = MainPresenter.LogDatas();
                logger = LoggerData.Error(new Exception("錯誤"), $"發送關閉訊號錯誤");
            }
            else
            {
                var logger = MainPresenter.LogDatas();
                logger = LoggerData.Error(new Exception("錯誤"), $"發送關閉訊號成功");
            }
            return IsIdle;
        }
        /// <summary>
        /// 取得門檢狀態狀態
        /// </summary>
        internal bool GetDoorStatus(string task_name)
        {
            try
            {
                if (IsDoorStatus || IsPlating)
                {
                    return IsDoorOpen;
                }
                else
                {
                    IsDoorStatus = true;
                    var is_open = false;
                    var by_pass = 1;
                    string send_get_status = $"RDS R2300 11";
                    is_open = plcNet.ReadData(send_get_status).Replace("E", "").Replace("\r\n", "").Replace(" ", "").Contains("0");
                    string send_get_by_pass = $"RDS MR2009 1";
                    by_pass = int.Parse(plcNet.ReadData(send_get_by_pass).Replace("E", "").Replace("\r\n", "").Replace(" ", ""));
                    IsDoorOpen = is_open && by_pass == 0;
                    IsDoorStatus = false;
                    Console.WriteLine($"DoorCheck:{task_name}\n");
                    return IsDoorOpen;
                }

            }
            catch (Exception ex)
            {
                var logger = MainPresenter.LogDatas();
                logger = LoggerData.Error(ex, $"發送門檢檢查錯誤");
                IsDoorStatus = false;
                return false;
            }

        }
        /// <summary>
        /// 雷射是否出光
        /// </summary>
        /// <returns>是/否</returns>
        internal bool GetLaserReadyStatus()
        {
            string send_get_status = $"RDS MR4001 1";
            return plcNet.ReadData(send_get_status).Replace("E", "").Replace("\r\n", "").Replace(" ", "").Contains("1"); // 0:未準備/ 1:已準備
        }
        /// <summary>
        /// 雷射準備狀態
        /// </summary>
        /// <returns>是/否</returns>
        internal bool GetLaserOnStatus()
        {
            string send_get_status = $"RDS MR4000 1";
            var row_data = plcNet.ReadData(send_get_status);
            return row_data.Replace("E", "").Replace("\r\n", "").Replace(" ", "").Contains("0"); // 0:出光/ 1:未出光
        }
        /// <summary>
        /// 雷射檔案準備狀態
        /// </summary>
        /// <returns>是/否</returns>
        internal bool GetLaserFileStatus()
        {
            string send_get_status = $"RDS MR4002 1";
            return plcNet.ReadData(send_get_status).Replace("E", "").Replace("\r\n", "").Replace(" ", "").Contains("1"); // 0:未準備/ 1:已準備
        }
        /// <summary>
        /// 設定PC錯誤
        /// </summary>
        /// <param name="value">1:開啟錯誤, 0:關閉錯誤</param>
        /// <returns>成功/失敗</returns>
        public bool PCErrorSet(int value)
        {
            string send_data = $"WRS DM00270 1 {value}";
            string status = plcNet.WriteData(send_data).Replace("E", "").Replace("\r\n", "");
            var logger = MainPresenter.LogDatas();
            if (!status.Contains("OK"))
            {
                Message = $"發送回傳訊號比對錯誤，發送訊號:{send_data}，比對訊號:OK";

                logger = LoggerData.Error(new Exception("錯誤"), $"發送回傳訊號比對錯誤，發送訊號:{send_data}，比對訊號:OK");
                return false;
            }
            if (value == 0)
            {
                ResetPLCErrorList(1);
                System.Threading.Thread.Sleep(200);
                ResetPLCErrorList(0);
            }
            return true;
        }
        /// <summary>
        /// 重置PLC錯誤列表
        /// </summary>
        /// <param name="value">1:開啟/0:關閉</param>
        /// <returns>成功/失敗</returns>
        public bool ResetPLCErrorList(int value)
        {
            string send_data = $"WRS MR2215 1 {value}";
            string status = plcNet.WriteData(send_data).Replace("E", "").Replace("\r\n", "");
            var logger = MainPresenter.LogDatas();
            if (!status.Contains("OK"))
            {
                Message = $"發送回傳訊號比對錯誤，發送訊號:{send_data}，比對訊號:OK";

                logger = LoggerData.Error(new Exception("錯誤"), $"發送回傳訊號比對錯誤，發送訊號:{send_data}，比對訊號:OK");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 設定自動模式
        /// </summary>
        /// <param name="value">1:開啟/0:關閉</param>
        /// <returns>成功/失敗</returns>
        public bool SwitchAutoMode(int value)
        {
            string send_data = $"WRS MR204 1 {value}";
            string status = plcNet.WriteData(send_data).Replace("E", "").Replace("\r\n", "");
            var logger = MainPresenter.LogDatas();
            if (!status.Contains("OK"))
            {
                Message = $"發送回傳訊號比對錯誤，發送訊號:{send_data}，比對訊號:OK";

                logger = LoggerData.Error(new Exception("錯誤"), $"發送回傳訊號比對錯誤，發送訊號:{send_data}，比對訊號:OK");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 下壓動點
        /// </summary>
        /// <returns>成功/失敗</returns>
        public bool PlateDown()
        {
            do
            {
                System.Threading.Thread.Sleep(100);
            }
            while (IsDoorStatus);
            IsPlating = true;
            var tmp2 = SetPlateOrg(0);
            var tmp = SetPlateMove(1);
            IsPlating = false;
            return (tmp && tmp2);
        }
        /// <summary>
        /// 下壓原點
        /// </summary>
        /// <returns>成功/失敗</returns>
        public bool PlateUp()
        {
            do
            {
                System.Threading.Thread.Sleep(100);
            }
            while (IsDoorStatus);
            IsPlating = true;
            var tmp = SetPlateMove(0);
            var tmp2 = SetPlateOrg(1);
            IsPlating = false;
            return (tmp && tmp2);
        }
        /// <summary>
        /// 設定下壓動點點位
        /// </summary>
        /// <param name="value">1:開啟/0:關閉</param>
        /// <returns>成功/失敗</returns>
        public bool SetPlateMove(int value)
        {
            string send_data = $"WRS MR49007 1 {value}";
            string status = plcNet.WriteData(send_data).Replace("E", "").Replace("\r\n", "");
            var logger = MainPresenter.LogDatas();
            if (!status.Contains(""))
            {
                Message = $"發送回傳訊號比對錯誤，發送訊號:{send_data}，比對訊號:OK";

                logger = LoggerData.Error(new Exception("錯誤"), $"發送回傳訊號比對錯誤，發送訊號:{send_data}，比對訊號:");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 設定下壓原點點位
        /// </summary>
        /// <param name="value">1:開啟/0:關閉</param>
        /// <returns>成功/失敗</returns>
        public bool SetPlateOrg(int value)
        {
            string send_data = $"WRS MR49008 1 {value}";
            string status = plcNet.WriteData(send_data).Replace("E", "").Replace("\r\n", "");
            var logger = MainPresenter.LogDatas();
            if (!status.Contains(""))
            {
                Message = $"發送回傳訊號比對錯誤，發送訊號:{send_data}，比對訊號:OK";

                logger = LoggerData.Error(new Exception("錯誤"), $"發送回傳訊號比對錯誤，發送訊號:{send_data}，比對訊號:");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 取得動點訊號
        /// </summary>
        /// <returns>1:達到動點/0:未達到動點</returns>
        public int GetPlateMove()
        {
            do
            {
                System.Threading.Thread.Sleep(100);
            }
            while (IsDoorStatus);
            IsPlating = true;
            string send_data = $"RDS MR49212 1";
            var result = int.Parse(plcNet.ReadData(send_data).Replace("E", "").Replace("\r\n", ""));
            IsPlating = false;
            return result;
        }
        /// <summary>
        /// 取得原點訊號
        /// </summary>
        /// <returns>1:達到原點/0:未達到原點</returns>
        public int GetPlateOrg()
        {
            do
            {
                System.Threading.Thread.Sleep(100);
            }
            while (IsDoorStatus);
            IsPlating = true;
            string send_data = $"RDS MR49213 1";
            var result = int.Parse(plcNet.ReadData(send_data).Replace("E", "").Replace("\r\n", ""));
            IsPlating = false;
            return result;
        }
        #endregion 實作方法
        #region 虛擬方法
        /// <summary>
        /// 製作單動流程
        /// </summary>
        /// <param name="step">流程步驟(預設1)</param>
        /// <returns>執行成功/失敗</returns>
        internal virtual bool RunOneStepFlow(int step = 1, bool init = false)
        {
            bool result = false;
            GetStatus();
            if (IsIdle)
            {
                if (SendData(step, "", init))
                {
                    result = SendIdle();
                    return true;
                }
                else
                {
                    SendIdle();
                    Message = "PLC訊號傳送未收到回應，請確認。";
                    var logger = MainPresenter.LogDatas();
                    logger = LoggerData.Error(new Exception("錯誤"), "PLC訊號傳送未收到回應，請確認");
                }

            }
            else
            {
                Message = "未收到PLC待機狀態，無法發送訊號。";
                var logger = MainPresenter.LogDatas();
                logger = LoggerData.Error(new Exception("錯誤"), "未收到PLC待機狀態，無法發送訊號");
            }
            return result;
        }
        #endregion 虛擬方法
        #region 單動方法
        /// <summary>
        /// 移動到下一站
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool RunToNextStep()
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info("PLC移動到下一站");
            var result = RunOneStepFlow();
            logger = LoggerData.Info("PLC移動到下一站完成");
            return result;
        }
        /// <summary>
        /// 執行全機初始化
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool RunAllInitialized()
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info("PLC執行全機初始化");
            var _timeout = Timeout;
            Timeout = 300000;
            var result = RunOneStepFlow(90, true);
            var tick = DateTime.Now;
            var initialize_finish = false;
            while ((DateTime.Now - tick).TotalSeconds < 300)
            {
                string send_data = $"RDS MR202 1";
                string status = plcNet.WriteData(send_data).Replace("E", "").Replace("\r\n", "");
                if (status == "1")
                {
                    initialize_finish = true;
                    break;
                }
            }
            result = initialize_finish;
            if (result)
                SwitchAutoMode(1);
            Timeout = _timeout;
            logger = LoggerData.Info("PLC執行全機初始化完成");
            return result;
        }
        /// <summary>
        /// 執行單站初始化
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool RunStationInitialized()
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info("PLC執行單站初始化");
            var _timeout = Timeout;
            Timeout = 180000;
            var result = RunOneStepFlow(91, true);
            Timeout = _timeout;
            logger = LoggerData.Info("PLC執行單站初始化完成");
            return result;
        }
        /// <summary>
        /// 放置螺帽
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool RunPutNut()
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info("PLC放置螺帽");
            var _timeout = Timeout;
            Timeout = 30000;
            var result = RunOneStepFlow();
            Timeout = _timeout;
            logger = LoggerData.Info("PLC放置螺帽完成");
            return result;
        }
        /// <summary>
        /// 執行折彎
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool RunBending()
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info("PLC執行折彎");
            var _timeout = Timeout;
            Timeout = 30000;
            var result = RunOneStepFlow();
            Timeout = _timeout;
            logger = LoggerData.Info("PLC執行折彎完成");
            return result;
        }
        /// <summary>
        /// 執行壓平
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool RunPlate()
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info("PLC執行壓平");
            var _timeout = Timeout;
            Timeout = 30000;
            var result = RunOneStepFlow();
            Timeout = _timeout;
            logger = LoggerData.Info("PLC執行壓平完成");
            return result;
        }
        /// <summary>
        /// 夾到掃描QR Code區域
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool RunToScanCode()
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info("PLC夾到掃描QR Code區域");
            var result = RunOneStepFlow();
            logger = LoggerData.Info("PLC夾到掃描QR Code區域完成");
            return result;
        }
        /// <summary>
        /// 夾到秤重機上
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool RunToWeight()
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info("PLC夾到秤重機上");
            var result = RunOneStepFlow(2);
            logger = LoggerData.Info("PLC夾到秤重機上完成");
            return result;
        }
        /// <summary>
        /// 從秤重機夾回台車上
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool RunBackToStation()
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info("PLC從秤重機夾回台車上");
            var result = RunOneStepFlow(3);
            logger = LoggerData.Info("PLC從秤重機夾回台車上完成");
            return result;
        }
        /// <summary>
        /// NG出料
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool RunNGOut()
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info("PLC NG出料");
            var result = RunOneStepFlow();
            logger = LoggerData.Info("PLC NG出料j完成");
            return result;
        }
        /// <summary>
        /// 執行翻料
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool RunToFlip()
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info("PLC 執行翻料");
            var result = RunOneStepFlow();
            logger = LoggerData.Info("PLC 執行翻料完成");
            return result;
        }
        #endregion 單動方法
    }
}
