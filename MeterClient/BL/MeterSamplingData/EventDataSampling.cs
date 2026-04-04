using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using CsvHelper;
using System.IO;
using System.Globalization;
using MeterClient.Helper;

namespace MeterClient.BL.MeterSamplingData
{
    public enum Event
    {
        MdiReset = 101,
        Parameterization = 102,
        PowerFailStart = 111,
        PowerFailEnd = 112,
        PhaseFailure = 113,
        OverVoltage = 114,
        UnderVoltage = 115,
        DemandOverload = 116,
        //ReverseEnergy = 117,

        //PhaseFailureL1Start = 131,
        //PhaseFailureL1End = 132,
        //PhaseFailureL2Start = 133,
        //PhaseFailureL2End = 134,
        //PhaseFailureL3Start = 135,
        //PhaseFailureL3End = 136,
        //PhaseFailureTrip = 137,
        //OverVoltageL1Start = 138,
        //OverVoltageL1End = 139,
        //OverVoltageL2Start = 140,
        //OverVoltageL2End = 141,
        //OverVoltageL3Start = 142,
        //OverVoltageL3End = 143,
        //OverVoltageTrip = 144,
        //UnderVoltageL1Start = 145,
        //UnderVoltageL1End = 146,
        //UnderVoltageL2Start = 147,
        //UnderVoltageL2End = 148,
        //UnderVoltageL3Start = 149,
        //UnderVoltageL3End = 150,
        //UnderVoltageTrip = 151,
        //PhaseOverCurrentL1Start = 152,
        //PhaseOverCurrentL1End = 153,
        //PhaseOverCurrentL2Start = 154,
        //PhaseOverCurrentL2End = 155,
        //PhaseOverCurrentL3Start = 156,
        //PhaseOverCurrentL3End = 157,
        //PhaseOverCurrentTrip = 158,
        //DemandOverloadEnd = 159,
        //OverloadStart = 160,
        //OverloadEnd = 161,
        //OverLoadTrip = 162,
        //VoltageUnbalanceStart = 163,
        //VoltageUnbalanceEnd = 164,
        //CurrentUnbalanceStart = 165,
        //CurrentUnbalanceEnd = 166,
        //HighApparentPowerStart = 167,
        //HighApparentPowerEnd = 168,
        //RemoteSwitchOn = 169,
        //RemoteSwitchOff = 170,
        //AutomaticSwitchOn = 171,
        //AutomaticSwitchOff = 172,
        //ManualSwitchOn = 173,
        //ManualSwitchOff = 174,
        //ResidualCurrentAlarmStart = 175,
        //ResidualCurrentAlarmEnd = 176,
        //ResidualCurrentTrip = 177,
        //ShortCircuitTrip = 179,
        //MccbTrip = 180,
        //ReverseEnergyL1Start = 187,
        //ReverseEnergyL1End = 188,
        //ReverseEnergyL2Start = 189,
        //ReverseEnergyL2End = 190,
        //ReverseEnergyL3Start = 191,
        //ReverseEnergyL3End = 192,

        //TimeSynchronization = 201,
        //ContactorOn = 202,
        //ContactorOff = 203,
        //DoorOpen = 206,
        //BatteryLow = 207,
        //MemoryFailure = 208,

        //OpticalPortLogin = 301,
        //SanctionLoadControlProgrammed = 303,
        //LoadSheddingScheduleProgrammed = 304,
        //IpAndPortProgrammed = 305,
        //TimeOfUseProgrammed = 306,
        //WakeupSimProgrammed = 307,
        //OverVoltageFunctionProgrammed = 308,
        //UnderVoltageFunctionProgrammed = 309,
        //OverCurrentFunctionProgrammed = 310,
        //OverLoadFunctionProgrammed = 311,
        //PhaseFailureFunctionProgrammed = 312,
        //VoltageUnbalanceFunctionProgrammed = 313,
        //CurrentUnbalanceFunctionProgrammed = 314,
        //HighApparentPowerFunctionProgrammed = 315,
        //OverVoltageFunctionCancelled = 316,
        //UnderVoltageFunctionCancelled = 317,
        //OverCurrentFunctionCancelled = 318,
        //OverLoadFunctionCancelled = 319,
        //PhaseFailureFunctionCancelled = 320,
        //VoltageUnbalanceFunctionCancelled = 321,
        //CurrentUnbalanceFunctionCancelled = 322,
        //HighApparentPowerFunctionCancelled = 323,
        //SanctionLoadControlCancelled = 324,
        //LoadSheddingScheduleCancelled = 325,
        //TimeOfUseProgrammedCancelled = 326
    }
    public class EventDataSampling
    {
        public int EventCode{ get; set; }
        public DateTime EventOccurTime { get; set; }
        public string date { get; set; }

