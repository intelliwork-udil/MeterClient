using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace MeterClient
{
    public class MeterConfigurationUI
    {
        TcpClient client;
        public static DateTime lastCommTime = DateTime.Now.AddSeconds(-20);
        // cancelled: Used for determining if a cancel has been requested
        private static volatile bool cancelled = false;

        private List<string> menu = new List<string>();

        private static MeterConfiguration meterConfiguration;

        public int timeout { get; set; } = 10000;

        private string aare = "00 01 00 01 00 30 00 2B 61 29 A1 09 06 07 60 85 74 05 08 01 01 A2 03 02 01 00 A3 05 A1 03 02 01 00 BE 10 04 0E 08 00 06 5F 1F 04 00 00 1C 1F 01 2C 00 07";
        private string error = "00 01 00 01 00 30 00 03 D8 01 01";

        private string initialCommand = "00 01 00 30 00 01 00";

        public MeterConfigurationUI()
        {

            menu.Add("1. Setup Meter Configuration.");
            menu.Add("2. Run Meter Client");
            menu.Add("3. Exit");

            meterConfiguration = MeterConfiguration.Instance;

        
        }

          protected static void myHandler(object sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;
            MeterConfigurationUI.cancelled = true;
            Console.WriteLine("CANCEL command received! Cleaning up. please wait...");
        }

        public async Task Main()
        {
            // Main function

            Console.CancelKeyPress += new ConsoleCancelEventHandler(myHandler);
        

      
        meterConfiguration = meterConfiguration.loadConfiguration();
            meterConfiguration.ippo.primary_ip_address = "127.0.0.1";

            meterConfiguration.ippo.primary_port = 32220;
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"Meter {meterConfiguration.msn} connected on {meterConfiguration.ippo.primary_ip_address}:{meterConfiguration.ippo.primary_port}");
            Console.ResetColor();

            while (true)
            {
                foreach (var item in menu)
                {
                    Console.WriteLine(item);
                }

                string input = Console.ReadLine();

                if (input == "1")
                {
                    SetupMeter();
                }
                else if (input == "2")
                {
                    await RunMeterClient();
                    //SimulateMeterClientRunningStream();
                    Console.WriteLine("Client Stopped");
                    MeterConfigurationUI.cancelled = false;

                }
                else if (input == "3")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid Input");
                }

                meterConfiguration.saveConfiguration();

            }
            meterConfiguration.saveConfiguration();
        }


        public void SetupMeter()
        {

            Console.WriteLine("------------------------------------Setup Your Meter------------------------------------");

            Console.WriteLine("Enter Meter Serial Number:");
            meterConfiguration.msn = Console.ReadLine();

            Console.WriteLine("Enter Meter Password:");
            meterConfiguration.password = Console.ReadLine();

            Console.WriteLine("Enter Primary IP Address:");
            meterConfiguration.ippo.primary_ip_address = Console.ReadLine();

            Console.WriteLine("Enter Primary Port:");
            meterConfiguration.ippo.primary_port = Console.Read();
        }





        public async Task RunMeterClient()
        {

            IPAddress ipAddress = IPAddress.Parse(meterConfiguration.ippo.primary_ip_address);

            var ipEndPoint = new IPEndPoint(ipAddress, meterConfiguration.ippo.primary_port);

             client = new();
            await client.ConnectAsync(ipEndPoint);
            await using NetworkStream stream = client.GetStream();

   

      await MeterClientRunningStream(stream);

        }

        private async Task  MeterClientRunningStream(NetworkStream stream)
        {
          
          
          

            do
            {
                if (MeterConfigurationUI.cancelled) return;
                string msn = ConvertToHex(meterConfiguration.msn);
                string heartbeat = "DD 04 " + msn;

                if(!client.Connected)
                {
                    IPAddress ipAddress = IPAddress.Parse(meterConfiguration.ippo.primary_ip_address);

                    var ipEndPoint = new IPEndPoint(ipAddress, meterConfiguration.ippo.primary_port);
                 client = new();
                    await client.ConnectAsync(ipEndPoint);
                stream =  client.GetStream();
                }

                SendCommand(stream, heartbeat);


                string re = ReadCommand(stream);

                if (re == "DA")
                {
                    //Console.ForegroundColor = ConsoleColor.Green;
                    //Console.WriteLine("Meter is Online (Press any Key To Quit)");
                    //Console.ResetColor();

                    while (true)
                    {
                        if ((DateTime.Now - lastCommTime).TotalSeconds > 180000)
                            break;
                        if (MeterConfigurationUI.cancelled) return;
                        //if (Console.KeyAvailable)
                        //{
                        //    Console.ForegroundColor = ConsoleColor.Red;
                        //    Console.WriteLine("Stopping the Meter Client.");
                        //    Console.ResetColor();
                        //    break;
                        //}

                        try
                        {
                            re = "";
                            do
                            {
                                
                                re = ReadCommand(stream);
                                if (MeterConfigurationUI.cancelled) return ;
                                if(String.IsNullOrEmpty(re))
                                      Thread.Sleep(5000);

                            }
                            while (String.IsNullOrEmpty(re) && (DateTime.Now - lastCommTime).TotalSeconds < 180);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Connection is inactive");
                        }
                        // Print Data
                        //Console.WriteLine($"Message received: \"{re}\"");

                        // Determine Command Type
                        CommandType commandType = CommandClassifier.commandType(re);

                        string sendCmd = "";

                        switch (commandType)
                        {
                            case CommandType.AARQ:
                                if (PasswordChecker(re))
                                {
                                    sendCmd = aare;
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("Password Correct");
                                    Console.ResetColor();
                                }
                                else
                                {
                                    sendCmd = error;
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Password Incorrect");
                                    Console.ResetColor();
                                }
                                break;
                            case CommandType.DeviceCreation:
                                meterConfiguration.dmdt.PerformCommand(re);
                                sendCmd = "C5 01 81 00";
                                var cmdArr = sendCmd.Split(' ');
                                int count = cmdArr.Length;
                                string finalCommand = "00 01 00 30 00 01 00 " + Convert.ToString(count, 16) + " " + sendCmd;
                                sendCmd = finalCommand;
                                break;
                            case CommandType.DMDT:
                                sendCmd = meterConfiguration.dmdt.GetDataCommand(re);
                                var cmdArr2 = sendCmd.Split(' ');
                                int count2 = cmdArr2.Length;
                                string finalCommand2 = "00 01 00 30 00 01 00 " + Convert.ToString(count2, 16) + " " + sendCmd;
                                sendCmd = finalCommand2;
                                break;
                            case CommandType.Nothing:
                               // Console.ForegroundColor = ConsoleColor.Red;
                                //Console.WriteLine("In-Valid Command");
                                //Console.ResetColor();
                                sendCmd = error;

                                break;
                            default:
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("In-Valid Command");
                                Console.ResetColor();
                                sendCmd = error;
                                break;
                        }

                        if (sendCmd != "")
                        {
                            SendCommand(stream, sendCmd);
                        }
                        sendCmd = "";
                        re = "";
                    }
                }
                else
                {



                }
            }
            while (!MeterConfigurationUI.cancelled);
            return;
        }


        //private void SimulateMeterClientRunningStream()
        //{
        //    if (MeterConfigurationUI.cancelled) return;
        //    string msn = ConvertToHex(meterConfiguration.msn);
        //    string heartbeat = "DD 04 " + msn;

        //    Console.WriteLine(heartbeat);

        //    string re = Console.ReadLine();

        //    Console.ForegroundColor = ConsoleColor.Green;
        //    Console.WriteLine($"Message received: \"{re}\"");
        //    Console.ResetColor();
           

        //    if (re == "DA")
        //    {
        //        Console.ForegroundColor = ConsoleColor.Green;
        //        Console.WriteLine("Meter is Online (Press any Key To Quit)");
        //        Console.ResetColor();

        //        while (true)
        //        {
        //            if (MeterConfigurationUI.cancelled) return;
        //            //if (Console.KeyAvailable)
        //            //{
        //            //    Console.ForegroundColor = ConsoleColor.Red;
        //            //    Console.WriteLine("Stopping the Meter Client.");
        //            //    Console.ResetColor();
        //            //    break;
        //            //}

        //         //   re = Console.ReadLine();

        //            // Print Data
        //           // Console.WriteLine($"Message received: \"{re}\"");

        //            // Determine Command Type
        //            CommandType commandType = CommandClassifier.commandType(re);

        //            string sendCmd = "";


        //            switch (commandType)
        //            {
        //                case CommandType.AARQ:
        //                    if (PasswordChecker(re))
        //                    {
        //                        sendCmd = aare;
        //                        Console.ForegroundColor = ConsoleColor.Green;
        //                        Console.WriteLine("Password Correct");
        //                        Console.ResetColor();
        //                    }
        //                    else
        //                    {
        //                        sendCmd = error;
        //                        Console.ForegroundColor = ConsoleColor.Red;
        //                        Console.WriteLine("Password Incorrect");
        //                        Console.ResetColor();
        //                    }
        //                    break;
        //                case CommandType.DeviceCreation:
        //                    meterConfiguration.dmdt.PerformCommand(re);
        //                    sendCmd = "C5 01 81 00";
        //                    var cmdArr = sendCmd.Split(' ');
        //                    int count = cmdArr.Length;
        //                    string finalCommand = "00 01 00 30 00 01 00 " + Convert.ToString(count, 16) + " " + sendCmd;
        //                    sendCmd = finalCommand;
        //                    break;
        //                case CommandType.DMDT:
        //                    sendCmd = meterConfiguration.dmdt.GetDataCommand(re);
        //                    var cmdArr2 = sendCmd.Split(' ');
        //                    int count2 = cmdArr2.Length;
        //                    string finalCommand2 = "00 01 00 30 00 01 00 " + Convert.ToString(count2, 16) + " " + sendCmd;
        //                    sendCmd = finalCommand2;
        //                    break;
        //                case CommandType.Nothing:
        //                    break;
        //                default:
        //                    Console.ForegroundColor = ConsoleColor.Red;
        //                    Console.WriteLine("In-Valid Command");
        //                    Console.ResetColor();
        //                    sendCmd = error;
        //                    break;
        //            }

        //            if (sendCmd != "")
        //            {
        //                Console.WriteLine(sendCmd);
        //            }

        //            re = "";
        //        }
        //    }
        //    else
        //    {
        //        Thread.Sleep(3000);
        //        SimulateMeterClientRunningStream();
        //    }
        //}


        public string ReadCommand(NetworkStream stream)
        {
            string re;
            try
            {
                var data = new byte[1024];

                stream.ReadTimeout = 20000;

                int count = stream.Read(data, 0, data.Length);

                // Convert Data to Hex String
                 re = Convert.ToHexString(data, 0, count);
                re = re.Replace(" ", "");
                re = AddSpaceEveryNCharacters(re, 2);
                if (count != 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"MDC: {re}");
                    Console.ResetColor();
                    lastCommTime = DateTime.Now;
                }
                
                return re;
            }catch(Exception ex)
            {
                return null;
            }
            }

        public void SendCommand(NetworkStream stream, string commandStr)
        {
            byte[] command = commandStr.Split()
            .Select(s => Convert.ToByte(s, 16)).ToArray();

            stream.Write(command, 0, command.Length);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Meter: " + commandStr);
            Console.ResetColor();
        }

        public bool PasswordChecker(string aarq)
        {
            string s = "00 01 00 30 00 01 00 38 60 36 A1 09 06 07 60 85 74 05 08 01 01 8A 02 07 80 8B 07 60 85 74 05 08 02 01 AC 0A 80 08 ";

            int endIndex = aarq.IndexOf(" BE 10 04 0E 01 00 00 00 06 5F 1F 04 00 00 7E 1F 04 B0");

            string password = aarq.Substring(s.Length, endIndex - s.Length).Replace(" ", "");

            string meterPassword = ConvertToX4ByteString(meterConfiguration.password).Replace(" ", "");

            Console.WriteLine($"{meterPassword} {password}");

            //  return (password == meterPassword);
            return true;
        }



        public string AddSpaceEveryNCharacters(string input, int n)
        {
            if (n <= 0)
            {
                throw new ArgumentException("Invalid value for N");
            }

            int length = input.Length;
            int numSpaces = (length - 1) / n;

            char[] spacedChars = new char[length + numSpaces];
            int index = 0;

            for (int i = 0; i < length; i++)
            {
                spacedChars[index++] = input[i];

                if ((i + 1) % n == 0 && i < length - 1)
                {
                    spacedChars[index++] = ' ';
                }
            }

            return new string(spacedChars);
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
    }
}
