using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    internal class RS232Processor : RS232Param
    {
        private double _value = double.NaN;
        /// <summary>
        /// 秤重重量
        /// </summary>
        internal double Value
        {
            get
            {
                return Math.Round(_value, 4);
            }
            set => _value = value;
        }
        internal string Message { get; set; }
        /// <summary>
        /// 資料轉換使用
        /// </summary>
        /// <param name="param">RS232Param參數</param>
        internal void SetRS232Param(RS232Param param)
        {
            Comport = param.Comport;
            BaudRate = param.BaudRate;
            DataBits = param.DataBits;
            Parity = param.Parity;
            StopBits = param.StopBits;
            Timeout = param.Timeout;
        }
        internal bool SendActionFunction(string action)
        {
            var success = false;
            switch (action)
            {

                case "Open":
                    success = Open();
                    Message = "Open Port Success.";
                    Value = 0;
                    break;
                case "Close":
                    success = Close();
                    Message = "Close Port Success.";
                    Value = 0;
                    break;
                case "O8":
                    success = GetWeight();
                    Message = success ? "Get Weight Success." : "Get Weight Fail.";
                    break;
                case "T":
                    success = ResetToZero();
                    break;
            }
            return success;
        }
        /// <summary>
        /// 開啟通訊
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal bool Open()
        {
            return PortOpen();
        }
        /// <summary>
        /// 關閉通訊
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal bool Close()
        {
            return PortClose();
        }
        /// <summary>
        /// 取得重量數值
        /// </summary>
        /// <returns>成功(實際數值)/失敗(Double.NaN)</returns>
        public bool GetWeight()
        {
            var result = SendMessage("O8");
            if (result == "comport is not connnected.")
            {
                _value = double.NaN;
                return false;
            }

            var res = result.Split('\n');
            if (res.Length == 1 && res[0] == "")
            {
                System.Threading.Thread.Sleep(200);
                GetWeight();
                return true;
            }

            if (res.Length > 2)
            {
                var _data = res[1].Replace("\r", "").Replace("G", "").Replace("U", "").Replace("e", "").Replace("S", "").Trim().Replace(" ", "");
                if (!double.TryParse(_data.Trim(), out _value))
                {
                    _value = double.NaN;
                    return false;
                }
                return true;
            }
            else
            {
                _value = double.NaN;
                return false;
            }
        }
        /// <summary>
        /// 秤重機歸0
        /// </summary>
        /// <returns>成功/失敗</returns>
        public bool ResetToZero()
        {
            var result = SendMessage("T ");
            if (result == "comport is not connnected.")
                return false;
            return true;

        }
    }
}
