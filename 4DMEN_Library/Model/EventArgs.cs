using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace _4DMEN_Library.Model
{
    public class UpdateLoggerArgs : EventArgs
    {
        public List<LogData> Datas { get; set; }
    }

    public class SendMessageBoxArgs : EventArgs
    {
        public string Name { get; set; }
        public MessageBoxButton Button { get; set; } = MessageBoxButton.OK;
        public MessageBoxImage Image { get; set; } = MessageBoxImage.Error;
        public string Message { get; set; }
    }
    public class SendInitResponseArgs : EventArgs
    {
        public bool success { get; set; }
        public string message { get; set; }
    }
}
