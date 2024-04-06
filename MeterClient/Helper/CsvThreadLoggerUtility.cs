using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterClient.Helper
{
    public class CsvThreadLoggerUtility
    {
        private static readonly object lockObj = new object();
        private static readonly object lockObj1 = new object();

        public static void Log(string client, string message)
        {
            lock (lockObj)
            {
                string logFilePath = "ThreadLog.csv";
                using (StreamWriter writer = File.AppendText(logFilePath))
                {
                    writer.WriteLine($"{client},{message},{DateTime.Now}");
                }
            }
        }


        public static void Log1(string client, string message)
        {
            lock (lockObj1)
            {
                string logFilePath = "MDCResponse.csv";
                using (StreamWriter writer = File.AppendText(logFilePath))
                {
                    writer.WriteLine($"{client},{message},{DateTime.Now}");
                }
            }
        }
    }
}
