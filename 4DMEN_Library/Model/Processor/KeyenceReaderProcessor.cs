using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    internal class KeyenceReaderProcessor : KeyenceReaderParam
    {
        internal string ReadResult = "";
        /// <summary>
        /// 資料轉換使用
        /// </summary>
        /// <param name="param">ReaderParam參數</param>
        internal void SetReaderParam(KeyenceReaderParam param)
        {
            IP = param.IP;
            Port = param.Port;
            Timeout = param.Timeout;
        }
        /// <summary>
        /// 讀取條碼
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool Read()
        {
            return SendAction($"LON", "");
        }
        /// <summary>
        /// 關閉讀碼
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool Close()
        {
            return SendAction($"LOFF", "ERROR");
        }
        /// <summary>
        /// 離開讀碼
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool Quit()
        {
            return SendAction($"QUIT", "QUIT");
        }
        /// <summary>
        /// 自定義Reader動作
        /// </summary>
        /// <param name="action">輸入動作</param>
        /// <returns>成功/失敗</returns>
        internal virtual bool SendAction(string action)
        {
            var result = false;
            string message = SendMessage(action);
            if (message.ToUpper().Contains("OK"))
            {
                result = true;
                Message = $"發送訊號成功，回傳結果：{message}";
            }
            else
                Message = $"發送訊號比對錯誤，發送訊號:{action}，比對訊號:OK";

            return result;
        }
        /// <summary>
        /// 自定義Reader動作
        /// </summary>
        /// <param name="action">輸入動作</param>
        /// <param name="match_data">比對結果</param>
        /// <returns>成功/失敗</returns>
        internal virtual bool SendAction(string action, string match_data)
        {
            var result = false;
            string message = SendMessage(action);
            ReadResult = "";
            if (message.ToUpper().Contains(match_data))
            {
                ReadResult = message.Split(':')[0].Replace("\r", "");
                result = true;
                Message = $"發送訊號成功，回傳結果：{message}";
            }
            else
                Message = $"發送訊號比對錯誤，發送訊號:{action}，比對訊號:{match_data}";
            return result;
        }

        internal bool SendActionFunction(string action)
        {
            var success = false;
            switch (action)
            {
                case "TEST":
                    success = TestConnection();
                    break;
                case "READ":
                    success = Read();
                    break;
                case "CLOSE":
                    success = Close();
                    break;
                case "QUIT":
                    success = Quit();
                    break;
            }
            return success;
        }
    }
}
