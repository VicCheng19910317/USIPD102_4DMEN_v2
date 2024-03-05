using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    internal class LFJProcessor : ClientTCP
    {
        #region 屬性參數
        internal string IP { get => ip; set => ip = value; }
        internal int Port { get => port; set => port = value; }
        internal string Message { get; set; } = "";
        internal bool ConnectedState { get => is_connected; }
        #endregion 屬性參數
        #region 覆寫功能
        protected override string SendMessage(string message)
        {
            try
            {
                string resMessage = "";
                if (is_connected)
                {
                    NetworkStream networkStream = tcpClient.GetStream();
                    string send_command = SendCRLF ? "\r\n" : "";
                    byte[] data = Encoding.ASCII.GetBytes($"{message}{send_command}");
                    if (networkStream.WriteAsync(data, 0, data.Length).Wait(timeout))
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead = buffer.Length;
                        System.Threading.Thread.Sleep(200);
                        if (networkStream.ReadAsync(buffer, 0, buffer.Length).Wait(timeout))
                        {
                            resMessage = Encoding.ASCII.GetString(buffer).Replace("\r\n", "").Replace("\0", "");
                        }
                        else
                        {
                            resMessage = "讀取回傳訊號超時";
                            ConnectedClose();
                            StartConnected();
                        }
                    }
                    else
                    {
                        resMessage = "發送訊號超時";
                        ConnectedClose();
                        StartConnected();
                    }
                }
                else
                {
                    resMessage = "系統未連線";
                }
                System.Threading.Thread.Sleep(150);
                return resMessage;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion 覆寫功能
        #region 實作功能
        /// <summary>
        /// 設定打標文字
        /// </summary>
        /// <param name="code">打標編碼</param>
        /// <param name="data">打標內容</param>
        /// <returns>成功/失敗</returns>
        internal bool SetMarkText(string code, string data)
        {
            var success = false;
            if (!is_connected)
            {
                Message = "尚未連線，請先進行連線確認。";
                return success;
            }
            success = SendAction($"{code};{data}", "");
            return success;
        }
        /// <summary>
        /// 設定打標QR Code
        /// </summary>
        /// <param name="code">打標編碼</param>
        /// <param name="data">打標內容</param>
        /// <returns>成功/失敗</returns>
        internal bool SetMarkCode(string code, string data)
        {
            var success = false;
            if (!is_connected)
            {
                Message = "尚未連線，請先進行連線確認。";
                return success;
            }
            success = SendAction($"{code};{data}", "");
            return success;
        }
        /// <summary>
        /// 開始打標
        /// </summary>
        /// <param name="code">打標編碼</param>
        /// <returns>成功/失敗</returns>
        internal bool StartMarking(string code)
        {
            var success = false;
            if (!is_connected)
            {
                Message = "尚未連線，請先進行連線確認。";
                return success;
            }
            success = SendAction($"{code}", "");
            return success;
        }
        /// <summary>
        /// 設定文字偏移位置
        /// </summary>
        /// <param name="code">打標編碼</param>
        /// <param name="x">位置X</param>
        /// <param name="y">位置Y</param>
        /// <param name="a">位置A</param>
        /// <returns>成功/失敗</returns>
        internal bool SetShiftData(string code,int x,int y,int a)
        {
            var success = false;
            if (!is_connected)
            {
                Message = "尚未連線，請先進行連線確認。";
                return success;
            }
            success = SendAction($"{code};{x};{y};{a}", "");
            return success;
        }
        /// <summary>
        /// 設定打標文字與條碼
        /// </summary>
        /// <param name="param">打標參數</param>
        /// <returns>成功/失敗</returns>
        internal bool SetTextParam(MarkingParam param)
        {
            string _message = "";
            var success_fst = SetMarkText(param.marking_fst_code, param.marking_fst_txt);
            _message += $"{Message}\n";
            var success_snd = SetMarkText(param.marking_snd_code, $"{param.marking_snd_txt}{param.marking_snd_index_txt}");
            _message += $"{Message}\n";
            var success_code = SetMarkCode(param.marking_2d_code, param.marking_2d_result);
            _message += Message;
            Message = _message;
            return success_fst && success_snd && success_code;
        }
        /// <summary>
        /// 自定義比對動作
        /// </summary>
        /// <param name="action">輸入動作</param>
        /// <returns>成功/失敗</returns>
        internal virtual bool SendAction(string action, string match_data)
        {
            var result = false;
            string message = SendMessage($"{action}");
            if (message.ToUpper().Contains(match_data))
            {
                var res_data = message.Replace("\r\n", "");
                
                Message = $"雷雕發送訊號成功，發送內容:{action}，回傳內容:{message}";
                var logger = MainPresenter.LogDatas();
                logger = LoggerData.Info($"雷雕發送訊號成功，發送內容:{action}，回傳內容:{message}");
                result = true;
            }
            else
            {
                Message = $"雷雕發送比對資訊失敗，發送內容:{action}，比對資料:{match_data}";
                var logger = MainPresenter.LogDatas();
                logger = LoggerData.Error(new Exception("錯誤"), $"雷雕發送比對資訊失敗，發送內容:{action}，比對資料:{match_data}");
            }
            return result;
        }
        #endregion 實作功能
    }
}
