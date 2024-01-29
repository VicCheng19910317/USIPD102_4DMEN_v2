using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    public class SocketTCP
    {
        #region 屬性參數
        /// <summary>
        /// IP地址
        /// </summary>
        protected internal IPAddress IP { get; set; } = IPAddress.Any;
        /// <summary>
        /// Socket監聽器
        /// </summary>
        protected internal Socket listener { get; set; } = null;
        /// <summary>
        /// 端口
        /// </summary>
        protected internal int port { get; set; } = 1234;
        /// <summary>
        /// 同時最高連線數量
        /// </summary>
        protected internal int listenCount { get; set; } = 10;

        protected internal bool StartListen { get; set; } = true;
        #endregion 屬性參數
        #region 虛擬功能
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns>成功/失敗</returns>
        protected internal virtual bool Open()
        {
            try
            {
                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var point = new IPEndPoint(IP, port);
                listener.Bind(point);
                listener.Listen(10);
                StartListen = true;
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 接收訊號基本架構
        /// </summary>
        protected internal virtual void ReceiveData()
        {
            while (StartListen)
            {
                try
                {
                    var sender = listener.Accept();
                    var bytes = new byte[1024];
                    int bytesLen = sender.Receive(bytes);
                    var data = Encoding.ASCII.GetString(bytes, 0, bytesLen);
                    RunFlow(data, sender);
                }
                catch(Exception ex)
                {

                }
            }
        }
        /// <summary>
        /// 執行流程
        /// </summary>
        /// <param name="request">收到內容</param>
        protected internal virtual void RunFlow(string request, Socket sender)
        {
            
        }
        /// <summary>
        /// 傳送結果回去到Client端
        /// </summary>
        /// <param name="response">回復訊息</param>
        /// <param name="sender">要回的client socket</param>
        protected internal virtual void SendResponse(string response, Socket sender)
        {
            byte[] msg = Encoding.ASCII.GetBytes(response);
            sender.Send(msg);
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }
        #endregion 虛擬功能
    }
}
