using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterClient.Helper
{
    /// <summary>
    /// Thread-safe singleton logger providing CSV-based logging for meter client actions.
    /// </summary>
    public class Logger
    {
        private static readonly object _lock = new object();



        private static Logger instance;

        private Logger()
        {

        }


        /// <summary>
        /// Gets the singleton instance of the <see cref="Logger"/>.
        /// </summary>
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
        /// <summary>
        /// Logs a message with thread, action, and timestamp informaton to 'tcp_log.csv'.
        /// </summary>
        /// <param name="threadName">Name of the thread or MSN of the meter.</param>
        /// <param name="action">The action being performed.</param>
        /// <param name="message">The message or payload to log.</param>
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
