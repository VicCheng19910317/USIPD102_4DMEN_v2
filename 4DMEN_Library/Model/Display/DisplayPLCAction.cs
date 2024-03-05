using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    public class DisplayPLCAction
    {
        internal string _station;
        public string Station
        {
            get
            {
                if (_station == "Main")
                    return "主流道站(A)";
                else if (_station == "In")
                    return "入料站Loader";
                else if (_station == "Out")
                    return "出料站Loader";
                else if (_station == "Lid")
                    return "組裝站Loader";
                else if (_station == "Nut")
                    return "螺帽站(B3)";
                else if (_station == "Bend")
                    return "折彎站(B4)";
                else if (_station == "Plate")
                    return "下壓站(B4)";
                else if (_station == "Height")
                    return "測高站(B5)";
                else if (_station == "NG")
                    return "NG站(B6)";
                else
                    return _station;
            }
            set => _station = value;
        }
        public string ReadAddress { get; set; }
        public string WriteAddress { get; set; }
        public string ActionDetail { get; set; }
    }
}
