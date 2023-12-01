using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterClient.BL
{
    public class MeterDataSampling
    {
        public DateTime request_datetime { get; set; }
        public DateTime activation_datetime { get; set; }
        public string data_type { get; set; }
        public int sampling_interval { get; set; }
        public int sampling_initial_time { get; set; }
        public MeterDataSampling()
        {

        }

        public MeterDataSampling(DateTime request_datetime, DateTime activation_datetime, string data_type, int sampling_interval, int sampling_initial_time)
        {
            this.request_datetime = request_datetime;
            this.activation_datetime = activation_datetime;
            this.data_type = data_type;
            this.sampling_interval = sampling_interval;
            this.sampling_initial_time = sampling_initial_time;
        }
    }
}
