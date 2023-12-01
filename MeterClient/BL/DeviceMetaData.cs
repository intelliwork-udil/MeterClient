using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterClient.BL
{
    public class DeviceMetaData
    {
        public int device_type { get; set; }
        public DateTime request_datetime { get; set; }
        public int communication_mode { get; set; }
        public bool bidirectional_device { get; set; }
        public int communication_type { get; set; }
        public TimeOnly initial_communication_time { get; set; }

        public int communication_interval { get; set; }
        public string sim_number { get; set; }
        public int sim_id { get; set; }
        public int mdi_reset_date { get; set; }
        public TimeOnly mdi_reset_time { get; set; }
        public int phase { get; set; }
        public int meter_type { get; set; }

        public DeviceMetaData()
        {

        }

        public DeviceMetaData(int device_type, DateTime request_datetime, int communication_mode, bool bidirectional_device, int communication_type, TimeOnly initial_communication_time, int communication_interval, string sim_number, int sim_id, int mdi_reset_date, TimeOnly mdi_reset_time, int phase, int meter_type)
        {
            this.device_type = device_type;
            this.request_datetime = request_datetime;
            this.communication_mode = communication_mode;
            this.bidirectional_device = bidirectional_device;
            this.communication_type = communication_type;
            this.initial_communication_time = initial_communication_time;
            if (communication_type == 1)
            {
                this.communication_interval = communication_interval;
            }
            else
            {
                this.communication_interval = 0;
            }
            this.sim_number = sim_number;
            this.sim_id = sim_id;
            this.mdi_reset_date = mdi_reset_date;
            this.mdi_reset_time = mdi_reset_time;
            this.phase = phase;
            this.meter_type = meter_type;
        }
    }
}
