using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterClient.BL
{
    public class ActivateMeterOpticalPort
    {
        public DateTime request_datetime { get; set; }
        public DateTime optical_port_on_datetime { get; set; }
        public DateTime optical_port_off_datetime { get; set; }
        public ActivateMeterOpticalPort()
        {
            request_datetime = DateTime.Now;
            optical_port_on_datetime = DateTime.Now;
            optical_port_off_datetime = DateTime.Now;
        }

        public ActivateMeterOpticalPort(DateTime request_datetime, DateTime optical_port_on_datetime, DateTime optical_port_off_datetime)
        {
            this.request_datetime = request_datetime;
            this.optical_port_on_datetime = optical_port_on_datetime;
            this.optical_port_off_datetime = optical_port_off_datetime;
        }
    }
}
