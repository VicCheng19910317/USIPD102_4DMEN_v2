using NLog;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    public class SerialRS232
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        protected SerialPort serialPort = null;
        public bool IsOpen
        {
            get
            {
                if (serialPort == null)
                    return false;
                return serialPort.IsOpen;
            }
        }
        public static string[] GetPortNames
        {
            get => SerialPort.GetPortNames();
        }
        protected int baud_rate = 9600;
        protected string comport = "COM5";
        protected int parity = 0;
        protected int data_bit = 8;
        protected int stop_bit = 1;
        public bool SendCRLF { get; set; } = true;
        protected int timeout { get; set; } = 2000;

        public SerialRS232()
        {

        }
        public SerialRS232(string _comm, int _baud, int _par, int _data, int _stop)
        {
            comport = _comm;
            baud_rate = _baud;
            parity = _par;
            data_bit = _data;
            stop_bit = _stop;
        }

        public bool PortOpen()
        {
            try
            {
                if (serialPort != null && serialPort.IsOpen)
                    return false;
                serialPort = new SerialPort(comport, baud_rate, (Parity)parity, data_bit, (StopBits)stop_bit);
                serialPort.ReadTimeout = timeout;
                serialPort.WriteTimeout = timeout;
                if (!serialPort.IsOpen)
                    serialPort.Open();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
            
        }
        public bool PortOpen(string _comm, int _baud, int _par, int _data, int _stop)
        {
            comport = _comm;
            baud_rate = _baud;
            parity = _par;
            data_bit = _data;
            stop_bit = _stop;
            if (serialPort != null && serialPort.IsOpen)
                return false;
            serialPort = new SerialPort(comport, baud_rate, (Parity)parity, data_bit, (StopBits)stop_bit);
            serialPort.ReadTimeout = timeout;
            serialPort.WriteTimeout = timeout;
            if (!serialPort.IsOpen)
                serialPort.Open();
            return true;
        }
        public bool PortClose()
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }
            return true;
        }
        public virtual string SendMessage(string message)
        {
            if (serialPort == null || !serialPort.IsOpen) return "comport is not connnected.";
            string send_command = SendCRLF ? "\r\n" : "";
            string send_data = $"{message}{send_command}";
            serialPort.Write(send_data);
            var receive_message = serialPort.ReadExisting();
            return receive_message;
        }
       
    }
}
