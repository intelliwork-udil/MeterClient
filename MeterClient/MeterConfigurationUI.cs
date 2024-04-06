using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using MeterClient.Helper;

namespace MeterClient
{
    public class MeterConfigurationUI
    {
        //TcpClient client;
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

            //menu.Add("1. Setup Meter Configuration.");
            menu.Add("1. Run Meter Client");
            //menu.Add("3. Exit");

            //meterConfiguration = MeterConfiguration.Instance;
            meterConfiguration = new MeterConfiguration();



            GenerateFolders("MeterConfigs");
            GenerateFolders("MeterSamplingData");

            // Sub folders
            GenerateFolders(Path.Combine("MeterSamplingData", "BillingData"));
            GenerateFolders(Path.Combine("MeterSamplingData", "InstanteneousData"));
            GenerateFolders(Path.Combine("MeterSamplingData", "LPROData"));


        }

        protected static void myHandler(object sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;
            MeterConfigurationUI.cancelled = true;
            Console.WriteLine("CANCEL command received! Cleaning up. please wait...");
        }

        public void GenerateFolders(string folderName)
        {
            if (!Directory.Exists(folderName))
            {
                try
                {
                    Directory.CreateDirectory(folderName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating folder {folderName}: {ex.Message}");
                }
            }
        }


        public async Task Main()
        {
            // Main function

            Console.CancelKeyPress += new ConsoleCancelEventHandler(myHandler);



            //meterConfiguration = meterConfiguration.loadConfiguration();
            //meterConfiguration.ippo.primary_ip_address = "127.0.0.1";

            //meterConfiguration.ippo.primary_port = 32220;
            Console.ForegroundColor = ConsoleColor.Green;

            //Console.WriteLine($"Meter {meterConfiguration.msn} connected on {meterConfiguration.ippo.primary_ip_address}:{meterConfiguration.ippo.primary_port}");
            Console.ResetColor();

            while (true)
            {
                foreach (var item in menu)
                {
                    Console.WriteLine(item);
                }

                string input = Console.ReadLine();

                //if (input == "1")
                //{
                //    SetupMeter();
                //}
                //else
                if (input == "1")
                {
                    await RunMeterClient();
                    //SimulateMeterClientRunningStream();
                    Console.WriteLine("Client Stopped");
                    MeterConfigurationUI.cancelled = false;

                }
                //else if (input == "3")
                //{
                //    break;
                //}
                else
                {
                    Console.WriteLine("Invalid Input");
                }

                //meterConfiguration.saveConfiguration();

            }
            //meterConfiguration.saveConfiguration();
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

            //IPAddress ipAddress = IPAddress.Parse(meterConfiguration.ippo.primary_ip_address);

            //var ipEndPoint = new IPEndPoint(ipAddress, meterConfiguration.ippo.primary_port);

            //client = new();
            //await client.ConnectAsync(ipEndPoint);
            //await using NetworkStream stream = client.GetStream();


            //Console.WriteLine("How many clients do you want to run?");
            //int clientCount = Convert.ToInt32(Console.ReadLine());

            //List<MeterConfiguration> clients = new List<MeterConfiguration>();
            //clients.Add(conf);

            //for (int i = 0; i < clientCount; i++)
            //{
            //    Thread thread = new Thread(() => RunSingleClient());
            //    thread.Start();

            //    //await RunSingleClient();
            //}

            new MeterClientsGenerator().generateClients();

            foreach (var client in MeterClientsGenerator.clients)
            {
                CsvThreadLoggerUtility.Log(client.msn, "Client Initialized");
                Thread thread = new Thread(() => RunSingleClient(client));
                thread.Start();
            }

            //new MeterClientsGenerator().generateClients();

            //foreach (var client in MeterClientsGenerator.clients)
            //{
            //    CsvThreadLoggerUtility.Log(client.msn, "Client Initialized");
            //    await Task.Run(() => RunSingleClient(client));
            //}



        }


        private async Task RunSingleClient(MeterConfiguration conf)
        {
            try
            {
                CsvThreadLoggerUtility.Log(conf.msn, "Thread Started");

                conf = conf.loadConfiguration("MeterConfigs/" + conf.msn + ".json");

                await conf.mdsm.GenerateSamplingData(conf);

                IPAddress ipAddress = IPAddress.Parse(conf.ippo.primary_ip_address);

                var ipEndPoint = new IPEndPoint(ipAddress, conf.ippo.primary_port);

                TcpClient client = new();
                await client.ConnectAsync(ipEndPoint);
                NetworkStream stream = client.GetStream();

                await MeterClientRunningStream(stream, conf, client);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }


        private async Task MeterClientRunningStream(NetworkStream stream, MeterConfiguration conf, TcpClient client)
        {
            bool isLogged = false;



            do
            {

                //startConnection:



                if (MeterConfigurationUI.cancelled) return;
                string msn = ConvertToHex(conf.msn);
                string heartbeat = "DD 04 " + msn;

                if (!client.Connected)
                {
                    IPAddress ipAddress = IPAddress.Parse(conf.ippo.primary_ip_address);

                    var ipEndPoint = new IPEndPoint(ipAddress, conf.ippo.primary_port);
                    client = new();
                    await client.ConnectAsync(ipEndPoint);
                    stream = client.GetStream();
                }

                SendCommand(stream, heartbeat, conf, true);
                if (isLogged == false)
                {
                    CsvThreadLoggerUtility.Log1(conf.msn, heartbeat);
                }


                string re = ReadCommand(stream, conf, true);

                if (isLogged == false)
                {
                    CsvThreadLoggerUtility.Log1(conf.msn, re);
                }
                isLogged = true;


                if (re == "DA")
                {
                    while (true)
                    {
                        if ((DateTime.Now - lastCommTime).TotalSeconds > 180)
                            break;
                        if (MeterConfigurationUI.cancelled) return;


                        try
                        {
                            re = "";
                            do
                            {

                                re = ReadCommand(stream, conf, false);
                                if (MeterConfigurationUI.cancelled) return;
                                if (String.IsNullOrEmpty(re))
                                {
                                    //if (conf.dmdt.communication_type == 1)
                                    //{
                                    //    client.Close();
                                    //    return;
                                    //    //Thread.Sleep(2 * 1000);
                                    //    //goto startConnection;
                                    //}
                                    //else
                                    //{
                                    //    Thread.Sleep(5000);
                                    //}
                                }


                                if ((DateTime.Now - lastCommTime).TotalSeconds > 180)
                                {
                                    SendCommand(stream, heartbeat, conf, false);
                                }

                            }
                            while (String.IsNullOrEmpty(re) && (DateTime.Now - lastCommTime).TotalSeconds < 180);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Connection is inactive");
                        }

                        ProcessCommand(stream, conf, re);

                        conf.saveConfiguration("MeterConfigs/" + conf.msn + ".json");
                    }
                }
                else
                {
                    ProcessCommand(stream, conf, re);


                }

            }
            while (!MeterConfigurationUI.cancelled);
            return;
        }

        private void ProcessCommand(NetworkStream stream, MeterConfiguration conf, string re)
        {
            CommandType commandType = CommandClassifier.commandType(re);


            Console.WriteLine($"Command Received for Meter: {conf.msn} - {commandType.ToString()}");

            string sendCmd = "";

            switch (commandType)
            {
                case CommandType.MSIM_READ:
                    sendCmd = "C4 01 81 00 09 14 38 39 39 32 33 30 30 30 30 30 34 35 33 31 34 31 39 37 37 46";
                    var cmdArr = sendCmd.Split(' ');
                    int count = cmdArr.Length;
                    string finalCommand = "00 01 00 30 00 01 00 " + Convert.ToString(count, 16).PadLeft(2, '0') + " " + sendCmd;
                    sendCmd = finalCommand;
                    break;
                case CommandType.IMEI_READ:
                    sendCmd = "C4 01 81 00 09 0F 38 36 31 33 36 35 30 34 33 31 32 38 34 31 37";
                    cmdArr = sendCmd.Split(' ');
                    count = cmdArr.Length;
                    finalCommand = "00 01 00 30 00 01 00 " + Convert.ToString(count, 16).PadLeft(2, '0') + " " + sendCmd;
                    sendCmd = finalCommand;
                    break;
                case CommandType.AARQ:
                    if (PasswordChecker(re, conf))
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
                    conf.dmdt.PerformCommand(re);
                    sendCmd = "C5 01 81 00";
                    cmdArr = sendCmd.Split(' ');
                    count = cmdArr.Length;
                    finalCommand = "00 01 00 30 00 01 00 " + Convert.ToString(count, 16).PadLeft(2, '0') + " " + sendCmd;
                    sendCmd = finalCommand;
                    break;
                case CommandType.DMDT:
                    sendCmd = conf.dmdt.GetDataCommand(re);
                    if (sendCmd != "")
                    {
                        var cmdArr2 = sendCmd.Split(' ');
                        int count2 = cmdArr2.Length;
                        string finalCommand2 = "00 01 00 30 00 01 00 " + Convert.ToString(count2, 16).PadLeft(2, '0') + " " + sendCmd;
                        sendCmd = finalCommand2;
                    }
                    break;
                case CommandType.MDSM:
                    sendCmd = conf.mdsm.ProcessCommand(re);
                    if (sendCmd != "")
                    {
                        var cmdArr3 = sendCmd.Split(' ');
                        int count3 = cmdArr3.Length;
                        string finalCommand3 = "00 01 00 30 00 01 00 " + Convert.ToString(count3, 16).PadLeft(2, '0') + " " + sendCmd;
                        sendCmd = finalCommand3;
                    }
                    break;
                case CommandType.AUXR:
                    sendCmd = conf.auxr.ProcessCommand(re);
                    if (sendCmd != "")
                    {
                        var cmdArr3 = sendCmd.Split(' ');
                        int count3 = cmdArr3.Length;
                        string finalCommand3 = "00 01 00 30 00 01 00 " + Convert.ToString(count3, 16).PadLeft(2, '0') + " " + sendCmd;
                        sendCmd = finalCommand3;
                    }
                    break;
                case CommandType.WSIM:
                    sendCmd = conf.wsim.ProcessCommand(re);
                    if (sendCmd != "")
                    {
                        var cmdArr3 = sendCmd.Split(' ');
                        int count3 = cmdArr3.Length;
                        string finalCommand3 = "00 01 00 30 00 01 00 " + Convert.ToString(count3, 16).PadLeft(2, '0') + " " + sendCmd;
                        sendCmd = finalCommand3;
                    }
                    break;
                case CommandType.IPPO:
                    sendCmd = conf.ippo.ProcessCommand(re);
                    if (sendCmd != "")
                    {
                        var cmdArr3 = sendCmd.Split(' ');
                        int count3 = cmdArr3.Length;
                        string finalCommand3 = "00 01 00 30 00 01 00 " + Convert.ToString(count3, 16).PadLeft(2, '0') + " " + sendCmd;
                        sendCmd = finalCommand3;
                    }
                    break;
                case CommandType.OPPO:
                    sendCmd = conf.oppo.ProcessCommand(re);
                    if (sendCmd != "")
                    {
                        var cmdArr3 = sendCmd.Split(' ');
                        int count3 = cmdArr3.Length;
                        string finalCommand3 = "00 01 00 30 00 01 00 " + Convert.ToString(count3, 16).PadLeft(2, '0') + " " + sendCmd;
                        sendCmd = finalCommand3;
                    }
                    break;
                case CommandType.TIOU:
                    sendCmd = conf.tiou.ProcessCommand(re);
                    if (sendCmd != "")
                    {
                        var cmdArr3 = sendCmd.Split(' ');
                        int count3 = cmdArr3.Length;
                        string finalCommand3 = "00 01 00 30 00 01 00 " + Convert.ToString(count3, 16).PadLeft(2, '0') + " " + sendCmd;
                        sendCmd = finalCommand3;
                    }
                    break;
                case CommandType.MDI:
                    sendCmd = conf.mdi.ProcessCommand(re);
                    if (sendCmd != "")
                    {
                        var cmdArr3 = sendCmd.Split(' ');
                        int count3 = cmdArr3.Length;
                        string finalCommand3 = "00 01 00 30 00 01 00 " + Convert.ToString(count3, 16).PadLeft(2, '0') + " " + sendCmd;
                        sendCmd = finalCommand3;
                    }
                    break;
                case CommandType.TIME_SYNCHRONIZATION:
                    sendCmd = conf.dvtm.ProcessCommand(re);
                    if (sendCmd != "")
                    {
                        var cmdArr3 = sendCmd.Split(' ');
                        int count3 = cmdArr3.Length;
                        string finalCommand3 = "00 01 00 30 00 01 00 " + Convert.ToString(count3, 16).PadLeft(2, '0') + " " + sendCmd;
                        sendCmd = finalCommand3;
                    }
                    break;
                //case CommandType.SANCTIONED_LOAD_CONTROL:
                //    sendCmd = conf.sanc.ProcessCommand(re);
                //    if (sendCmd != "")
                //    {
                //        var cmdArr3 = sendCmd.Split(' ');
                //        int count3 = cmdArr3.Length;
                //        string finalCommand3 = "00 01 00 30 00 01 00 " + Convert.ToString(count3, 16).PadLeft(2, '0') + " " + sendCmd;
                //        sendCmd = finalCommand3;
                //    }
                //    break;
                case CommandType.LOAD_SHEDDING_SCHEDULLING:
                    sendCmd = conf.lsch.ProcessCommand(re);
                    if (sendCmd != "")
                    {
                        var cmdArr3 = sendCmd.Split(' ');
                        int count3 = cmdArr3.Length;
                        string finalCommand3 = "00 01 00 30 00 01 00 " + Convert.ToString(count3, 16).PadLeft(2, '0') + " " + sendCmd;
                        sendCmd = finalCommand3;
                    }
                    break;
                case CommandType.INST_DATA_READ:
                    Logger.Instance.Log(conf.msn, "INST Data Read", "");
                    re = conf.mdsm.ProcessCommandForInstantaneousData(re, stream, conf);
                    if (re != "")
                    {
                        ProcessCommand(stream, conf, re);
                    }
                    sendCmd = "";
                    break;
                case CommandType.BILL_DATA_READ:
                    re = conf.mdsm.ProcessCommandForBillingData(re, stream, conf);
                    if (re != "")
                    {
                        ProcessCommand(stream, conf, re);
                    }
                    sendCmd = "";
                    break;
                case CommandType.LPRO_DATA_READ:
                    re = conf.mdsm.ProcessCommandForLPROData(re, stream, conf);
                    if (re != "")
                    {
                        ProcessCommand(stream, conf, re);
                    }
                    sendCmd = "";
                    break;
                case CommandType.Nothing:
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
                SendCommand(stream, sendCmd, conf, false);
            }
            else
            {
                SendCommand(stream, error, conf, false);
            }
        }

        public static string ReadCommand(NetworkStream stream, MeterConfiguration conf, bool needsLogg = false)
        {
            string re = "";
            try
            {
                var data = new byte[1024];

                stream.ReadTimeout = 200000;

                int count = stream.Read(data, 0, data.Length);

                // Convert Data to Hex String
                re = Convert.ToHexString(data, 0, count);
                re = re.Replace(" ", "");
                re = MeterConfigurationUI.AddSpaceEveryNCharacters(re, 2);
                if (count != 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"MDC: For meter: {conf.msn} {re}");
                    Console.ResetColor();
                    lastCommTime = DateTime.Now;

                    //if (needsLogg)
                    //{
                    //    Logger.Instance.Log(conf.msn, "Receive", re);
                    //}
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Logger.Instance.Log(conf.msn, "Error", ex.Message);
                return re;
            }
            return re;
        }

        public static void SendCommand(NetworkStream stream, string commandStr, MeterConfiguration conf, bool needsLogg = false)
        {
            try
            {
                commandStr = commandStr.Replace(" ", "");

                commandStr = AddSpaceEveryNCharacters(commandStr, 2);

                byte[] command = commandStr.Split()
                .Select(s => Convert.ToByte(s, 16)).ToArray();

                stream.Write(command, 0, command.Length);

                if (needsLogg)
                {
                    Logger.Instance.Log(conf.msn, "Send", commandStr);
                }

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"Meter {conf.msn}: " + commandStr);
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public bool PasswordChecker(string aarq, MeterConfiguration conf)
        {
            string s = "00 01 00 30 00 01 00 38 60 36 A1 09 06 07 60 85 74 05 08 01 01 8A 02 07 80 8B 07 60 85 74 05 08 02 01 AC 0A 80 08 ";

            int endIndex = aarq.IndexOf(" BE 10 04 0E 01 00 00 00 06 5F 1F 04 00 00 7E 1F 04 B0");

            string password = aarq.Substring(s.Length, endIndex - s.Length).Replace(" ", "");

            //string meterPassword = ConvertToX4ByteString(meterConfiguration.password).Replace(" ", "");

            string meterPassword = string.Concat(conf.password.Select(c => "3" + c));


            Console.WriteLine($"{meterPassword} {password}");

            return (password == meterPassword);
            //return true;
        }



        public static string AddSpaceEveryNCharacters(string input, int n)
        {
            if (n <= 0)
            {
                throw new ArgumentException("Invalid value for N");
            }

            input = input.Replace(" ", "");

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
