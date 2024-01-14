using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterClient.BL
{
    /// <summary>
    /// The objective is to synchronize meter time with the MDC time
    /// </summary>
    /// <remarks>
    /// Attributes: request_datetime
    /// </remarks>
    public class TimeSynchronization
    {
        /// <summary>
        /// Date & Time at which request is made
        /// </summary>
        public DateTime request_datetime { get; set; }

        /// <summary>
        /// Default Constructor
        /// Set Defaulkt Values to Attributes
        /// </summary>
        public TimeSynchronization()
        {
            request_datetime = DateTime.Now;
        }
        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="request_datetime"></param>
        public TimeSynchronization(DateTime request_datetime)
        {
            this.request_datetime = request_datetime;
        }

        public string ProcessCommand(string re)
        {
            string command = "";

            if (re == "00 01 00 30 00 01 00 0D C0 01 81 00 01 01 00 00 09 02 FF 02 00")
            {
                string year = request_datetime.Year.ToString("X2").PadLeft(4, '0');
                string month = request_datetime.Month.ToString("X2").PadLeft(2, '0');
                string day = request_datetime.Day.ToString("X2").PadLeft(2, '0');

                year = MeterConfigurationUI.AddSpaceEveryNCharacters(year, 2);

                string date = year + " " + month + " " + day;

                command = "C4 01 81 00 09 05 " + date + " 01";
            }
            else if (re == "00 01 00 30 00 01 00 0D C0 01 81 00 01 01 00 00 07 01 FF 02 00")
            {
                string hour = request_datetime.Hour.ToString("X2").PadLeft(2, '0');
                string minute = request_datetime.Minute.ToString("X2").PadLeft(2, '0');
                string seconds = request_datetime.Second.ToString("X2").PadLeft(2, '0');



                string time = hour + " " + minute + " " + seconds;

                command = "C4 01 81 00 09 04 " + time + " FF";
            }
            else if (re.Contains("C1 01 81 00 08 00 00 01 00 00 FF 02 00 09 0C"))
            {
                re = re.Replace("00 01 00 30 00 01 00 1B C1 01 81 00 08 00 00 01 00 00 FF 02 00 09 0C ", "");

                var reArr = re.Split(' ');
                string y1Str = reArr[0];
                string y2Str = reArr[1];

                string yearStr = y1Str + y2Str;

                string monthStr = reArr[2];
                string dayStr = reArr[3];

                string hourStr = reArr[5];
                string minuteStr = reArr[6];
                string secondsStr = reArr[7];

                int year = Convert.ToInt32(yearStr, 16);
                int month = Convert.ToInt32(monthStr, 16);
                int day = Convert.ToInt32(dayStr, 16);

                int hour = Convert.ToInt32(hourStr, 16);
                int minute = Convert.ToInt32(minuteStr, 16);
                int seconds = Convert.ToInt32(secondsStr, 16);

                DateTime dt = new DateTime(year, month, day, hour, minute, seconds);

                request_datetime = dt;

                command = "C5 01 81 00";
            }



            return command;
        }
    }
}