        public string time { get; set; }

        public EventDataSampling()
        {
            Random random = new Random();
            Array values = Enum.GetValues(typeof(Event)); // Get all possible values of the Event enum
            EventCode = (int)values.GetValue(random.Next(values.Length)); // Randomly select one of the enum values and assign its integer representation to EventCode
            EventOccurTime = DateTime.Now;

            DateTime Timestamp = DateTime.Now;

            date = Timestamp.ToString("yyyy-MM-dd");
            time = Timestamp.ToString("HH:mm:ss");
        }
       
        public void SaveDataToCsv(EventDataSampling data, MeterConfiguration conf)
        {
            try
            {
                string filePath = "MeterSamplingData/EventData/Meter-" + conf.msn + ".csv";
                bool fileExists = File.Exists(filePath);

                using (var writer = new StreamWriter(filePath, true))
                using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = !fileExists }))
                {
                    if (!fileExists)
                    {
                        csv.WriteHeader<EventDataSampling>();
                        csv.NextRecord();
                    }

                    csv.WriteRecord(data);
                    csv.NextRecord();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void CleanupOldData(MeterConfiguration conf)
        {
            try
            {
                string filePath = "MeterSamplingData/EventData/Meter-" + conf.msn + ".csv";
                List<EventDataSampling> dataList;

                if (File.Exists(filePath))
                {
                    using (var reader = new StreamReader(filePath))
                    using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                    {
                        dataList = csv.GetRecords<EventDataSampling>().ToList();
                    }
                }
                else
                {
                    dataList = new List<EventDataSampling>();
                }

                DateTime currentDate = DateTime.Now.Date;
                DateTime oneWeekAgo = currentDate.AddDays(-7);
                dataList = dataList.Where(item => (Convert.ToDateTime(item.date)) >= oneWeekAgo).ToList();

                using (var writer = new StreamWriter(filePath))
                using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    csv.WriteRecords(dataList);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static List<EventDataSampling> getData(MeterConfiguration conf, DateTime startDate, DateTime endDate)
        {
            string filePath = "MeterSamplingData/EventData/Meter-" + conf.msn + ".csv";
            List<EventDataSampling> dataList;

            if (File.Exists(filePath))
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    dataList = csv.GetRecords<EventDataSampling>().ToList();
                }
            }
            else
            {
                dataList = new List<EventDataSampling>();
            }

            List<EventDataSampling> filteredRecords = dataList
                .Where(record =>
                    DateTime.Parse($"{record.date} {record.time}") >= startDate &&
                    DateTime.Parse($"{record.date} {record.time}") <= endDate)
                .ToList();

            return filteredRecords;
        }

        public string DataInCommand(bool isLast = false)
        {
            DateTime _date = DateTime.ParseExact(date + " " + time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            string suffix = isLast ? "00 80 00 FF" : "00 80 FF";
            // EventCode must be 2 bytes (4 hex digits), e.g. 64 → "00 40", 218 → "00 DA"
            string eventHex = EventCode.ToString("X4"); // always 4 hex chars
            string eventCodeHex = eventHex.Substring(0, 2) + " " + eventHex.Substring(2, 2);
            string com = $"02 02 10 {eventCodeHex} 09 0C {Converter.Instance.DateTimeToHex(_date, "03")}  {suffix}";
            return com;
        }
    }
}
