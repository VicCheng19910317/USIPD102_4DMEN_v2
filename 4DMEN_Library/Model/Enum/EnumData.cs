using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    public class EnumData
    {
        /// <summary>
        /// 執行緒狀態
        /// </summary>
        [Flags]
        public enum TaskStatus : int
        {
            Null = 0,
            Init = 1,
            Ready = 2,
            Running = 3,
            Busy = 4,
            Idle = 5,
            Done = 6,
            Wait = 7,
            Pause = 8,
        }
        /// <summary>
        /// Error物件的選項列舉
        /// </summary>
        [Flags]
        public enum Option : uint
        {
            Null = 0x01,

            Retry = 0x02,

            Skip = 0x04,

            Confirm = 0x08,

            OK = 0x10,
        }
    }
}
