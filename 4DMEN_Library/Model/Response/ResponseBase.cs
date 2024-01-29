using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    public class ResponseBase
    {
        public int Stream { get; set; }
        public int Function { get; set; }
    }
    public class ResultResponse : ResponseBase
    {
        public string Result { get; set; }
    }
    public class ListResponse : ResponseBase
    {
        /// <summary>
        /// S2F13 Request
        /// </summary>
        public Dictionary<string, string> List { get; set; }
    }
    public class DateTimeResponse: ResponseBase
    {
        public string Datetime { get; set; }
    }
    public class RecipeResponse: ResponseBase
    {
        public string RecipeBody { get; set; }
    }
    public class ListArrayResponse : ResponseBase
    {
        public List<string> List { get; set; }
    }
}
