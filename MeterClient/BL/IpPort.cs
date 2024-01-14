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

        public string ProcessCommand(string re)
        {
            string command = "";

            if (re.Contains("C0 01 81 00 01 00 00 19 04 80 FF 02 00"))
            {
                string pPortHex = primary_port.ToString("X").PadLeft(4, '0');
                string sPortHex = secondary_port.ToString("X").PadLeft(4, '0');

                //127.0.0.1
                string pIpHex = parseIpAddress(primary_ip_address).PadLeft(8, '0');
                string sIpHex = parseIpAddress(secondary_ip_address).PadLeft(8, '0');


                pPortHex = MeterConfigurationUI.AddSpaceEveryNCharacters(pPortHex, 2);
                sPortHex = MeterConfigurationUI.AddSpaceEveryNCharacters(sPortHex, 2);
                pIpHex = MeterConfigurationUI.AddSpaceEveryNCharacters(pIpHex, 2);
                sIpHex = MeterConfigurationUI.AddSpaceEveryNCharacters(sIpHex, 2);




                command = "C4 01 81 00 02 15 11 00 04 01 02 09 04 " + pIpHex + " 09 02 " + pPortHex + " 09 04 " + sIpHex + " 09 02 " + sPortHex + " 09 04 FF FF FF FF 09 02 FF FF 09 04 FF FF FF FF 09 02 FF FF 0A 01 30 09 04 FF FF FF FF 09 04 FF FF FF FF 0A 05 43 4D 4E 45 54 0A 01 30 0A 04 6E 75 6C 6C 0A 04 6E 75 6C 6C 11 0A 11 01 11 02 11 00";
            }

            return command;
        }

        public string parseIpAddress(string ip)
        {
            if (ip == "")
            {
                ip = "0.0.0.0";
            }

            string[] ipSplit = ip.Split('.');

            string ipHex = "";

            foreach (string s in ipSplit)
            {
                ipHex += Convert.ToInt32(s).ToString("X").PadLeft(2, '0');
            }

            return ipHex;
        }
    }
}
