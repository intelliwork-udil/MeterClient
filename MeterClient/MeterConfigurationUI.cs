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
using System.Threading;
using System.Diagnostics;

namespace MeterClient
{
    /// <summary>
    /// Provides a console-based user interface for configuring and running the meter simulator clients.
    /// Manages network streams, command processing, and concurrent client execution.
    /// </summary>
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
        private static string error = "00 01 00 01 00 30 00 03 D8 01 01";

        private string initialCommand = "00 01 00 01 00 30 00";


        public static int NeedsConnecting = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeterConfigurationUI"/> class.
        /// Sets up initial menu options and ensures required directories exist.
        /// </summary>
        public MeterConfigurationUI()
        {

            
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
            GenerateFolders(Path.Combine("MeterSamplingData", "EventData"));


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


        /// <summary>
        /// The main entry point for the UI logic. Displays the menu and handles user input.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Main()
        {
            // Main function

            Console.CancelKeyPress += new ConsoleCancelEventHandler(myHandler);


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



        private static SemaphoreSlim semaphore = new SemaphoreSlim(0);

        int clientsInitialized = 0;

        /// <summary>
        /// Triggers the generation of meter clients and starts their execution concurrently.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RunMeterClient()
        {

            new MeterClientsGenerator().generateClients();


            int clientsCount = MeterClientsGenerator.clients.Count;

            //Task[] tasks = new Task[clientsCount];

            //for (int i = 0; i < clientsCount; i++)
            //{
            //    int progress = (i * 100) / clientsCount;

            //    tasks[i] = Task.Run(() => RunSingleClient(MeterClientsGenerator.clients[i]));

            //    clientsInitialized++;
            //    ProgressBarHelper.DrawTextProgressBar(progress, i, clientsCount);
            //}

            //Console.WriteLine("All Tasks Loaded! Press any key to Continue");
            //Console.ReadKey();

            //semaphore.Release(clientsCount);

            //await Task.WhenAll(tasks);

            foreach (var client in MeterClientsGenerator.clients)
            {
                CsvThreadLoggerUtility.Log(client.msn, "Client Initialized");
                //Thread thread = new Thread(() => RunSingleClient(client));
                //thread.Start();

                var _ = RunSingleClient(client);

                //int progress = (clientsInitialized * 100) / clientsCount;

                //ProgressBarHelper.DrawTextProgressBar(progress, clientsInitialized, clientsCount);

                clientsInitialized++;
            }
        }


        private async Task RunSingleClient(MeterConfiguration conf)
        {



            try
            {
                CsvThreadLoggerUtility.Log(conf.msn, "Thread Started");
                //conf = conf.loadConfiguration("MeterConfigs/" + conf.msn + ".json");

                await conf.mdsm.GenerateSamplingData(conf);


                //while (true)
                //{
                //    if (clientsInitialized == MeterClientsGenerator.clients.Count)
                //    {
                //        break;
                //    }
                //}

                if (NeedsConnecting == 1)
                {
                    IPAddress ipAddress = IPAddress.Parse(conf.ippo.primary_ip_address);

                    var ipEndPoint = new IPEndPoint(ipAddress, conf.ippo.primary_port);

                    TcpClient client = new();
                    await client.ConnectAsync(ipEndPoint);
                    NetworkStream stream = client.GetStream();

                    await MeterClientRunningStream(stream, conf, client);
                }
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
                    //return;
                    IPAddress ipAddress = IPAddress.Parse(conf.ippo.primary_ip_address);

                    var ipEndPoint = new IPEndPoint(ipAddress, conf.ippo.primary_port);
                    client = new();
                    await client.ConnectAsync(ipEndPoint);
                    stream = client.GetStream();
                }

                await SendCommandAsync(stream, heartbeat, conf, true);
                if (isLogged == false)
                {
                    CsvThreadLoggerUtility.Log1(conf.msn, heartbeat);
                }


                string re = await ReadCommand(stream, conf, true);

                if (isLogged == false)
                {
                    CsvThreadLoggerUtility.Log1(conf.msn, re);
                }
                isLogged = true;


                if (re == "DA")
                {
                    while (true)
                    {
                        if ((DateTime.Now - lastCommTime).TotalSeconds > 30)
                            break;
                        if (MeterConfigurationUI.cancelled) return;


                        try
                        {
                            re = "";
                            do
                            {

                                re = await ReadCommand(stream, conf, false);
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


                                //if ((DateTime.Now - lastCommTime).TotalSeconds > 180)
                                //{
                                //    SendCommand(stream, heartbeat, conf, false);
                                //}

                            }
                            while (String.IsNullOrEmpty(re) && (DateTime.Now - lastCommTime).TotalSeconds < 30);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Connection is inactive");
                            break;
                        }

                        await ProcessCommandAsync(stream, conf, re);

                        //client.Close();
                        //return;

                        //conf.saveConfiguration("MeterConfigs/" + conf.msn + ".json");
                    }
                }
                else
                {
                    await ProcessCommandAsync(stream, conf, re);

                    //client.Close();
                    //return;

                }

            }
            while (!MeterConfigurationUI.cancelled);
            return;
        }

        /// <summary>
        /// Processes a received command, determines its type, and sends the appropriate response.
        /// </summary>
        /// <param name="stream">The network stream to communicate over.</param>
        /// <param name="conf">The configuration for the specific meter.</param>
        /// <param name="re">The raw command string received.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task ProcessCommandAsync(NetworkStream stream, MeterConfiguration conf, string re)
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
                    string finalCommand = "00 01 00 01 00 30 00 " + Convert.ToString(count, 16).PadLeft(2, '0') + " " + sendCmd;
                    sendCmd = finalCommand;
                    break;
                case CommandType.IMEI_READ:
                    sendCmd = "C4 01 81 00 09 0F 38 36 31 33 36 35 30 34 33 31 32 38 34 31 37";
                    cmdArr = sendCmd.Split(' ');
                    count = cmdArr.Length;
                    finalCommand = "00 01 00 01 00 30 00 " + Convert.ToString(count, 16).PadLeft(2, '0') + " " + sendCmd;
                    sendCmd = finalCommand;
                    break;
                case CommandType.AARQ:
                    Random random = new Random();

                    int r = random.Next(5, 10);
                    if (PasswordChecker(re, conf))
                    {
                        Thread.Sleep(r * 1000);
                        sendCmd = aare;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Password Correct");
                        Console.ResetColor();
                    }
                    else
                    {
                        Thread.Sleep(r * 1000);
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
                    finalCommand = "00 01 00 01 00 30 00 " + Convert.ToString(count, 16).PadLeft(2, '0') + " " + sendCmd;
                    sendCmd = finalCommand;
                    break;
                case CommandType.DMDT:
                    sendCmd = conf.dmdt.GetDataCommand(re);
                    if (sendCmd != "")
                    {
                        var cmdArr2 = sendCmd.Split(' ');
                        int count2 = cmdArr2.Length;
                        string finalCommand2 = "00 01 00 01 00 30 00 " + Convert.ToString(count2, 16).PadLeft(2, '0') + " " + sendCmd;
                        sendCmd = finalCommand2;
                    }
                    break;
                case CommandType.MDSM:
                    sendCmd = conf.mdsm.ProcessCommand(re);
                    if (sendCmd != "")
                    {
                        var cmdArr3 = sendCmd.Split(' ');
                        int count3 = cmdArr3.Length;
                        string finalCommand3 = "00 01 00 01 00 30 00 " + Convert.ToString(count3, 16).PadLeft(2, '0') + " " + sendCmd;
                        sendCmd = finalCommand3;
                    }
                    break;
                case CommandType.AUXR:
                    sendCmd = conf.auxr.ProcessCommand(re);
                    if (sendCmd != "")
                    {
                        var cmdArr3 = sendCmd.Split(' ');
                        int count3 = cmdArr3.Length;
                        string finalCommand3 = "00 01 00 01 00 30 00 " + Convert.ToString(count3, 16).PadLeft(2, '0') + " " + sendCmd;
                        sendCmd = finalCommand3;
                    }
                    break;
                case CommandType.WSIM:
                    sendCmd = conf.wsim.ProcessCommand(re);
                    if (sendCmd != "")
                    {
                        var cmdArr3 = sendCmd.Split(' ');
                        int count3 = cmdArr3.Length;
                        string finalCommand3 = "00 01 00 01 00 30 00 " + Convert.ToString(count3, 16).PadLeft(2, '0') + " " + sendCmd;
                        sendCmd = finalCommand3;
                    }
                    break;
                case CommandType.IPPO:
                    sendCmd = conf.ippo.ProcessCommand(re);
                    if (sendCmd != "")
                    {
                        var cmdArr3 = sendCmd.Split(' ');
                        int count3 = cmdArr3.Length;
                        string finalCommand3 = "00 01 00 01 00 30 00 " + Convert.ToString(count3, 16).PadLeft(2, '0') + " " + sendCmd;
                        sendCmd = finalCommand3;
                    }
                    break;
                case CommandType.OPPO:
                    sendCmd = conf.oppo.ProcessCommand(re);
                    if (sendCmd != "")
                    {
                        var cmdArr3 = sendCmd.Split(' ');
                        int count3 = cmdArr3.Length;
                        string finalCommand3 = "00 01 00 01 00 30 00 " + Convert.ToString(count3, 16).PadLeft(2, '0') + " " + sendCmd;
                        sendCmd = finalCommand3;
                    }
                    break;
                case CommandType.TIOU:
                    sendCmd = conf.tiou.ProcessCommand(re);
                    if (sendCmd != "")
                    {
                        var cmdArr3 = sendCmd.Split(' ');
                        int count3 = cmdArr3.Length;
                        string finalCommand3 = "00 01 00 01 00 30 00 " + Convert.ToString(count3, 16).PadLeft(2, '0') + " " + sendCmd;
                        sendCmd = finalCommand3;
                    }
                    break;
                case CommandType.MDI:
                    sendCmd = conf.mdi.ProcessCommand(re);
                    if (sendCmd != "")
                    {
                        var cmdArr3 = sendCmd.Split(' ');
                        int count3 = cmdArr3.Length;
                        string finalCommand3 = "00 01 00 01 00 30 00 " + Convert.ToString(count3, 16).PadLeft(2, '0') + " " + sendCmd;
                        sendCmd = finalCommand3;
                    }
                    break;
                case CommandType.TIME_SYNCHRONIZATION:
                    sendCmd = conf.dvtm.ProcessCommand(re);
                    if (sendCmd != "")
                    {
                        var cmdArr3 = sendCmd.Split(' ');
                        int count3 = cmdArr3.Length;
                        string finalCommand3 = "00 01 00 01 00 30 00 " + Convert.ToString(count3, 16).PadLeft(2, '0') + " " + sendCmd;
                        sendCmd = finalCommand3;
                    }
                    break;
                //case CommandType.SANCTIONED_LOAD_CONTROL:
                //    sendCmd = conf.sanc.ProcessCommand(re);
                //    if (sendCmd != "")
                //    {
                //        var cmdArr3 = sendCmd.Split(' ');
                //        int count3 = cmdArr3.Length;
                //        string finalCommand3 = "00 01 00 01 00 30 00 " + Convert.ToString(count3, 16).PadLeft(2, '0') + " " + sendCmd;
                //        sendCmd = finalCommand3;
                //    }
                //    break;
                case CommandType.LOAD_SHEDDING_SCHEDULLING:
                    sendCmd = conf.lsch.ProcessCommand(re);
                    if (sendCmd != "")
                    {
                        var cmdArr3 = sendCmd.Split(' ');
                        int count3 = cmdArr3.Length;
                        string finalCommand3 = "00 01 00 01 00 30 00 " + Convert.ToString(count3, 16).PadLeft(2, '0') + " " + sendCmd;
                        sendCmd = finalCommand3;
                    }
                    break;
                case CommandType.INST_DATA_READ:
                    Logger.Instance.Log(conf.msn, "INST Data Read", "");
                    re = await conf.mdsm.ProcessCommandForInstantaneousDataAsync(re, stream, conf);
                    if (re != "")
                    {
                        await ProcessCommandAsync(stream, conf, re);
                        return;
                    }
                    sendCmd = "";
                    break;
                case CommandType.Event:
                    Logger.Instance.Log(conf.msn, "Event Data Read", "");
                     re = await conf.mdsm.ProcessCommandForEventDataAsync(re, stream, conf);
                    if (re != "")
                    {
                        await ProcessCommandAsync(stream, conf, re);
                        return;
                    }
                    sendCmd = "";
                    break;
                case CommandType.BILL_DATA_READ:
                    re = await conf.mdsm.ProcessCommandForBillingDataAsync(re, stream, conf);
                    if (re != "")
                    {
                        await ProcessCommandAsync(stream, conf, re);
                        return;
                    }
                    sendCmd = "";
                    break;
                case CommandType.MBILL_DATA_READ:
                    re = await conf.mdsm.ProcessCommandForMBillingDataAsync(re, stream, conf);
                    if (re != "")
                    {
                        await ProcessCommandAsync(stream, conf, re);
                        return;
                    }
                    sendCmd = "";
                    break;

                case CommandType.LPRO_DATA_READ:
                    re = await conf.mdsm.ProcessCommandForLPRODataAsync(re, stream, conf);
                    if (re != "")
                    {
                        await ProcessCommandAsync(stream, conf, re);
                        return;
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
                await SendCommandAsync(stream, sendCmd, conf, false);
            }
            else
            {
                await SendCommandAsync(stream, error, conf, false);
            }
        }

        /// <summary>
        /// Asynchronously reads a command from the network stream and converts it to a hex string.
        /// </summary>
        /// <param name="stream">The network stream to read from.</param>
        /// <param name="conf">The meter configuration context.</param>
        /// <param name="needsLogg">Whether to log the received data.</param>
        /// <returns>The received command as a hex string.</returns>
        public static async Task<string> ReadCommand(NetworkStream stream, MeterConfiguration conf, bool needsLogg = false)
        {
            string re = "";
            try
            {
                var data = new byte[512];

                stream.ReadTimeout = 200000;

                int count = await stream.ReadAsync(data, 0, data.Length);

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

        /// <summary>
        /// Asynchronously sends a hex command string over the network stream.
        /// </summary>
        /// <param name="stream">The network stream to write to.</param>
        /// <param name="commandStr">The hex command string to send.</param>
        /// <param name="conf">The meter configuration context.</param>
        /// <param name="needsLogg">Whether to log the sent data.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task SendCommandAsync(NetworkStream stream, string commandStr, MeterConfiguration conf, bool needsLogg = false)
        {
            try
            {
                commandStr = commandStr.Replace(" ", "");

                commandStr = AddSpaceEveryNCharacters(commandStr, 2);

                byte[] command = commandStr.Split()
                .Select(s => Convert.ToByte(s, 16)).ToArray();

                await stream.WriteAsync(command, 0, command.Length);

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
                Console.WriteLine($"Meter {conf.msn}: " + ex.Message);
            }
        }

        public bool PasswordChecker(string aarq, MeterConfiguration conf)
        {
            string s = "00 01 00 01 00 30 00 38 60 36 A1 09 06 07 60 85 74 05 08 01 01 8A 02 07 80 8B 07 60 85 74 05 08 02 01 AC 0A 80 08 ";

            int endIndex = aarq.IndexOf(" BE 10 04 0E 01 00 00 00 06 5F 1F 04 00 00 7E 1F 04 B0");

            string password = aarq.Substring(s.Length, endIndex - s.Length).Replace(" ", "");

            //string meterPassword = ConvertToX4ByteString(meterConfiguration.password).Replace(" ", "");

            string meterPassword = string.Concat(conf.password.Select(c => "3" + c));


            Console.WriteLine($"{meterPassword} {password}");

            if (password != meterPassword)
            {

            }

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
