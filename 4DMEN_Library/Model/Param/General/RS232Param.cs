using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    public class RS232Param : SerialRS232
    {
        public string Comport { get => comport; set => comport = value; }
        public int BaudRate { get => baud_rate; set => baud_rate = value; }
        public int DataBits { get => data_bit; set => data_bit = value; }
        public int Parity { get=> parity; set => parity = value; }
        public int StopBits { get=> stop_bit; set => stop_bit = value; }
        public int Timeout { get => timeout; set => timeout = value; }
        public List<string> Action_Name { get; set; }
    }
}
