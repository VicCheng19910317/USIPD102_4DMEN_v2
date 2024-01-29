using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    public class LogData
    {
        /// <summary>
        /// 事件時間
        /// </summary>
        public string DateTime { get; set; }
        /// <summary>
        /// 事件等級
        /// </summary>
        public string LogLevel { get; set; }
        /// <summary>
        /// 事件類別
        /// </summary>
        public string LogClass { get; set; }
        /// <summary>
        /// 事件內容
        /// </summary>
        public string LogMessage { get; set; }
        /// <summary>
        /// 例外狀況記錄資訊
        /// </summary>
        public string LogException { get; set; }
    }
}
