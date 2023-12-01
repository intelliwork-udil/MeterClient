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
    }
}
