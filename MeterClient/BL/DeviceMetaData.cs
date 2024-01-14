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


        public void PerformCommand(string command)
        {
            if (command.Contains("C1 01 81 00 01 00 00 60 3C 06 FF 02 00 02 02 09 03"))
            {
                command = command.Replace("C1 01 81 00 01 00 00 60 3C 06 FF 02 00 02 02 09 03 ", "");

                var cmdArr = command.Split(' ');

                int hr = Convert.ToInt32(cmdArr[0], 16);
                int min = Convert.ToInt32(cmdArr[1], 16);
                int sec = Convert.ToInt32(cmdArr[2], 16);

                initial_communication_time = new TimeOnly(hr, min, sec);


                communication_interval = Convert.ToInt32(cmdArr[cmdArr.Length - 1], 16) + Convert.ToInt32(cmdArr[cmdArr.Length - 2], 16);


                Console.WriteLine("Initial Communication Time: " + initial_communication_time.ToString());
            }

            else if (command.Contains("C1 01 81 00 01 00 00 60 3C 12 FF 02 00 11"))
            {
                var val = command[command.Length - 1];
                if (val == '1')
                {
                    communication_type = 1;
                }
                else
                {
                    communication_type = 2;
                }
                Console.WriteLine("Communication Type: " + communication_type);
            }
            else if (command.Contains("C1 01 81 00 01 00 00 60 3C 0B FF 02 00 03 00"))
            {
                Console.WriteLine("Nothing Proceed");
                // Nothing in protocol Document
            }
            else if (command.Contains("C1 01 81 00 16 00 00 0F 00 00 FF 04 00 01 02 02 04 09 04"))
            {
                command = command.Replace("C1 01 81 00 16 00 00 0F 00 00 FF 04 00 01 02 02 04 09 04 ", "");

                var cmdArr = command.Split(' ');

                int hr = Convert.ToInt32(cmdArr[0], 16);
                int min = Convert.ToInt32(cmdArr[1], 16);
                int sec = Convert.ToInt32(cmdArr[2], 16);

                mdi_reset_time = new TimeOnly(hr, min, sec);


                mdi_reset_date = Convert.ToInt32(cmdArr[cmdArr.Length - 2], 16);

                Console.WriteLine("MDI Reset Time: " + mdi_reset_time.ToString());
            }
            else if (command.Contains("C1 01 81 00 01 00 00 5E 42 1E FF 02 00 11 "))
            {
                var val = command[command.Length - 1];
                if (val == '1')
                {
                    bidirectional_device = false;
                }
                else
                {
                    bidirectional_device = true;
                }

                Console.WriteLine("Bidirectional Device: " + bidirectional_device);
            }
        }

        public string GetDataCommand(string receivedCommand)
        {
            string command = "C4 01 81 00 ";
            if (receivedCommand == "C0 01 81 00 01 00 00 60 3C 06 FF 02 00")
            {
                string hr = Convert.ToString(initial_communication_time.Hour, 16);
                string min = Convert.ToString(initial_communication_time.Minute, 16);
                string sec = Convert.ToString(initial_communication_time.Second, 16);

                string interval = communication_interval.ToString("X4");

                string comm_time = hr + " " + min + " " + sec;

                command += "02 02 09 03 " + comm_time + " 12 " + interval;

            }
            else if (receivedCommand == "C0 01 81 00 01 00 00 60 3C 12 FF 02 00")
            {
                if (communication_type == 1)
                {
                    command += "11 01";
                }
                else
                {
                    command += "11 00";
                }

            }
            else if (receivedCommand == "C0 01 81 00 01 00 00 5E 42 1E FF 02 00")
            {
                if (bidirectional_device)
                {
                    command += "11 02";
                }
                else
                {
                    command += "11 01";
                }
            }
            else
            {
                command = "";
            }

            return command;
        }


    }
}
