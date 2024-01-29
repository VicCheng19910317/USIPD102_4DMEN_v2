using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    public class EsponArmsParam : ClientTCP
    {
        #region 參數屬性
        public string Name { get; set; }
        public string IP { get => base.ip; set => base.ip = value; }
        public int Port { get => port; set => port = value; }
        public bool IsConnected { get => is_connected; set => is_connected = value; }
        public string Message { get => message; set => message = value; }
        public int Timeout { get => timeout; set => timeout = value; }
        public List<string> Action_Name { get; set; }
        #endregion 參數屬性
        #region 登入參數屬性
        public int LoginPort { get; set; } = 5000;
        protected TcpClient LoginClient = null;
        public bool IsLogin { get; set; } = false;
        public bool IsLoginConnection { get; set; } = false;
        #endregion 登入參數屬性
    }


}
