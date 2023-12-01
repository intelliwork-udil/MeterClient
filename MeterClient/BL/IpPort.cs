using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterClient.BL
{
    public class IpPort
    {
        public string primary_ip_address { get; set; }
        public string secondary_ip_address { get; set; }
        public int primary_port { get; set; }
        public int secondary_port { get; set; }
        public DateTime request_datetime { get; set; }

        public IpPort()
        {

        }

        public IpPort(string primary_ip_address, string secondary_ip_address, int primary_port, int secondary_port, DateTime request_datetime)
        {
            this.primary_ip_address = primary_ip_address;
            this.secondary_ip_address = secondary_ip_address;
            this.primary_port = primary_port;
            this.secondary_port = secondary_port;
            this.request_datetime = request_datetime;
        }
    }
}
