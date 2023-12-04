using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeterClient.BL;
using Newtonsoft.Json;

namespace MeterClient
{
    public class MeterConfiguration
    {
        private static MeterConfiguration instance;
        private MeterConfiguration()
        {
            auxr = new AuxRelayOperations();
            oppo = new ActivateMeterOpticalPort();
            dmdt = new DeviceMetaData();
            ippo = new IpPort();
            lsch = new LoadSheddingScheduling();
            mdi = new MdiResetDate();
            mdsm = new MeterDataSampling();
            mtst = new MeterStatus();
            sanc = new SanctionedLoadControl();
            tiou = new TimeOfUse();
            dvtm = new TimeSynchronization();
            wsim = new WakeUpSimNumber();
        }

        public static MeterConfiguration Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MeterConfiguration();
                }
                return instance;
            }
        }


        public AuxRelayOperations auxr { get; set; }
        public ActivateMeterOpticalPort oppo { get; set; }
        public DeviceMetaData dmdt { get; set; }
        public IpPort ippo { get; set; }
        public LoadSheddingScheduling lsch { get; set; }
        public MdiResetDate mdi { get; set; }
        public MeterDataSampling mdsm { get; set; }
        public MeterStatus mtst { get; set; }
        public SanctionedLoadControl sanc { get; set; }
        public TimeOfUse tiou { get; set; }
        public TimeSynchronization dvtm { get; set; }
        public WakeUpSimNumber wsim { get; set; }


        // Configuration Values
        public string msn { get; set; }
        public string password { get; set; }

        public void saveConfiguration(string filePath = "")
        {
            // Check if the file exists
            if (!File.Exists(filePath))
            {
                // If the file doesn't exist, create an empty file
                File.WriteAllText(filePath, string.Empty);
            }
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public MeterConfiguration loadConfiguration(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                instance = JsonConvert.DeserializeObject<MeterConfiguration>(json);


            }
            else
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }
            return instance;
        }


        public string? GetFirstHexCommand(string hexCommand)
        {
            var parsedCommand = hexCommand.Split(' ');
            if (parsedCommand[0] == "DD")
            {
                if (parsedCommand[1] == "04")
                {
                    // Check if Valid MSN

                    return "DA ";
                }
            }
            return null;
        }

        public string? GetSecondCommand(string hexCommand)
        {
            if (hexCommand == "00 01 00 30 00 01 00 38 60 36 A1 09 06 07 60 85 74 05 08 01 01 8A 02 07 80 8B 07 60 85 74 05 08 02 01 AC 0A 80 08 31 32 33 34 35 36 37 38 BE 10 04 0E 01 00 00 00 06 5F 1F 04 00 00 7E 1F 04 B0 ")
            {
                return "00 01 00 01 00 30 00 2B 61 29 A1 09 06 07 60 85 74 05 08 01 01 A2 03 02 01 00 A3 05 A1 03 02 01 00 BE 10 04 0E 08 00 06 5F 1F 04 00 00 1C 1F 01 2C 00 07 ";
            }
            return null;
        }


    }
}
