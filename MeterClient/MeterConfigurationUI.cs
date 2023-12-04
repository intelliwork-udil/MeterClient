using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace MeterClient
{
    public class MeterConfigurationUI
    {

        private List<string> menu = new List<string>();

        private static MeterConfiguration meterConfiguration;

        public string hostName { get; set; }
        public int port { get; set; }

        public MeterConfigurationUI()
        {
            menu.Add("1. Setup Meter Configuration.");
            menu.Add("2. Load Meter Configuration.");
            menu.Add("3. Save Meter Configuration.");
            menu.Add("4. Run Meter Client");
            menu.Add("5. Exit");

            meterConfiguration = MeterConfiguration.Instance;

        }

        public async Task Main()
        {
            string op = "";
            while (true)
            {
                foreach (var item in menu)
                {
                    Console.WriteLine(item);
                }

                string input = Console.ReadLine();

                if (input == "1")
                {
                    FirstMenu();
                }
                else if (input == "2")
                {
                    meterConfiguration = meterConfiguration.loadConfiguration("C:\\Users\\Umair\\Desktop\\1.json");
                }
                else if (input == "3")
                {
                    meterConfiguration.saveConfiguration("C:\\Users\\Umair\\Desktop\\1.json");
                }
                else if (input == "4")
                {
                    TakeIpAddress();
                    await SecondMenu();
                }
                else if (input == "5")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid Input");
                }


            }
        }


        public void FirstMenu()
        {
            Console.WriteLine("Enter Meter Serial Number:");
            meterConfiguration.msn = Console.ReadLine();
            Console.WriteLine("Enter Meter Password:");
            meterConfiguration.password = Console.ReadLine();
        }

        public void TakeIpAddress()
        {
            Console.WriteLine("Enter Meter IP Address:");
            hostName = Console.ReadLine();
            Console.WriteLine("Enter Meter Port:");
            port = Convert.ToInt32(Console.ReadLine());

        }

        public string ConvertToHex(string input)
        {
            string output = "";

            string hexMSN = (long.Parse(input)).ToString("X");
            string spacedHexString = string.Join(" ", Enumerable.Range(0, hexMSN.Length / 2).Select(i => hexMSN.Substring(i * 2, 2)));

            string reversedHexString = string.Concat(Enumerable.Range(0, hexMSN.Length / 2)
                                                     .Select(i => hexMSN.Substring(i * 2, 2))
                                                     .Reverse());


            string stringWithSpaces = string.Concat(Enumerable.Range(0, reversedHexString.Length / 2).Select(i => reversedHexString.Substring(i * 2, 2) + " "));

            output = stringWithSpaces.Trim();

            return output;
        }


        public string ConvertToX4ByteString(string value)
        {
            int val = int.Parse(value);
            var temp = val.ToString("X4");
            var hexString = temp[0] + "" + temp[1] + " " + temp[2] + "" + temp[3];
            return hexString;
        }


        public async Task SecondMenu()
        {
            string msn = ConvertToHex(meterConfiguration.msn);

            //Console.WriteLine($"MSN: {msn}");
            //Console.WriteLine($"Password: {ConvertToHex(meterConfiguration.password)}");

            byte[] heartbeat = ("DD 04 " + msn).Split()
            .Select(s => Convert.ToByte(s, 16)).ToArray();
            var data = new byte[1024];

            IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync(hostName);
            IPAddress ipAddress = ipHostInfo.AddressList[0];

            var ipEndPoint = new IPEndPoint(ipAddress, port);

            using TcpClient client = new();
            await client.ConnectAsync(ipEndPoint);
            await using NetworkStream stream = client.GetStream();

            stream.Write(heartbeat, 0, heartbeat.Length);
            //DebugData(heartbeat, heartbeat.Length);
            int count = stream.Read(data, 0, data.Length);

            //Console.WriteLine(data);

            string re = Convert.ToHexString(data, 0, count);

            //var message = Encoding.UTF8.GetString(heartbeat, 0, data);
            Console.WriteLine($"Message received: \"{re}\"");

            if (re == "DA")
            {
                stream.ReadTimeout = 90000;

                count = stream.Read(data, 0, data.Length);

                re = Convert.ToHexString(data, 0, count);

                Console.WriteLine($"Message received: \"{re}\"");


                // Check for password Correction
                string startString = "00010030000100386036A1090607608574050801018A0207808B0760857405080201AC0A8008";
                int startIndex = re.IndexOf(startString);
                int endIndex = re.IndexOf("BE10040E01000000065F1F0400007E1F04B0", startIndex + startString.Length);

                string password = re.Substring(startIndex + startString.Length, endIndex - (startIndex + startString.Length));

                string meterPassword = ConvertToX4ByteString(meterConfiguration.password).Replace(" ", "");

                //Console.WriteLine($"Password: {password}");
                //Console.WriteLine($"Meter Password: {ConvertToHex(meterConfiguration.password)}");

                if (password == meterPassword)
                {
                    Console.WriteLine("Password Correct");

                    byte[] are = "00 01 00 01 00 30 00 2B 61 29 A1 09 06 07 60 85 74 05 08 01 01 A2 03 02 01 00 A3 05 A1 03 02 01 00 BE 10 04 0E 08 00 06 5F 1F 04 00 00 1C 1F 01 2C 00 07".Split()
                        .Select(s => Convert.ToByte(s, 16)).ToArray();

                    stream.Write(are, 0, are.Length);
                }
                else
                {
                    Console.WriteLine("Password Incorrect");
                }



            }
            else
            {
                Thread.Sleep(30000);
                await SecondMenu();
            }








            Console.ReadKey();
        }
    }
}
