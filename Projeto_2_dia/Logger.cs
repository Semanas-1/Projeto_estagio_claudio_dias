using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto_2_dia
{
    public class Logger
    {
        private string threadName;
        public Logger(string threadName)
        {
            this.threadName = threadName;
        }

        // Add log to program
        public void Log(string message, LogType type = LogType.Info)
        {
            Program.AddLog(new Log()
            {
                Message = message,
                Time = DateTime.Now.ToString("HH:mm:ss"),
                Type = type,
                ThreadName = threadName
            });
        }

    }
}
