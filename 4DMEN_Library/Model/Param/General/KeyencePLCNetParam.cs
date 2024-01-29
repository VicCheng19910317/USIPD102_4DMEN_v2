using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    public class KeyencePLCNetParam 
    {
        
        public string ReadAddress { get; set; }
        public string WriteAddress { get; set; }
        public string Message { get; set; }
        public PLCNet plcNet { get; set; } = new PLCNet();
        public int Timeout { get => plcNet.timeout; set => plcNet.timeout = value; }
    }
}
