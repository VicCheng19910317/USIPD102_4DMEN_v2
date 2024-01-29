using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    public class SocketRequest
    {
        public int Stream { get; set; }
        public int Function { get; set; }
        /// <summary>
        /// S2F13 Request
        /// </summary>
        public List<string> List { get; set; }
        /// <summary>
        /// S2F15 Request
        /// </summary>
        public Dictionary<int,dynamic> ECList { get; set; }
        /// <summary>
        /// S2F31 Request
        /// </summary>
        public DateTime DateTime { get; set; }
        /// <summary>
        /// S2F41 Request
        /// </summary>
        public CommandParam Command { get; set; }
        /// <summary>
        /// S5F1 Request, Y 代表报警开始, N 代表报警结束
        /// </summary>
        public string AlarmSet { get; set; }
        /// <summary>
        /// S5F1 Request
        /// </summary>
        public int AlarmID { get; set; }
        /// <summary>
        /// S5F1 Request
        /// </summary>
        public string AlarmText { get; set; }
        /// <summary>
        /// S6F11 Request
        /// </summary>
        public int EventID { get; set; }
        /// <summary>
        /// S6F11 Request
        /// </summary>
        public Dictionary<string,dynamic> Reports { get; set; }
        /// <summary>
        /// S7F3 Request
        /// </summary>
        public string RecipeBody { get; set; }
        /// <summary>
        /// S7F5, S7F17 Request
        /// </summary>
        public string RecipeName { get; set; }
        /// <summary>
        /// S10F3 Request
        /// </summary>
        public string Message { get; set; }
    }
    public class CommandParam
    {
        public string Name { get; set; }
        public Dictionary<string, dynamic> Parameter { get; set; }
    }
}
