using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterClient.BL
{
    public class MeterStatus
    {
        public DateTime request_datetime { get; set; }
        public bool meter_activation_status { get; set; }
        public MeterStatus()
        {

        }

        public MeterStatus(DateTime request_datetime, bool meter_activation_status)
        {
            this.request_datetime = request_datetime;
            this.meter_activation_status = meter_activation_status;
        }
    }
}
