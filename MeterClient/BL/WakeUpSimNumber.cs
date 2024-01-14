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

        public string ProcessCommand(string re)
        {
            string command = "C4 01 81 00 09 0B ";

            string sim = "";

            if (re == "00 01 00 30 00 01 00 0D C0 01 81 00 01 00 00 60 0C 80 FF 02 00")
            {
                sim = wakeup_number_1;
            }
            else if (re == "00 01 00 30 00 01 00 0D C0 01 81 00 01 00 00 60 0C 81 FF 02 00")
            {
                sim = wakeup_number_2;
            }
            else if (re == "00 01 00 30 00 01 00 0D C0 01 81 00 01 00 00 60 0C 82 FF 02 00")
            {
                sim = wakeup_number_3;
            }

            sim = string.Concat(sim.Select(c => "3" + c));

            sim = MeterConfigurationUI.AddSpaceEveryNCharacters(sim, 2);

            command += sim;

            return command;
        }
    }
}
