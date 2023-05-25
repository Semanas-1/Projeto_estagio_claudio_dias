using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto_2_dia
{
    public enum LogType
    {
        Info,
        Error,
        Warning
    }

    public class Log
    {
        public string Message { get; set; }
        public string Time { get; set; }
        public LogType Type { get; set; }
        public string ThreadName { get; set; }
    }
}
