using CsvHelper;
using CsvHelper.Configuration;
using MeterClient.BL.MeterSamplingData;
using MeterClient.Helper;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
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


        public async Task GenerateSamplingData(MeterConfiguration conf)
        {
            Thread thread = new Thread(async () => await GenerateINSTSamplingInterval(conf));
            thread.Start();

            Thread thread2 = new Thread(async () => await GenerateBillingSamplingInterval(conf));
            thread2.Start();

            Thread thread3 = new Thread(async () => await GenerateLPROSamplingInterval(conf));
            thread3.Start();

            Thread thread4 = new Thread(async () => await GenerateEventSamplingInterval(conf));
            thread4.Start();

        }

        //public async Task GenerateSamplingData(MeterConfiguration conf)
        //{
        //    var task1 = Task.Run(() => GenerateINSTSamplingInterval(conf));
        //    var task2 = Task.Run(() => GenerateBillingSamplingInterval(conf));
        //    var task3 = Task.Run(() => GenerateLPROSamplingInterval(conf));

        //    await Task.WhenAll(task1, task2, task3);
        //}


        private async Task GenerateINSTSamplingInterval(MeterConfiguration conf)
        {
            while (true)
            {
                InstanteneousDataSampling inst = new InstanteneousDataSampling(conf);
                inst.SaveDataToCsv(inst, conf);

                if (DateTime.Now.TimeOfDay == TimeSpan.Zero)
                {
                    inst.CleanupOldData(conf);
                }

                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }

        private async Task GenerateBillingSamplingInterval(MeterConfiguration conf)
        {
            while (true)
            {
                BillingDataSampling bill = new BillingDataSampling(conf);
                bill.SaveDataToCsv(bill, conf);

                if (DateTime.Now.TimeOfDay == TimeSpan.Zero)
                {
                    bill.CleanupOldData(conf);
                }

                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }

        private async Task GenerateLPROSamplingInterval(MeterConfiguration conf)
        {

            while (true)
            {
                LproDataSampling bill = new LproDataSampling(conf);
                bill.SaveDataToCsv(bill, conf);

                if (DateTime.Now.TimeOfDay == TimeSpan.Zero)
                {
                    bill.CleanupOldData(conf);
                }
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }

        private async Task GenerateEventSamplingInterval(MeterConfiguration conf)
        {
            while (true)
            {
                EventDataSampling ev = new EventDataSampling();
                ev.SaveDataToCsv(ev, conf);

                if (DateTime.Now.TimeOfDay == TimeSpan.Zero)
                {
                    ev.CleanupOldData(conf);
                }

                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }

        public int getSamplingIntervalInSeconds()
        {
            if (data_type == "INST")
            {
                sampling_interval = 15;
            }
            else if (data_type == "LPRO")
            {
                sampling_interval = 30;
            }
            else if (data_type == "BILL")
            {
                sampling_interval = 1440;
            }
            return this.sampling_interval * 60;
        }

        public string ProcessCommand(string re)
        {
            string command = "";
            if (re.Contains("C0 01 81 00 07 01 00 "))
            {
                if (re.Contains("63 01 00 FF 04"))
                {
                    this.data_type = "LPRO";
                }
                else if (re.Contains("63 01 01 FF 04"))
                {
                    this.data_type = "INST";
                }
                else if (re.Contains("63 02 00 FF 04"))
                {
                    this.data_type = "BILL";
                }
                int samplingIntervalInSeconds = getSamplingIntervalInSeconds();

                string samplingIntervalInSecondsHex = samplingIntervalInSeconds.ToString("X");


                samplingIntervalInSecondsHex = samplingIntervalInSecondsHex.PadLeft(8, '0');
                string intIncmd = MeterConfigurationUI.AddSpaceEveryNCharacters(samplingIntervalInSecondsHex, 2);

                command = "C4 01 81 00 06 " + intIncmd;

            }
            else if (re.Contains("C0 01 81 00 03 00 00 5E 5C 28 FF 02 00"))
            {
                command = "C4 01 81 00 11 " + this.sampling_interval.ToString("X").PadLeft(2, '0');
            }
            else if (re.Contains("C0 01 81 00 01 00 00 5E 5C 2C FF 02 00"))
            {
                string activYear = this.activation_datetime.Year.ToString("X").PadLeft(4, '0');
                activYear = MeterConfigurationUI.AddSpaceEveryNCharacters(activYear, 2);

                string activMonth = this.activation_datetime.Month.ToString("X").PadLeft(2, '0');
                string activDay = this.activation_datetime.Day.ToString("X").PadLeft(2, '0');


                string actDate = activYear + " " + activMonth + " " + activDay;


                string activHour = this.activation_datetime.Hour.ToString("X").PadLeft(2, '0');
                string activMinute = this.activation_datetime.Minute.ToString("X").PadLeft(2, '0');
                string activSecond = this.activation_datetime.Second.ToString("X").PadLeft(2, '0');

                string actTime = activHour + " " + activMinute + " " + activSecond;


                command = "C4 01 81 00 09 0C " + actDate + " 03 " + actTime + " 00 80 00 00";
            }


            return command;
        }


        public async Task<string> ProcessCommandForEventDataAsync(string re, NetworkStream stream, MeterConfiguration conf)
        {
            string command = "";
            if (re.Contains("C0 01 C1 00 07 01 00 63 62 00 FF 02 01 01 02 04 02 04 12 00 08 09 06 00 00 01 00 00 FF 0F 02 12 00 00 09 0C"))
            {
                var data = re.Split().Select(x => Convert.ToByte(x, 16)).ToArray();
                int a1 = data[44];
                int a2 = data[45];
                int a3 = data[46];
                int a4 = data[47];

                int year = Converter.Instance.ConvertToYear(a1, a2);
                int month = a3;
                int day = a4;

                int hr = data[49];
                int min = data[50];
                int sec = data[51];

                var startTime = new DateTime(year, month, day, hr, min, sec);

                a1 = data[58];
                a2 = data[59];
                a3 = data[60];
                a4 = data[61];

                year = Converter.Instance.ConvertToYear(a1, a2);
                month = a3;
                day = a4;

                hr = data[63];
                min = data[64];
                sec = data[65];

                var endTime = new DateTime(year, month, day, hr, min, sec);
                var dataList = EventDataSampling.getData(conf, startTime, endTime);

                string sendingCommand = "";

                foreach (var d in dataList)
                {
                    if (d == dataList.LastOrDefault())
                    {
                        sendingCommand += d.DataInCommand(true);
                    }
                    else
                    {
                        sendingCommand += d.DataInCommand(false) + " ";
                    }
                }

                sendingCommand = sendingCommand.Replace(" ", "");

                int packetSize = 255;
                int headerSize = 8;
                packetSize = packetSize - headerSize;
                packetSize = packetSize * 2;
                int packetNumber = 1;

                int chunkSize = packetSize - 24;
                int totalPackets = (int)Math.Ceiling((double)sendingCommand.Length / chunkSize);
                string totalPacketsHex = Convert.ToString(totalPackets, 16).PadLeft(2, '0');

                for (int i = 0; i < sendingCommand.Length; i += chunkSize)
                {
                    string packetData = sendingCommand.Substring(i, Math.Min(chunkSize, sendingCommand.Length - i));

                    bool isLastPacket = (i + chunkSize) >= sendingCommand.Length;
                    string lastBlockFlag = isLastPacket ? "01" : "00";
                    
                    string packetHeader = $"C4 02 C1 {lastBlockFlag} {Convert.ToString(packetNumber, 16).PadLeft(8, '0')} 00 82 01 18 01 {totalPacketsHex}";

                    packetData = packetHeader + packetData;
                    string packet = packetData;
                    packet = MeterConfigurationUI.AddSpaceEveryNCharacters(packet, 2);
                    var cmdArr = packet.Split(' ');
                    int count = cmdArr.Length;
                    string finalCommand = "00 01 00 30 00 01 00 " + Convert.ToString(count, 16).PadLeft(2, '0') + " " + packet;
                    packet = finalCommand;
                    await MeterConfigurationUI.SendCommandAsync(stream, packet, conf, false);
                    var response = await MeterConfigurationUI.ReadCommand(stream, conf, false);
                    var response1 = response.Replace(" ", "");
                    if (!response1.Contains("C00281000000") && !response1.Contains("C002C1000000"))
                    {
                        command = response;
                        break;
                    }
                    response1 = "";
                    response = "";
                    packetNumber++;
                }

            }
            return command;
        }


        public async Task<string> ProcessCommandForInstantaneousDataAsync(string re, NetworkStream stream, MeterConfiguration conf)
        {
            string command = "";
            if (re.Contains("C0 01 81 00 07 01 00 63 01 01 FF 02 01 01 02 04 02 04 12 00 08 09 06 00 00 01 00 00 FF 0F 02 12 00 00 09 0C"))
            {

                var data = re.Split().Select(x => Convert.ToByte(x, 16)).ToArray();


                int a1 = data[44];
                int a2 = data[45];
                int a3 = data[46];
                int a4 = data[47];


                int year = Converter.Instance.ConvertToYear(a1, a2);
                int month = a3;
                int day = a4;

                int hr = data[49];
                int min = data[50];
                int sec = data[51];

                var startTime = new DateTime(year, month, day, hr, min, sec);


                a1 = data[58];
                a2 = data[59];
                a3 = data[60];
                a4 = data[61];


                year = Converter.Instance.ConvertToYear(a1, a2);
                month = a3;
                day = a4;

                hr = data[63];
                min = data[64];
                sec = data[65];

                var endTime = new DateTime(year, month, day, hr, min, sec);

                var dataList = InstanteneousDataSampling.getData(conf, startTime, endTime);


                int numBlocks = 0;
                int maxDataPerBlock = 0;

                string sendingCommand = "";



                foreach (var d in dataList)
                {
                    if (d == dataList.LastOrDefault())
                    {
                        sendingCommand += d.DataInCommand();
                    }
                    else
                    {
                        sendingCommand += d.DataInCommand() + " ";
                    }
                }

                sendingCommand = sendingCommand.Replace(" ", "");

                int packetSize = 255;
                int headerSize = 8; // Assuming that every packet would be max 255 or FF size long

                packetSize = packetSize - headerSize;

                packetSize = packetSize * 2; // Since we are sending hex data and every data would be of length 2

                int packetNumber = 1;

                for (int i = 0; i < sendingCommand.Length; i += packetSize)
                {

                    string packetData = sendingCommand.Substring(i, Math.Min(packetSize - 24, sendingCommand.Length - i - 24));

                    string packetHeader = "C4028100000000" + Convert.ToString(packetNumber, 16).PadLeft(2, '0') + "00820118";

                    packetData = packetHeader + packetData;

                    // Determine if this is the last packet
                    bool isLastPacket = (i + packetSize) >= sendingCommand.Length;

                    // Construct the packet with the appropriate prefix and postfix
                    string packet = ConstructINSTPacket(packetData, isLastPacket, packetHeader, packetNumber);



                    packet = MeterConfigurationUI.AddSpaceEveryNCharacters(packet, 2);

                    var cmdArr = packet.Split(' ');
                    int count = cmdArr.Length;
                    string finalCommand = "00 01 00 30 00 01 00 " + Convert.ToString(count, 16).PadLeft(2, '0') + " " + packet;
                    packet = finalCommand;

                    await MeterConfigurationUI.SendCommandAsync(stream, packet, conf, false);

                    // Wait for the response
                    var response = await MeterConfigurationUI.ReadCommand(stream, conf, false);

                    var response1 = response.Replace(" ", "");

                    if (!response1.Contains("C00281000000"))
                    {
                        command = response;
                        break;
                    }
                    response1 = "";
                    response = "";
                    packetNumber++;
                }


            }


            return command;
        }
        public async Task<string> ProcessCommandForBillingDataAsync(string re, NetworkStream stream, MeterConfiguration conf)
        {
            string command = "";
            if (re.Contains("C0 01 81 00 07 01 00 63 02 00 FF 02 01 01 02 04 02 04 12 00 08 09 06 00 00 01 00 00 FF 0F 02 12 00 00 09 0C"))
            {

                var data = re.Split().Select(x => Convert.ToByte(x, 16)).ToArray();


                int a1 = data[44];
                int a2 = data[45];
                int a3 = data[46];
                int a4 = data[47];


                int year = Converter.Instance.ConvertToYear(a1, a2);
                int month = a3;
                int day = a4;

                int hr = data[49];
                int min = data[50];
                int sec = data[51];

                var startTime = new DateTime(year, month, day, hr, min, sec);


                a1 = data[58];
                a2 = data[59];
                a3 = data[60];
                a4 = data[61];


                year = Converter.Instance.ConvertToYear(a1, a2);
                month = a3;
                day = a4;

                hr = data[63];
                min = data[64];
                sec = data[65];

                var endTime = new DateTime(year, month, day, hr, min, sec);


                // Get Data List
                var dataList = BillingDataSampling.getData(conf, startTime, endTime);


                int numBlocks = 0;
                int maxDataPerBlock = 0;

                string sendingCommand = "";



                foreach (var d in dataList)
                {
                    if (d == dataList.LastOrDefault())
                    {
                        sendingCommand += d.DataInCommand(conf);
                    }
                    else
                    {
                        sendingCommand += d.DataInCommand(conf) + " ";
                    }
                }

                sendingCommand = sendingCommand.Replace(" ", "");

                // Send Data to MDC
                //string packetData = sendingCommand.Substring(i, Math.Min(packetSize, sendingCommand.Length - i));

                //int maxBufferSize = stream.Socket.SendBufferSize;

                int packetSize = 255;
                int headerSize = 8; // Assuming that every packet would be max 255 or FF size long

                packetSize = packetSize - headerSize;

                packetSize = packetSize * 2; // Since we are sending hex data and every data would be of length 2

                int packetNumber = 1;

                //int packetSize = Math.Min(maxBufferSize, sendingCommand.Length);

                for (int i = 0; i < sendingCommand.Length; i += packetSize)
                {

                    //sendingCommand = "C40281000000000200820118" + sendingCommand;

                    string packetData = sendingCommand.Substring(i, Math.Min(packetSize - 24, sendingCommand.Length - i - 24));

                    //string packetHeader = "C4028100000000" + Convert.ToString(packetNumber, 16).PadLeft(2, '0') + "00820118";
                    string packetHeader = $"C4 02 81 {Convert.ToString(packetNumber - 1, 16).PadLeft(2, '0')} {Convert.ToString(packetNumber, 16).PadLeft(8, '0')} 00 82 01 18 01 02";

                    packetData = packetHeader + packetData;

                    // Determine if this is the last packet
                    bool isLastPacket = (i + packetSize) >= sendingCommand.Length;

                    // Construct the packet with the appropriate prefix and postfix
                    string packet = ConstructBILLPacket(packetData, isLastPacket, packetHeader, packetNumber);



                    packet = MeterConfigurationUI.AddSpaceEveryNCharacters(packet, 2);

                    var cmdArr = packet.Split(' ');
                    int count = cmdArr.Length;
                    string finalCommand = "00 01 00 30 00 01 00 " + Convert.ToString(count, 16).PadLeft(2, '0') + " " + packet;
                    packet = finalCommand;

                    await MeterConfigurationUI.SendCommandAsync(stream, packet, conf, false);

                    // Wait for the response
                    var response = await MeterConfigurationUI.ReadCommand(stream, conf, false);

                    var response1 = response.Replace(" ", "");

                    if (!response1.Contains("C00281000000"))
                    {
                        command = response;
                        break;
                    }
                    response1 = "";
                    response = "";
                    packetNumber++;
                }


            }


            return command;
        }


        public async Task<string> ProcessCommandForLPRODataAsync(string re, NetworkStream stream, MeterConfiguration conf)
        {
            string command = "";
            try
            {
                if (re.Contains("C0 01 81 00 07 01 00 63 01 00 FF 02 01 01 02 04 02 04 12 00 08 09 06 00 00 01 00 00 FF 0F 02 12 00 00 09 0C"))
                {

                    var data = re.Split().Select(x => Convert.ToByte(x, 16)).ToArray();


                    int a1 = data[44];
                    int a2 = data[45];
                    int a3 = data[46];
                    int a4 = data[47];


                    int year = Converter.Instance.ConvertToYear(a1, a2);
                    int month = a3;
                    int day = a4;

                    int hr = data[49];
                    int min = data[50];
                    int sec = data[51];

                    var startTime = new DateTime(year, month, day, hr, min, sec);


                    a1 = data[58];
                    a2 = data[59];
                    a3 = data[60];
                    a4 = data[61];


                    year = Converter.Instance.ConvertToYear(a1, a2);
                    month = a3;
                    day = a4;

                    hr = data[63];
                    min = data[64];
                    sec = data[65];

                    var endTime = new DateTime(year, month, day, hr, min, sec);


                    // Get Data List
                    var dataList = LproDataSampling.getData(conf, startTime, endTime);


                    int numBlocks = 0;
                    int maxDataPerBlock = 0;

                    string sendingCommand = "";



                    foreach (var d in dataList)
                    {
                        if (d == dataList.LastOrDefault())
                        {
                            sendingCommand += d.DataInCommand(conf);
                        }
                        else
                        {
                            sendingCommand += d.DataInCommand(conf) + " ";
                        }
                    }

                    sendingCommand = sendingCommand.Replace(" ", "");

                    // Send Data to MDC
                    //string packetData = sendingCommand.Substring(i, Math.Min(packetSize, sendingCommand.Length - i));

                    //int maxBufferSize = stream.Socket.SendBufferSize;

                    int packetSize = 255;
                    int headerSize = 8; // Assuming that every packet would be max 255 or FF size long

                    packetSize = packetSize - headerSize;

                    packetSize = packetSize * 2; // Since we are sending hex data and every data would be of length 2

                    int packetNumber = 1;

                    //int packetSize = Math.Min(maxBufferSize, sendingCommand.Length);

                    for (int i = 0; i < sendingCommand.Length; i += packetSize)
                    {

                        //sendingCommand = "C40281000000000200820118" + sendingCommand;

                        string packetData = sendingCommand.Substring(i, Math.Min(packetSize - 24, sendingCommand.Length - i - 24));

                        //string packetHeader = "C4028100000000" + Convert.ToString(packetNumber, 16).PadLeft(2, '0') + "00820118";
                        string packetHeader = $"C4 02 81 {Convert.ToString(packetNumber - 1, 16).PadLeft(2, '0')} {Convert.ToString(packetNumber, 16).PadLeft(8, '0')} 00 82 01 18 01 02";

                        packetData = packetHeader + packetData;

                        // Determine if this is the last packet
                        bool isLastPacket = (i + packetSize) >= sendingCommand.Length;

                        // Construct the packet with the appropriate prefix and postfix
                        string packet = ConstructLPROPacket(packetData, isLastPacket, packetHeader, packetNumber);



                        packet = MeterConfigurationUI.AddSpaceEveryNCharacters(packet, 2);

                        var cmdArr = packet.Split(' ');
                        int count = cmdArr.Length;
                        string finalCommand = "00 01 00 30 00 01 00 " + Convert.ToString(count, 16).PadLeft(2, '0') + " " + packet;
                        packet = finalCommand;

                        await MeterConfigurationUI.SendCommandAsync(stream, packet, conf, false);

                        // Wait for the response
                        var response = await MeterConfigurationUI.ReadCommand(stream, conf, false);

                        var response1 = response.Replace(" ", "");

                        if (!response1.Contains("C00281000000"))
                        {
                            command = response;
                            break;
                        }
                        response1 = "";
                        response = "";
                        packetNumber++;
                    }


                }

            }
            catch (Exception)
            {
                command = "";
            }

            return command;
        }

        private string ConstructBILLPacket(string data, bool isLastPacket, string replacableStr, int packetNumber)
        {
            if (isLastPacket)
            {
                data = data.Replace(replacableStr, $"C4 02 81 {Convert.ToString(packetNumber - 1, 16).PadLeft(2, '0')} {Convert.ToString(packetNumber, 16).PadLeft(8, '0')} 00 7C ");
            }
            return data;
        }
        private string ConstructEVENTPacket(string data, bool isLastPacket, string replacableStr, int packetNumber)
        {
            if (isLastPacket)
            {
                data = data.Replace(replacableStr, $"C4 02 C1 01 {Convert.ToString(packetNumber, 16).PadLeft(8, '0')} ");
            }
            return data;
        }
        private string ConstructLPROPacket(string data, bool isLastPacket, string replacableStr, int packetNumber)
        {
            if (isLastPacket)
            {
                data = data.Replace(replacableStr, $"C4 02 81 01 {Convert.ToString(packetNumber, 16).PadLeft(8, '0')} 00 6E");
            }
            return data;
        }

        private string ConstructINSTPacket(string data, bool isLastPacket, string replacableStr, int packetNumber)
        {
            if (isLastPacket)
            {
                data = data.Replace(replacableStr, "C4028101000000" + Convert.ToString(packetNumber, 16).PadLeft(2, '0') + "0072");
            }
            return data;
        }
    }
}
