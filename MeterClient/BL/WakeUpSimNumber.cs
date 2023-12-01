using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterClient.BL
{
    public class WakeUpSimNumber
    {
        public DateTime request_datetime { get; set; }
        public string wakeup_number_1 { get; set; }
        public string wakeup_number_2 { get; set; }
        public string wakeup_number_3 { get; set; }

        public WakeUpSimNumber()
        {

        }

        public WakeUpSimNumber(DateTime request_datetime, string wakeup_number_1, string wakeup_number_2, string wakeup_number_3)
        {
            this.request_datetime = request_datetime;
            this.wakeup_number_1 = wakeup_number_1;
            this.wakeup_number_2 = wakeup_number_2;
            this.wakeup_number_3 = wakeup_number_3;
        }
    }
}
