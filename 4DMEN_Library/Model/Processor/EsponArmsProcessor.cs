using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    /// <summary>
    /// 手臂處理器，需先將手臂登入後才可輸入動作
    /// </summary>
    internal class EsponArmsProcessor : EsponArmsParam
    {
        #region 參數
        /// <summary>
        /// 回傳參數
        /// </summary>
        internal List<dynamic> return_param = new List<dynamic>();

        #endregion 參數
        #region 類別欄位

        #endregion 類別欄位
        #region 類別方法


        #endregion 類別方法
        #region 建構子

        #endregion
        #region 動作方法
        /// <summary>
        /// 資料轉換使用
        /// </summary>
        /// <param name="param">ArmsParam參數</param>
        internal void SetArmsParam(EsponArmsParam param)
        {
            Name = param.Name;
            IP = param.IP;
            Port = param.Port;
            Timeout = param.Timeout;
            Action_Name = param.Action_Name;
        }
        /// <summary>
        /// 手臂取料
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool Pick()
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info($"手臂 {Name} 手臂取料");
            var _timeout = Timeout;
            _timeout = 10000;
            var success = SendAction($"PICK");
            logger = LoggerData.Info($"手臂 {Name} 手臂取料完成");
            timeout = Timeout;
            return success;
        }
        internal virtual bool SetRecipe(int recipe)
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info($"手臂 {Name} 設定產品編號{recipe}");
            var _timeout = Timeout;
            timeout = 20000;
            var success = SendAction($"PROD;{recipe}");
            logger = LoggerData.Info($"手臂 {Name} 設定產品編號{recipe}完成");
            timeout = Timeout;
            return success;
        }
        /// <summary>
        /// 手臂取料置掃碼
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool Read()
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info($"手臂 {Name} 手臂取料置掃碼");
            var _timeout = Timeout;
            timeout = 20000;
            var success = SendAction($"READ");
            logger = LoggerData.Info($"手臂 {Name} 手臂取料置掃碼完成");
            timeout = Timeout;
            return success;
        }
        /// <summary>
        /// 手臂放料
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool Put()
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info($"手臂 {Name} 手臂放料");
            var success = SendAction($"PUTP");
            logger = LoggerData.Info($"手臂 {Name} 手臂放料完成");
            return success;
        }
        /// <summary>
        /// 手臂回原點
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool Home()
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info($"手臂 {Name} 手臂回原點");
            var success = SendAction($"HOME");
            logger = LoggerData.Info($"手臂 {Name} 手臂回原點完成");
            return success;
        }
        /// <summary>
        /// 設定取/放料位置
        /// </summary>
        /// <param name="index">指定放料位置</param>
        /// <returns>成功/失敗</returns>
        internal virtual bool SetPickPos(int index)
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info($"手臂 {Name} 設定取/放料位置");
            var result = SendAction($"SYPU;{index}");
            logger = LoggerData.Info($"手臂 {Name} 設定取/放料位置完成");
            return result;
        }
        /// <summary>
        /// 暫停
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool Pause()
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info($"手臂 {Name} 暫停");
            var success = SendAction("Pause");
            logger = LoggerData.Info($"手臂 {Name} 暫停完成");
            return success;
        }
        /// <summary>
        /// 暫停恢復
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool Resume()
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info($"手臂 {Name} 暫停恢復");
            var success = SendAction($"Resume");
            logger = LoggerData.Info($"手臂 {Name} 暫停恢復");
            return success;
        }
        /// <summary>
        /// 自動塗膠(塗膠機用)
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool Glue()
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info($"手臂 {Name} 自動塗膠");
            var _timeout = Timeout;
            Timeout = 20000;
            var success = SendAction($"GLUE");
            logger = LoggerData.Info($"手臂 {Name} 自動塗膠完成");
            Timeout = _timeout;
            return success;
        }
        /// <summary>
        /// 吐膠(塗膠機用)
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool Spit()
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info($"手臂 {Name} 吐膠");
            var success = SendAction($"SPIT");
            logger = LoggerData.Info($"手臂 {Name} 吐膠完成");
            return success;
        }

        /// <summary>
        /// 吐膠時間設定(塗膠機用)
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool SpitTime(double time)
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info($"手臂 {Name} 吐膠時間設定");
            var success = SendAction($"SET_PURGE_GLUE_TIME;{time}");
            logger = LoggerData.Info($"手臂 {Name} 吐膠時間設定完成");
            return success;
        }
        /// <summary>
        /// 手動塗膠(塗膠機用)
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool ManualGlue(int recipe)
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info($"手臂 {Name} 手動塗膠");
            var _timeout = Timeout;
            Timeout = 180000;
            var success = SendAction($"MANUALGLUE;{recipe}");
            logger = LoggerData.Info($"手臂 {Name} 手動塗膠完成");
            Timeout = _timeout;
            return success;
        }
        /// <summary>
        /// 設定組裝精度(出料手臂用)
        /// </summary>
        /// <param name="accuracy">精度資料</param>
        /// <returns>成功/失敗</returns>
        internal virtual bool SetPlateAccuracy(PlateAccuracy accuracy)
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info($"設定手臂 {Name} 組裝精度(出料手臂用)");
            var success = SendAction($"CCDOFFSET;{accuracy.X.Lower};{accuracy.X.Upper};{accuracy.Y.Lower};{accuracy.Y.Upper};{accuracy.U.Lower};{accuracy.U.Upper}");
            logger = LoggerData.Info($"設定手臂 {Name} 組裝精度(出料手臂用)完成");
            return success;
        }
        /// <summary>
        /// 設定手臂偏移值(取料x,y,z,u,放料x,y,z,u)
        /// </summary>
        /// <param name="shift">偏移值</param>
        /// <returns>成功/失敗</returns>
        internal virtual bool SetShiftValue(ShiftArms shift)
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info($"手臂 {Name} 設定手臂偏移值");
            var success = SendAction($"SetShift;{shift.Pick.X};{shift.Pick.Y};{shift.Pick.Z};{shift.Pick.U};{shift.Put.X};{shift.Put.Y};{shift.Put.Z};{shift.Put.U}");
            logger = LoggerData.Info($"手臂 {Name} 設定手臂偏移值完成");
            return success;
        }
        /// <summary>
        /// 取得手臂偏移值
        /// </summary>
        /// <returns>偏移值</param>
        internal virtual ShiftArms GetShiftValue()
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info($"手臂 {Name} 取得手臂偏移值");
            ShiftArms shift = new ShiftArms();
            if (SendAction($"GetShift"))
            {
                shift = new ShiftArms
                {
                    Pick = new ShiftArmsAxis { X = return_param[2], Y = return_param[3], Z = return_param[4], U = return_param[5] },
                    Put = new ShiftArmsAxis { X = return_param[6], Y = return_param[7], Z = return_param[8], U = return_param[9] }
                };
            }
            logger = LoggerData.Info($"手臂 {Name} 取得手臂偏移值完成");
            return shift;
        }
        internal virtual NeedleAlignAxis NeedleTeach()
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info($"手臂 {Name} 校針");
            var org_timeout = Timeout;
            NeedleAlignAxis result = new NeedleAlignAxis();
            Timeout = 600000;
            if (SendAction("NEED"))
            {
                result = new NeedleAlignAxis
                {
                    X = double.Parse(return_param[2]),
                    Y = double.Parse(return_param[3]),
                    Z = double.Parse(return_param[4])
                };
            }
            Timeout = org_timeout;
            logger = LoggerData.Info($"手臂 {Name} 校針完成");
            return result;
        }
        /// <summary>
        /// 出料手臂移動到第一個掃描位置
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool SetCOne()
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info($"手臂 {Name} 移動到第一個掃描位置");
            var result = SendAction($"CONE");
            logger = LoggerData.Info($"手臂 {Name} 移動到第一個掃描位置完成");
            return result;
        }
        /// <summary>
        /// 出料手臂移動到第二個掃描位置
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool SetCTwo()
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info($"手臂 {Name} 移動到第二個掃描位置");
            var result = SendAction($"CTWO");
            logger = LoggerData.Info($"手臂 {Name} 移動到第二個掃描位置完成");
            return result;
        }
        /// <summary>
        /// 自定義手臂動作
        /// </summary>
        /// <param name="action">輸入動作</param>
        /// <returns>成功/失敗</returns>
        internal virtual bool SendAction(string action)
        {
            var result = false;
            string message = SendMessage($"{action}");
            if (message.ToUpper().Contains("RUNFINISH"))
            {
                var res_data = message.Replace("RUNFINISH;", "");
                if (res_data.Split(';').Length > 1)
                {
                    return_param.Clear();
                    return_param.AddRange(res_data.Split(';').ToList());
                }
                Message = $"發送訊號成功，發送內容:{action}，回傳內容:{message}";
                var logger = MainPresenter.LogDatas();
                logger = LoggerData.Info($"手臂 {Name} 發送訊號成功，發送內容:{action}，回傳內容:{message}");
                result = true;
            }
            else
            {
                Message = $"手臂 {Name} 發送比對資訊失敗，發送內容:{action}，回傳內容:{message}，比對資料:RUNFINISH";
                var logger = MainPresenter.LogDatas();
                logger = LoggerData.Error(new Exception("錯誤"), $"手臂 {Name} 發送比對資訊失敗，發送內容:{action}，回傳內容:{message}，比對資料:RUNFINISH");
            }
            return result;
        }
        /// <summary>
        /// 自定義手臂比對動作
        /// </summary>
        /// <param name="action">輸入動作</param>
        /// <returns>成功/失敗</returns>
        internal virtual bool SendAction(string action, string match_data)
        {
            var result = false;
            string message = SendMessage($"{action}");
            if (message.ToUpper().Contains(match_data))
            {
                var res_data = message.Replace(match_data, "");
                if (res_data.Split(';').Length > 1)
                {
                    return_param.Clear();
                    return_param.AddRange(res_data.Split(';').ToList());
                }
                Message = $"手臂 {Name} 發送訊號成功，發送內容:{action}，回傳內容:{message}";
                var logger = MainPresenter.LogDatas();
                logger = LoggerData.Info($"手臂 {Name} 發送訊號成功，發送內容:{action}，回傳內容:{message}");
                result = true;
            }
            else
            {
                Message = $"手臂 {Name} 發送比對資訊失敗，發送內容:{action}，比對資料:{match_data}";
                var logger = MainPresenter.LogDatas();
                logger = LoggerData.Error(new Exception("錯誤"), $"手臂 {Name} 發送比對資訊失敗，發送內容:{action}，比對資料:{match_data}");
            }
            return result;
        }
        /// <summary>
        /// 自定義手臂動作
        /// </summary>
        /// <param name="action">輸入動作</param>
        /// <param name="param">輸入動作參數</param>
        /// /// <returns>成功/失敗</returns>
        internal virtual bool SendAction(string action, List<string> param)
        {
            var result = false;
            var send_param = param.Aggregate("", (total, next) => total += $";{next}");
            string message = SendMessage($"{action}{send_param}");
            if (message.ToUpper().Contains("RUNFINISH"))
            {
                var res_data = message.Replace("RUNFINISH;", "");
                if (res_data.Split(';').Length > 1)
                {
                    return_param.Clear();
                    return_param.AddRange(res_data.Split(';').ToList());
                }
                result = true;
            }
            return result;
        }
        /// <summary>
        /// 自定義手臂動作
        /// </summary>
        /// <param name="action">輸入動作</param>
        /// <param name="param">輸入動作參數</param>
        /// /// <returns>成功/失敗</returns>
        internal virtual bool SendAction(string action, List<string> param, string match_data)
        {
            var result = false;
            var send_param = param.Aggregate("", (total, next) => total += $";{next}");
            string message = SendMessage($"{action}{send_param}");
            if (message.ToUpper().Contains(match_data))
            {
                var res_data = message.Replace(match_data, "");
                if (res_data.Split(';').Length > 1)
                {
                    return_param.Clear();
                    return_param.AddRange(res_data.Split(';').ToList());
                }
                result = true;
            }
            return result;
        }

        internal override bool ConnectedClose()
        {
            try
            {
                if (tcpClient != null && tcpClient.Client != null && tcpClient.Connected)
                {
                    tcpClient.Close();
                    System.Threading.Thread.Sleep(250);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// 依動作執行單動流程
        /// </summary>
        /// <param name="action">動作(system_func需要三個參數, sypu需要兩個參數)</param>
        /// <returns>成功/失敗</returns>
        internal bool SendActionFunction(string action)
        {
            bool success = false;
            var tmp = action.Split(';')[0].ToLower();
            var tmp_param = action.Split(';');
            List<string> param = new List<string>();
            for (int i = 1; i < tmp_param.Length; i++)
                param.Add(tmp_param[i]);
            switch (tmp)
            {
                case "test":
                    success = TestConnection();
                    break;
                case "read":
                    success = Read();
                    break;
                case "ctrl":
                    success = SystemLogin();
                    break;
                case "pick":
                    success = Pick();
                    break;
                case "prod":
                    success = SetRecipe(int.Parse(action.Split(';')[1]));
                    break;
                case "put":
                case "putp":
                    success = Put();
                    break;
                case "home":
                    success = Home();
                    break;
                case "sypu":
                    if (action.Split(';').Length < 2)
                    {
                        success = false;
                        Message = "輸入動作參數錯誤，請確認";
                        var logger = MainPresenter.LogDatas();
                        logger = LoggerData.Error(new Exception("錯誤"), $"輸入動作參數錯誤，請確認");
                        break;
                    }
                    success = SetPickPos(int.Parse(action.Split(';')[1]));
                    break;
                case "pause":
                    success = Pause();
                    break;
                case "resume":
                    success = Resume();
                    break;
                case "glue":
                    success = Glue();
                    break;
                case "spit":
                    success = Spit();
                    break;
                case "manual_glue":
                    success = ManualGlue(int.Parse(tmp_param[0]));
                    break;
                case "login":
                    success = SystemLogin();
                    break;
                case "logout":
                    success = SystemLogout();
                    break;
                case "system_func":
                    if (action.Split(';').Length < 3)
                    {
                        success = false;
                        Message = "輸入動作參數錯誤，請確認";
                        var logger = MainPresenter.LogDatas();
                        logger = LoggerData.Error(new Exception("錯誤"), $"輸入動作參數錯誤，請確認");
                        break;
                    }
                    success = SystemLoginParamCheck(param[1], param[2]);
                    break;
                case "reset":
                    success = SystemLoginParamCheck("$Reset", "#Reset,0");
                    break;
                case "cone":
                    success = SetCOne();
                    break;
                case "ctwo":
                    success = SetCTwo();
                    break;
                default:
                    success = SendAction(action);
                    break;
            }
            return success;
        }
        #endregion 動作方法
        #region 覆寫方法
        protected override string SendMessage(string message)
        {
            try
            {
                string resMessage = "";
                if (!IsConnected)
                {
                    resMessage = $"手臂 {Name}, {IP} 未連線";
                }
                else if (IsLogin)
                {
                    NetworkStream networkStream = tcpClient.GetStream();
                    string send_command = SendCRLF ? "\r\n" : "";
                    byte[] data = Encoding.ASCII.GetBytes($"{message}{send_command}");
                    if (networkStream.WriteAsync(data, 0, data.Length).Wait(timeout))
                    {
                        try
                        {
                            System.Threading.Thread.Sleep(200);
                            byte[] buffer = new byte[1024];
                            int bytesRead = buffer.Length;
                            if (networkStream.ReadAsync(buffer, 0, buffer.Length).Wait(timeout))
                                resMessage = Encoding.ASCII.GetString(buffer).Replace("\0", "").Replace("\r\n", "");
                            else
                                resMessage = $"{IP} 讀取回傳訊號超時";
                        }
                        catch (Exception ex)
                        {
                            ConnectedClose();
                            StartConnected();
                            throw new Exception($"讀取錯誤：{ex.Message}");
                        }

                    }
                    else
                    {
                        resMessage = $"{IP} 發送訊號超時";
                        ConnectedClose();
                        StartConnected();
                    }
                }
                else
                {
                    resMessage = $"{IP}未登入";
                }
                return resMessage;
            }
            catch (Exception ex)
            {
                ConnectedClose();
                StartConnected();
                Message = $"發送手臂訊號錯誤，錯誤訊號:{ex.Message}";
                return Message;
            }
        }
        #endregion 覆寫方法
        #region 登入方法
        /// <summary>
        /// 系統登入
        /// </summary>
        /// <returns></returns>
        protected bool SystemLoginConnection()
        {
            IsLoginConnection = false;
            LoginClient = new System.Net.Sockets.TcpClient();
            LoginClient.ReceiveTimeout = Timeout;
            LoginClient.SendTimeout = Timeout;
            if (!LoginClient.ConnectAsync(ip, LoginPort).Wait(2000))
            {
                message = $"登入連線失敗";
                return false;
            }
            System.Threading.Thread.Sleep(200);
            IsLoginConnection = true;
            message = $"登入連線成功";
            return true;
        }
        /// <summary>
        /// 登入參數確認
        /// </summary>
        /// <param name="send_data">登入參數</param>
        /// <param name="receive_match_data">登入完成確認參數</param>
        /// <returns>成功/失敗</returns>
        protected bool SystemLoginParamCheck(string send_data, string receive_match_data)
        {
            #region 傳送/接收連線用參數
            NetworkStream networkStream = LoginClient.GetStream();
            string send_command = SendCRLF ? "\r\n" : "";
            string res_message = "";
            var logger = MainPresenter.LogDatas();
            #endregion 傳送/接收連線用參數
            var data = Encoding.ASCII.GetBytes($"{send_data}{send_command}");
            if (networkStream.WriteAsync(data, 0, data.Length).Wait(timeout))
            {
                byte[] buffer = new byte[1024];
                int bytesRead = buffer.Length;
                System.Threading.Thread.Sleep(200);

                if (networkStream.ReadAsync(buffer, 0, buffer.Length).Wait(3000))
                {
                    res_message = Encoding.ASCII.GetString(buffer).Replace("\r\n", "").Replace("\0", "");
                    if (res_message != receive_match_data)
                    {
                        message = $"比對登入參數失敗，參數訊號：{receive_match_data}";
                        logger = LoggerData.Error(new Exception("錯誤"), $"手臂 {Name} 比對登入參數失敗，參數訊號：{receive_match_data}");
                        return false;
                    }
                    else
                        message = res_message;
                }
                else
                {
                    message = $"接收登入回傳訊息超時，請確認";
                    logger = LoggerData.Error(new Exception("錯誤"), $"手臂 {Name} 接收登入回傳訊息超時，請確認");
                    return false;
                }
            }
            else
            {
                message = $"傳送登入訊息超時，請確認";
                logger = LoggerData.Error(new Exception("錯誤"), $"手臂 {Name} 傳送登入訊息超時，請確認");
                return false;
            }
            message = $"遠端傳送登入完成，登入參數:{send_data}";
            logger = LoggerData.Info($"手臂 {Name} 遠端傳送登入完成，登入參數:{send_data}");
            return true;
        }
        /// <summary>
        /// 遠端登出
        /// </summary>
        /// <returns>成功/失敗</returns>
        protected bool SystemLogout()
        {
            try
            {
                LoginClient.Close();
                System.Threading.Thread.Sleep(1500); //這個也很久，要確認
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 遠端登入
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal bool SystemLogin()
        {
            try
            {
                IsConnected = false;
                IsLogin = false;
                if (!IsLoginConnection) //確認登入連線
                {
                    if (SystemLoginConnection())
                    {
                        ConnectedClose();
                        System.Threading.Thread.Sleep(200);
                        // 輸入登入訊號
                        var success = SystemLoginParamCheck("$Login,", "#Login,0");
                        message = (success) ? message : $"輸入登入訊號錯誤，錯誤內容:{message}";
                        // 輸入重置訊號
                        success = SystemLoginParamCheck("$Reset", "#Reset,0");
                        message = (success) ? message : $"輸入重置訊號錯誤，錯誤內容:{message}";
                        // 輸入開始訊號
                        System.Threading.Thread.Sleep(200);
                        success = SystemLoginParamCheck("$Start,0", "#Start,0");
                        message = (success) ? message : $"輸入開始訊號錯誤，錯誤內容:{message}";
                        // 登入控制
                        System.Threading.Thread.Sleep(500);
                        success = StartConnected();
                        message = (success) ? message : $"登入控制錯誤，錯誤內容:{message}";
                        // 設定產品編號
                        success = SetRecipe(MainPresenter.SystemParam().Recipe);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    var success = SystemLogout();
                    message = (success) ? message : $"遠端登出錯誤，錯誤內容:{message}";
                    IsLoginConnection = false;
                    SystemLogin();
                }
                IsLogin = true;
                return true;
            }
            catch (Exception ex)
            {
                var logger = MainPresenter.LogDatas();
                logger = LoggerData.Error(ex, $"遠端登入錯誤");
                return false;
            }
        }
        #endregion 登入方法
    }
}
