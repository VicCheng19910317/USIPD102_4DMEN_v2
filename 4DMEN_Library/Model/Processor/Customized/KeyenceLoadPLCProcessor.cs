using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    internal class KeyenceLoadPLCProcessor : KeyencePLCProcessor
    {
        internal bool IsStop { get; set; }
        internal int SwitchCaseTime { get; set; } = 120;
        #region 虛擬屬性
        internal override int Status
        {
            set
            {
                IsChangedCassette = IsStop = IsSend = IsError = IsIdle = IsFinish = false;
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
                        IsFinish = true;
                        break;
                    case 5:
                        IsStop = true;
                        break;
                    case 8:
                    case 0:
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
        #endregion 虛擬屬性
        #region 覆寫功能
        /// <summary>
        /// 覆寫傳送資料功能(不處理回傳)
        /// </summary>
        /// <param name="step">執行步驟</param>
        /// <returns>成功</returns>
        internal override bool SendData(int step, string match_data = "", bool init = false)
        {
            string send_data = $"WRS DM00{WriteAddress} 1 {step}";
            string response_message = plcNet.WriteData(send_data);
            var logger = MainPresenter.LogDatas();
            if (!response_message.Contains(match_data))
            {
                Message = $"發送回傳訊號比對錯誤，發送訊號:{send_data}，比對訊號:{match_data}";
                logger = LoggerData.Error(new Exception("錯誤"), $"發送回傳訊號比對錯誤，發送訊號:{send_data}，比對訊號:{match_data}");
                return false;
            }
            Message = $"發送訊號成功";

            logger = LoggerData.Info($"發送訊號成功");
            return true;
        }
        /// <summary>
        /// 執行單動功能(不處理回傳)
        /// </summary>
        /// <param name="step">執行步驟(預設1)</param>
        /// <returns>回傳是否可執行</returns>
        internal override bool RunOneStepFlow(int step = 1, bool init = false)
        {
            bool result = false;
            GetStatus();
            if (IsFinish)
                SendIdle();
            if (IsIdle)
            {
                result = SendData(step);
                System.Threading.Thread.Sleep(250);
                SendIdle();
            }
            else
            {
                Message = "未收到PLC待機狀態，無法發送訊號";
                var logger = MainPresenter.LogDatas();
                logger = LoggerData.Error(new Exception("錯誤"), $"未收到PLC待機狀態，無法發送訊號");
            }

            return result;
        }
        /// <summary>
        /// 發送通訊關閉訊號(idle)
        /// </summary>
        /// <returns>執行成功/失敗</returns>
        internal override bool SendIdle()
        {
            string send_data = $"WRS DM00{WriteAddress} 1 0";
            string response_message = plcNet.WriteData(send_data);
            return true;
        }
        #endregion 覆寫功能
    }
}
