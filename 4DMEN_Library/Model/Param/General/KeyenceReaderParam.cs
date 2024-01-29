using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    public class KeyenceReaderParam : ClientTCP
    {
        public string Name { get; set; }
        public string IP { get => base.ip; set => base.ip = value; }
        public int Port { get => port; set => port = value; }
        public bool IsConnected { get => is_connected; set => is_connected = value; }
        public string Message { get => message; set => message = value; }
        public int Timeout { get => timeout; set => timeout = value; }
        public List<string> Action_Name { get; set; }
    }
}
