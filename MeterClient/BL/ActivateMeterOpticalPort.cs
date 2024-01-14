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

        public string ProcessCommand(string re)
        {
            string command = "";

            if (re.Contains("C0 01 81 00 01 00 00 60 3C 10 FF 02 00"))
            {
                string onyear = optical_port_on_datetime.Year.ToString("X").PadLeft(4, '0');
                string onmonth = optical_port_on_datetime.Month.ToString("X").PadLeft(2, '0');
                string onday = optical_port_on_datetime.Day.ToString("X").PadLeft(2, '0');

                string onhour = optical_port_on_datetime.Hour.ToString("X").PadLeft(2, '0');
                string onminute = optical_port_on_datetime.Minute.ToString("X").PadLeft(2, '0');
                string onsecond = optical_port_on_datetime.Second.ToString("X").PadLeft(2, '0');

                string offyear = optical_port_off_datetime.Year.ToString("X").PadLeft(4, '0');
                string offmonth = optical_port_off_datetime.Month.ToString("X").PadLeft(2, '0');
                string offday = optical_port_off_datetime.Day.ToString("X").PadLeft(2, '0');

                string offhour = optical_port_off_datetime.Hour.ToString("X").PadLeft(2, '0');
                string offminute = optical_port_off_datetime.Minute.ToString("X").PadLeft(2, '0');
                string offsecond = optical_port_off_datetime.Second.ToString("X").PadLeft(2, '0');

                onyear = MeterConfigurationUI.AddSpaceEveryNCharacters(onyear, 2);
                onmonth = MeterConfigurationUI.AddSpaceEveryNCharacters(onmonth, 2);
                onday = MeterConfigurationUI.AddSpaceEveryNCharacters(onday, 2);

                onhour = MeterConfigurationUI.AddSpaceEveryNCharacters(onhour, 2);
                onminute = MeterConfigurationUI.AddSpaceEveryNCharacters(onminute, 2);
                onsecond = MeterConfigurationUI.AddSpaceEveryNCharacters(onsecond, 2);

                offyear = MeterConfigurationUI.AddSpaceEveryNCharacters(offyear, 2);
                offmonth = MeterConfigurationUI.AddSpaceEveryNCharacters(offmonth, 2);
                offday = MeterConfigurationUI.AddSpaceEveryNCharacters(offday, 2);

                offhour = MeterConfigurationUI.AddSpaceEveryNCharacters(offhour, 2);
                offminute = MeterConfigurationUI.AddSpaceEveryNCharacters(offminute, 2);
                offsecond = MeterConfigurationUI.AddSpaceEveryNCharacters(offsecond, 2);

                string ondate = onyear + " " + onmonth + " " + onday;
                string ontime = onhour + " " + onminute + " " + onsecond;

                string offdate = offyear + " " + offmonth + " " + offday;
                string offtime = offhour + " " + offminute + " " + offsecond;

                command = "C4 01 81 00 02 02 09 0C " + ondate + " 06 " + ontime + " 00 80 00 FF 09 0C " + offdate + " 03 " + offtime + " 00 80 00 FF";
            }

            return command;
        }
    }
}
