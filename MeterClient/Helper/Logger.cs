using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterClient.Helper
{
    public class Logger
    {
        private static readonly object _lock = new object();



        private static Logger instance;

        private Logger()
        {

        }


        public static Logger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Logger();
                }
                return instance;
            }
        }
        public void Log(string threadName, string action, string message)
        {
            lock (_lock)
            {
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss},{threadName},{action},{message}";
                File.AppendAllLines("tcp_log.csv", new[] { logEntry });
            }
        }
    }
}
