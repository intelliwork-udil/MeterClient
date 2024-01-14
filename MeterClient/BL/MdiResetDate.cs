using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterClient.BL
{
    public class MdiResetDate
    {
        public DateTime request_datetime { get; set; }
        public int mdi_reset_date { get; set; }
        public TimeOnly mdi_reset_time { get; set; }
        public MdiResetDate()
        {
            request_datetime = DateTime.Now;
            mdi_reset_date = 1;
            mdi_reset_time = new TimeOnly(0, 0);
        }

        public MdiResetDate(DateTime request_datetime, int mdi_reset_date, TimeOnly mdi_reset_time)
        {
            this.request_datetime = request_datetime;
            this.mdi_reset_date = mdi_reset_date;
            this.mdi_reset_time = mdi_reset_time;
        }
        public string ProcessCommand(string re)
        {
            string command = "";

            if (re.Contains("C0 01 81 00 16 00 00 0F 00 00 FF 04 00"))
            {
                string mdi_reset_date_hex = mdi_reset_date.ToString("X").PadLeft(2, '0');

                string mdi_reset_time_hex = mdi_reset_time.Hour.ToString("X").PadLeft(2, '0') + " " + mdi_reset_time.Minute.ToString("X").PadLeft(2, '0') + " " + mdi_reset_time.Second.ToString("X").PadLeft(2, '0');

                command = "C4 01 81 00 01 01 02 02 09 04 " + mdi_reset_time_hex + " 00 09 05 FF FF FF " + mdi_reset_date_hex + " FF";
            }

            return command;
        }
    }
}
