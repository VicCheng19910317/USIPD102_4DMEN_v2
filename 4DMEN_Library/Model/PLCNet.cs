using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    public class PLCNet
    {
        #region 類別變數
        /// <summary>
        /// 連線狀態
        /// </summary>
        public bool Connect { get; internal set; } = false;
        public string IP { get; set; } = "192.168.0.10";
        public int Port { get; set; } = 8501;
        public int timeout 
        { 
            get => WriteNet[0].Timeout; 
            set 
            {
                foreach(var read in ReadNet)
                {
                    read.Timeout = value;
                }
                foreach (var write in WriteNet)
                {
                    write.Timeout = value;
                }
            } 
        }
        private const int SocketsQty = 7;
        internal TcpNet[] WriteNet { get; set; }
        internal TcpNet[] ReadNet { get; set; }
        #endregion 類別變數
        #region 類別建構子
        public PLCNet()
        {

            ReadNet = new TcpNet[SocketsQty + 1];
            WriteNet = new TcpNet[SocketsQty];
            for (int i = 0; i < SocketsQty; i++)
            {
                ReadNet[i] = new TcpNet { ID = i};
                WriteNet[i] = new TcpNet { ID = i };
            }
            ReadNet[SocketsQty] = new TcpNet { ID = SocketsQty };
        }
        #endregion 類別建構子

        #region 類別方法
        /// <summary>
        /// 初始化
        /// </summary>
        public bool Init(string ip, int port)
        {
            for (int i = 0; i < SocketsQty; i++)
            {
                if (!ReadNet[i].StartConnected(IP, Port))
                    return false;

                if (!WriteNet[i].StartConnected(IP, Port))
                    return false;
            }

            if (!ReadNet[SocketsQty].StartConnected(IP, Port))
                return false;

            Connect = true;
            return Connect;

        }
        /// <summary>
        /// 初始化
        /// </summary>
        public bool Init()
        {
            for (int i = 0; i < SocketsQty; i++)
            {
                if (!ReadNet[i].StartConnected(IP, Port))
                    return false;

                if (!WriteNet[i].StartConnected(IP, Port))
                    return false;
            }

            if (!ReadNet[SocketsQty].StartConnected(IP, Port))
                return false;

            Connect = true;
            return Connect;

        }
        /// <summary>
        /// 關閉並釋放連接
        /// </summary>
        public void CloseConnect()
        {
            Task.Run(() =>
            {
                if (Connect)
                {
                    Connect = false;
                    SpinWait.SpinUntil(() =>
                    WriteNet.ToList().TrueForAll(node => !node.IsSending) &&
                    ReadNet.ToList().TrueForAll(node => !node.IsSending),
                    Timeout.Infinite);
                    WriteNet.ToList().ForEach(node => node.ConnectedClose());
                    WriteNet.ToList().ForEach(node => node.tcpClient.Dispose());
                    ReadNet.ToList().ForEach(node => node.ConnectedClose());
                    ReadNet.ToList().ForEach(node => node.tcpClient.Dispose());
                }
            });

        }
        /// <summary>
        /// 讀取資料
        /// </summary>
        /// <param name="send_string">發送內容</param>
        public string ReadData(string send_string)
        {
            if (!Connect) return "PLC未連線";
            var client = GetReadIdleKVSockets();
            return client.SendMessage(send_string);
        }
        /// <summary>
        /// 寫入資料
        /// </summary>
        /// <param name="send_string">發送內容</param>
        public string WriteData(string send_string)
        {
            if (!Connect) return "PLC未連線";
            var client = GetWriteIdelKVSockets();
            return client.SendMessage(send_string);

        }
        /// <summary>
        /// 取得閒置的讀取通訊元件
        /// </summary>
        /// <returns>閒置TCP</returns>
        private TcpNet GetReadIdleKVSockets()
        {
            lock (ReadNet)
            {
                SpinWait.SpinUntil(() => ReadNet.Count(node => !node.IsSending) > 0x00, Timeout.Infinite);

                var myKVSockets = ReadNet.ToList().First(node => !node.IsSending);

                myKVSockets.IsSending = true;

                return myKVSockets;
            }
        }
        /// <summary>
        /// 取得閒置的寫入通訊元件
        /// </summary>
        /// <returns></returns>
        private TcpNet GetWriteIdelKVSockets()
        {
            lock (WriteNet)
            {
                SpinWait.SpinUntil(() => WriteNet.Count(node => !node.IsSending) > 0x00, Timeout.Infinite);

                var myKVSockets = WriteNet.ToList().First(node => !node.IsSending);

                myKVSockets.IsSending = true;

                return myKVSockets;
            }
        }
        #endregion 類別方法
    }
}
