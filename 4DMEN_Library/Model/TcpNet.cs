using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    public class TcpNet
    {
        internal int ID { get; set; }
        internal TcpClient tcpClient = null;
        internal string IP { get; set; } = "127.0.0.1";
        internal int Port { get; set; } = 2000;
        internal int Timeout { get; set; } = 10000;
        internal bool SendCRLF { get; set; } = true;
        internal string message { get; set; } = "";
        internal bool IsConnected { get; set; } = false;
        internal bool IsSending { get; set; } = false;
        internal bool StartConnected()
        {
            try
            {
                if (tcpClient != null && tcpClient.Client != null && tcpClient.Connected) return true;
                tcpClient = new TcpClient() { ReceiveTimeout = Timeout, SendTimeout = Timeout };
                if (!tcpClient.ConnectAsync(IP, Port).Wait(Timeout))
                {
                    IsConnected = false;
                    message = "系統連線失敗";
                    return false;
                }
                IsConnected = true;
                message = "系統已連線";
                System.Threading.Thread.Sleep(200);
                return true;
            }
            catch (SocketException e)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        internal bool StartConnected(string ip,int port)
        {
            try
            {
                IP = ip;
                Port = port;
                if (tcpClient != null && tcpClient.Client != null && tcpClient.Connected) return true;
                tcpClient = new TcpClient() { ReceiveTimeout = Timeout, SendTimeout = Timeout };
                if (!tcpClient.ConnectAsync(ip, port).Wait(Timeout))
                {
                    IsConnected = false;
                    message = "系統連線失敗";
                    return false;
                }
                IsConnected = true;
                message = "系統已連線";
                System.Threading.Thread.Sleep(200);
                return true;
            }
            catch (SocketException e)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        internal virtual bool ConnectedClose()
        {
            try
            {
                tcpClient.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        internal virtual string SendMessage(string message)
        {
            string resMessage = "";
            try
            {
                
                if (!IsConnected)
                {
                    resMessage = $"PLC未連線";
                }
                else
                {
                    NetworkStream networkStream = tcpClient.GetStream();
                    string send_command = SendCRLF ? "\r\n" : "";
                    byte[] data = Encoding.ASCII.GetBytes($"{message}{send_command}");
                    if (networkStream.WriteAsync(data, 0, data.Length).Wait(Timeout))
                    {
                        try
                        {
                            System.Threading.Thread.Sleep(200);
                            byte[] buffer = new byte[1024];
                            int bytesRead = buffer.Length;
                            if (networkStream.ReadAsync(buffer, 0, buffer.Length).Wait(Timeout))
                                resMessage = Encoding.ASCII.GetString(buffer).Replace("\0", "").Replace("\r\n", "");
                            else
                            {
                                resMessage = $"PLC發送訊號超時";
                                ConnectedClose();
                                StartConnected();
                            }
                        }
                        catch (Exception ex)
                        {
                            ConnectedClose();
                            StartConnected();
                            throw new Exception($"PLC讀取錯誤：{ex.Message}");
                        }
                    }
                    else
                    {
                        resMessage = $"PLC發送訊號超時";
                        ConnectedClose();
                        StartConnected();
                    }
                }
                IsSending = false;
                RecordData.RecordProcessData(MainPresenter.SystemParam(), $"send message:{message},receive message:{resMessage}");
                return resMessage;
            }
            catch (Exception ex)
            {
                ConnectedClose();
                StartConnected();
                resMessage = $"發送手臂訊號錯誤，錯誤訊號:{ex.Message}";
                IsSending = false;
                return resMessage;
            }

        }

    }
    
}
