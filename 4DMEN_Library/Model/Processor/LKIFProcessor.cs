using System;
using System.Runtime.InteropServices;

namespace _4DMEN_Library.Model
{
    internal class LKIFProcessor
    {
        #region 屬性參數
        /// <summary>
        /// 網路地址
        /// </summary>
        public string IP { get; set; } = "192.168.0.82";
        /// <summary>
        /// 訊息
        /// </summary>
        public string Message { get; set; } = "";
        /// <summary>
        /// 量測結果
        /// </summary>
        public float Value { get; set; } = float.NaN;
        public bool ConnectState { get; set; } = false;
        #endregion 屬性參數
        #region 外接DLL
        [DllImport("wsock32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        extern public static int inet_addr([MarshalAs(UnmanagedType.VBByRefStr)] ref string cp);
        #endregion 外接DLL
        #region 通用功能
        private string FuncQuit(string funcName, LKIF2.RC returnCode)
        {
            return IsSuccess(returnCode) ? ShowSuccessMsg(funcName, returnCode) : ShowFailureMsg(funcName, returnCode);
        }
        private bool IsSuccess(LKIF2.RC returnCode)
        {
            return returnCode == LKIF2.RC.RC_OK;
        }
        private string ShowSuccessMsg(string funcName, LKIF2.RC returnCode)
        {
            return $"{funcName} is succeeded. RC = 0x{((int)returnCode).ToString("X")}";
        }
        private string ShowFailureMsg(string funcName, LKIF2.RC returnCode)
        {
            return $"Failed in {funcName }. RC = 0x {((int)returnCode).ToString("X")}";
        }
        #endregion 通用功能
        #region 實作功能
        /// <summary>
        /// 測高歸0
        /// </summary>
        /// <param name="outNo">編號(0:入料手臂 or 1:測高)</param>
        /// <param name="onOff">1:開啟, 0:關閉</param>
        /// <returns>結果文字</returns>
        internal bool SetZeroSingle(int outNo, int onOff = 1)
        {
            if (!ConnectState)
            {
                Message = "Failed to connection.";
                return false;
            }
            LKIF2.RC result = (LKIF2.RC)0;
            result = LKIF2.LKIF2_SetZeroSingle(outNo, onOff);
            Message = FuncQuit("LKIF2_SetZeroSingle", result);
            return Message.Contains("Fail") ? false : true;
        }
        /// <summary>
        /// 取得量測高度數值
        /// </summary>
        /// <param name="pIndex">0: 入料手臂/1: 測高站</param>
        /// <returns>錯誤:NaN, 正確:實際數值</returns>
        internal float GetEstimateHieghtValue(int pIndex = 0)
        {
            if (!ConnectState)
            {
                Message = "Failed to connection.";
                return float.NaN;
            }
            LKIF2.RC result = (LKIF2.RC)0;
            LKIF2.LKIF_FLOATVALUE_OUT estimate_data = new LKIF2.LKIF_FLOATVALUE_OUT();
            estimate_data.value = pIndex;
            result = LKIF2.LKIF2_GetCalcDataSingle(pIndex, ref estimate_data);
            return (IsSuccess(result) && estimate_data.FloatResult == LKIF2.LKIF_FLOATRESULT.LKIF_FLOATRESULT_VALID) ? estimate_data.value : float.NaN;
        }
        /// <summary>
        /// 關閉設備
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal bool CloseDevice()
        {
            try
            {
                Message = FuncQuit("LKIF2_closeDeviceETHER", LKIF2.LKIF2_CloseDevice());
                ConnectState = Message.Contains("Failed");
                return true;
            }
            catch(Exception ex)
            {
                Message = ex.Message;
                return false;
            }
        }
        /// <summary>
        /// 開啟設備
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal bool OpenDevice()
        {
            LKIF2.LKIF_OPENPARAM_ETHERNET openParam = new LKIF2.LKIF_OPENPARAM_ETHERNET();
            var ip = IP;
            openParam.IPAddress.S_addr = inet_addr(ref ip);
            try
            {
                Message = FuncQuit("LKIF2_OpenDeviceETHER", LKIF2.LKIF2_OpenDeviceETHER(ref openParam));
                ConnectState = Message.Contains("succeeded");
                return true;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return false;
            }
        }
        internal bool SendActionFunction(int channel, string action)
        {
            bool success = false;
            Message = "";
            switch (action)
            {
                case "歸零":
                    success = SetZeroSingle(channel);
                    break;
                case "測高":
                    Value = GetEstimateHieghtValue(channel);
                    success = !float.IsNaN(Value);
                    break;
                case "連線":
                    success = OpenDevice();
                    break;
                case "中斷連線":
                    success = CloseDevice();
                    break;
            }
            return success;
        }
        #endregion 實作功能
    }
}
