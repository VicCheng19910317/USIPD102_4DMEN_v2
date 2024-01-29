using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    public class ClientTCP
    {
        protected TcpClient tcpClient = null;
        protected string ip { get; set; } = "127.0.0.1";
        protected int port { get; set; } = 2000;
        protected int timeout { get; set; } = 10000;
        protected bool SendCRLF { get; set; } = true;
        protected string message { get; set; } = "";
        protected bool is_connected { get; set; } = false;
        internal bool StartConnected()
        {
            try
            {
                if (tcpClient != null && tcpClient.Client != null && tcpClient.Connected) return true;
                tcpClient = new TcpClient() { ReceiveTimeout = timeout, SendTimeout = timeout };
                if (!tcpClient.ConnectAsync(ip, port).Wait(timeout))
                {
                    is_connected = false;
                    message = "系統連線失敗";
                    return false;
                }
                is_connected = true;
                message = "系統已連線";
                System.Threading.Thread.Sleep(250);
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
        protected bool TestConnection()
        {
            StartConnected();
            if (is_connected)
                ConnectedClose();
            return is_connected;
        }
        protected virtual bool TestConnection(string _ip, int _port)
        {
            ip = _ip;
            port = _port;
            StartConnected();
            if (is_connected)
                ConnectedClose();
            return is_connected;
        }
        protected virtual string SendMessage(string message)
        {
            try
            {
                string resMessage = "";
                if (StartConnected())
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
                        }
                    }
                    else
                    {
                        resMessage = "發送訊號超時";
                        ConnectedClose();
                    }
                }
                else
                {
                    resMessage = "系統未連線";
                }
                System.Threading.Thread.Sleep(150);
                return resMessage;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }
        protected virtual string SendMessage(string _ip, int _port, string message)
        {
            ip = _ip;
            port = _port;
            string resMessage = "";
            if (StartConnected())
            {
                NetworkStream networkStream = tcpClient.GetStream();
                string send_command = SendCRLF ? "\r\n" : "";
                byte[] data = Encoding.ASCII.GetBytes($"{message}{send_command}");
                if (networkStream.WriteAsync(data, 0, data.Length).Wait(timeout))
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = buffer.Length;
                    if (networkStream.ReadAsync(buffer, 0, buffer.Length).Wait(timeout))
                    {
                        resMessage = Encoding.ASCII.GetString(buffer);
                    }
                    else
                    {
                        resMessage = "讀取回傳訊號超時";
                    }
                }
                else
                {
                    resMessage = "發送訊號超時";
                }
                ConnectedClose();
            }
            else
            {
                resMessage = "系統未連線";
            }
            return resMessage;
        }

    }
}
