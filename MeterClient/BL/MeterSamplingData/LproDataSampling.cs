using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MeterClient.Helper;

namespace MeterClient.BL.MeterSamplingData
{
    public class LproDataSampling
    {
        public long Id { get; set; } // Auto-increment (Not NULL) Unique-ID
        public string Msn { get; set; }
        public long Bigint { get; set; }
        public string MeterSerialNumber { get; set; }
        public string GlobalDeviceId { get; set; } // Unique ID for each device by MDM
        public DateTime MeterDateTime { get; set; } // Meter Date & Time
        public double Frequency { get; set; } // Frequency in Hertz
        public int ChannelId { get; set; } // Load Profile Channel ID where applicable
        public int Interval { get; set; } // Interval of Profile

        // Active Energy (Import)
        public double ActiveEnergyPosT1 { get; set; }
        public double ActiveEnergyPosT2 { get; set; }
        public double ActiveEnergyPosTl { get; set; }

        // Active Energy (Export)
        public double ActiveEnergyNegT1 { get; set; }
        public double ActiveEnergyNegT2 { get; set; }
        public double ActiveEnergyNegTl { get; set; }

        // Absolute Active Energy
        public double ActiveEnergyAbsT1 { get; set; }
        public double ActiveEnergyAbsT2 { get; set; }
        public double ActiveEnergyAbsTl { get; set; }

        // Reactive Energy (Import)
        public double ReactiveEnergyPosT1 { get; set; }
        public double ReactiveEnergyPosT2 { get; set; }
        public double ReactiveEnergyPosTl { get; set; }

        // Reactive Energy (Export)
        public double ReactiveEnergyNegT1 { get; set; }
        public double ReactiveEnergyNegT2 { get; set; }
        public double ReactiveEnergyNegTl { get; set; }

        // Absolute Reactive Energy
        public double ReactiveEnergyAbsT1 { get; set; }
        public double ReactiveEnergyAbsT2 { get; set; }
        public double ReactiveEnergyAbsTl { get; set; }

        // Active MDI (Import)
        public double ActiveMdiPosT1 { get; set; }
        public double ActiveMdiPosT2 { get; set; }
        public double ActiveMdiPosTl { get; set; }

        // Active MDI (Export)
        public double ActiveMdiNegT1 { get; set; }
        public double ActiveMdiNegT2 { get; set; }
        public double ActiveMdiNegTl { get; set; }

        // Absolute Active MDI
        public double ActiveMdiAbsT1 { get; set; }
        public double ActiveMdiAbsT2 { get; set; }
        public double ActiveMdiAbsTl { get; set; }

        // Cumulative Active MDI (Import)
        public double CumulativeMdiPosT1 { get; set; }
        public double CumulativeMdiPosT2 { get; set; }
        public double CumulativeMdiPosTl { get; set; }

        // Cumulative Active MDI (Export)
        public double CumulativeMdiNegT1 { get; set; }
        public double CumulativeMdiNegT2 { get; set; }
        public double CumulativeMdiNegTl { get; set; }

        // Absolute Cumulative Active MDI
        public double CumulativeMdiAbsT1 { get; set; }
        public double CumulativeMdiAbsT2 { get; set; }
        public double CumulativeMdiAbsTl { get; set; }

        // Current Phase
        public double CurrentPhaseA { get; set; }
        public double CurrentPhaseB { get; set; }
        public double CurrentPhaseC { get; set; }

        // Voltage Phase
        public double VoltagePhaseA { get; set; }
        public double VoltagePhaseB { get; set; }
        public double VoltagePhaseC { get; set; }

        // Active Power Import Phase
        public double ActivePwrPosPhaseA { get; set; }
        public double ActivePwrPosPhaseB { get; set; }
        public double ActivePwrPosPhaseC { get; set; }

        // Active Power Export Phase
        public double ActivePwrNegPhaseA { get; set; }
        public double ActivePwrNegPhaseB { get; set; }
        public double ActivePwrNegPhaseC { get; set; }

        // Aggregate Active Power
        public double AggregateActivePwrPos { get; set; }
        public double AggregateActivePwrNeg { get; set; }
        public double AggregateActivePwrAbs { get; set; }


        // Reactive Power Import Phase
        public double ReactivePwrPosPhaseA { get; set; }
        public double ReactivePwrPosPhaseB { get; set; }
        public double ReactivePwrPosPhaseC { get; set; }

        // Aggregate Reactive Power Import
        public double AggregateReactivePwrPos { get; set; }

        // Reactive Power Export Phase
        public double ReactivePwrNegPhaseA { get; set; }
        public double ReactivePwrNegPhaseB { get; set; }
        public double ReactivePwrNegPhaseC { get; set; }

        // Aggregate Reactive Power Export
        public double AggregateReactivePwrNeg { get; set; }
        public double AggregateReactivePwrAbs { get; set; } // Aggregate Reactive Power Absolute as per Pakistan standard

        public double AveragePowerFactor { get; set; } // Average Power Factor
        public DateTime MdcReadDateTime { get; set; } // Reading Date & Time of MDC
        public DateTime DbDateTime { get; set; } // Record Entry Date & Time in Database
        public int IsSynced { get; set; } // 0 By default, 1 if data is safe to delete from MDC

        public string date { get; set; }
        public string time { get; set; }

        // Constructor
        public LproDataSampling(MeterConfiguration conf)
        {
            Random random = new Random();

            Msn = conf.msn;
            ActiveEnergyPosT1 = random.Next(1, 101);
            ActiveEnergyPosT2 = random.Next(1, 101);
            ActiveEnergyPosTl = random.Next(1, 101);

            ActiveEnergyNegT1 = random.Next(1, 101);
            ActiveEnergyNegT2 = random.Next(1, 101);
            ActiveEnergyNegTl = random.Next(1, 101);

            ActiveEnergyAbsT1 = random.Next(1, 101);
            ActiveEnergyAbsT2 = random.Next(1, 101);
            ActiveEnergyAbsTl = random.Next(1, 101);



            ReactiveEnergyPosT1 = random.Next(1, 101);
            ReactiveEnergyPosT2 = random.Next(1, 101);
            ReactiveEnergyPosTl = random.Next(1, 101);

            ReactiveEnergyNegT1 = random.Next(1, 101);
            ReactiveEnergyNegT2 = random.Next(1, 101);
            ReactiveEnergyNegTl = random.Next(1, 101);

            ReactiveEnergyAbsT1 = random.Next(1, 101);
            ReactiveEnergyAbsT2 = random.Next(1, 101);
            ReactiveEnergyAbsTl = random.Next(1, 101);



            ActiveMdiPosT1 = random.Next(1, 101);
            ActiveMdiPosT2 = random.Next(1, 101);
            ActiveMdiPosTl = random.Next(1, 101);

            ActiveMdiNegT1 = random.Next(1, 101);
            ActiveMdiNegT2 = random.Next(1, 101);
            ActiveMdiNegTl = random.Next(1, 101);

            ActiveMdiAbsT1 = random.Next(1, 101);
            ActiveMdiAbsT2 = random.Next(1, 101);
            ActiveMdiAbsTl = random.Next(1, 101);


            CumulativeMdiPosT1 = random.Next(1, 101);
            CumulativeMdiPosT2 = random.Next(1, 101);
            CumulativeMdiPosTl = random.Next(1, 101);

            CumulativeMdiNegT1 = random.Next(1, 101);
            CumulativeMdiNegT2 = random.Next(1, 101);
            CumulativeMdiNegTl = random.Next(1, 101);

            CumulativeMdiAbsT1 = random.Next(1, 101);
            CumulativeMdiAbsT2 = random.Next(1, 101);
            CumulativeMdiAbsTl = random.Next(1, 101);




            IsSynced = random.Next(0, 2);

            DateTime Timestamp = DateTime.Now;

            date = Timestamp.ToString("yyyy-MM-dd");
            time = Timestamp.ToString("HH:mm:ss");
        }

        public LproDataSampling() { }
        public void SaveDataToCsv(LproDataSampling data, MeterConfiguration conf)
        {

            try
            {

                string filePath = "MeterSamplingData/LPROData/Meter-" + conf.msn + ".csv";

                // Check if the file exists
                bool fileExists = File.Exists(filePath);

                using (var writer = new StreamWriter(filePath, true))
                using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = !fileExists }))
                {
                    // If the file doesn't exist, write the header
                    if (!fileExists)
                    {
                        csv.WriteHeader<LproDataSampling>();
                        csv.NextRecord();
                    }

                    // Write the new data record
                    csv.WriteRecord<LproDataSampling>(data);
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


                string filePath = "MeterSamplingData/LPROData/Meter-" + conf.msn + ".csv";

                // Read all data from the CSV file
                List<LproDataSampling> dataList;
                if (File.Exists(filePath))
                {
                    using (var reader = new StreamReader(filePath))
                    using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                    {
                        dataList = csv.GetRecords<LproDataSampling>().ToList();
                    }
                }
                else
                {
                    dataList = new List<LproDataSampling>();
                }

                // Get the current date at midnight
                DateTime currentDate = DateTime.Now.Date;

                // Calculate the date one week ago
                DateTime oneWeekAgo = currentDate.AddDays(-7);

                // Filter data to keep only those within the one-week period
                dataList = dataList.Where(item => (Convert.ToDateTime(item.date)) >= oneWeekAgo).ToList();

                // Save the filtered data back to the file
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

        public static List<LproDataSampling> getData(MeterConfiguration conf, DateTime startDate, DateTime endDate)
        {
            string filePath = "MeterSamplingData/LPROData/Meter-" + conf.msn + ".csv";

            // Read all data from the CSV file
            List<LproDataSampling> dataList;
            if (File.Exists(filePath))
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    dataList = csv.GetRecords<LproDataSampling>().ToList();
                }
            }
            else
            {
                dataList = new List<LproDataSampling>();
            }


            // Assuming dataList is a List<LproDataSampling> containing your records
            List<LproDataSampling> filteredRecords = dataList
                .Where(record =>
                    DateTime.Parse($"{record.date} {record.time}") >= startDate &&
                    DateTime.Parse($"{record.date} {record.time}") <= endDate)
                .ToList();


            return filteredRecords;
        }


        public string DataInCommand(MeterConfiguration conf)
        {

            DateTime _date = DateTime.ParseExact(date + " " + time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            string com = $"02 25 09 0C {Converter.Instance.DateTimeToHex(_date, "05")} 00 FF 10 00 06 {Converter.Instance.ValueToHex(8, conf.mdsm.sampling_interval)} 06 {Converter.Instance.ValueToHex(8, ActiveEnergyPosT1)} 06 {Converter.Instance.ValueToHex(8, ActiveEnergyPosT2)} 06 {Converter.Instance.ValueToHex(8, ActiveEnergyPosTl)} 06 {Converter.Instance.ValueToHex(8, ActiveEnergyNegT1)} 06 {Converter.Instance.ValueToHex(8, ActiveEnergyNegT2)} 06 {Converter.Instance.ValueToHex(8, ActiveEnergyNegTl)} 06 {Converter.Instance.ValueToHex(8, ActiveEnergyAbsT1)} 06 {Converter.Instance.ValueToHex(8, ActiveEnergyAbsT2)} 06 {Converter.Instance.ValueToHex(8, ActiveEnergyAbsTl)} 06 {Converter.Instance.ValueToHex(8, ReactiveEnergyPosT1)} 06 {Converter.Instance.ValueToHex(8, ReactiveEnergyPosT2)} 06 {Converter.Instance.ValueToHex(8, ReactiveEnergyPosTl)} 06 {Converter.Instance.ValueToHex(8, ReactiveEnergyNegT1)} 06 {Converter.Instance.ValueToHex(8, ReactiveEnergyNegT2)} 06 {Converter.Instance.ValueToHex(8, ReactiveEnergyNegTl)} 06 {Converter.Instance.ValueToHex(8, ReactiveEnergyAbsT1)} 06 {Converter.Instance.ValueToHex(8, ReactiveEnergyAbsT2)} 06 {Converter.Instance.ValueToHex(8, ReactiveEnergyAbsTl)} 06 {Converter.Instance.ValueToHex(8, ActiveMdiPosT1)} 06 {Converter.Instance.ValueToHex(8, ActiveMdiPosT2)} 06 {Converter.Instance.ValueToHex(8, ActiveMdiPosTl)} 06 {Converter.Instance.ValueToHex(8, ActiveMdiNegT1)} 06 {Converter.Instance.ValueToHex(8, ActiveMdiNegT2)} 06 {Converter.Instance.ValueToHex(8, ActiveMdiNegTl)} 06 {Converter.Instance.ValueToHex(8, ActiveMdiAbsT1)} 06 {Converter.Instance.ValueToHex(8, ActiveMdiAbsT2)} 06 {Converter.Instance.ValueToHex(8, ActiveMdiAbsTl)} 06 {Converter.Instance.ValueToHex(8, AggregateActivePwrPos)} 06 {Converter.Instance.ValueToHex(8, AggregateActivePwrNeg)} 06 {Converter.Instance.ValueToHex(8, AggregateActivePwrAbs)} 06 {Converter.Instance.ValueToHex(8, AggregateReactivePwrPos)} 06 {Converter.Instance.ValueToHex(8, AggregateReactivePwrNeg)} 06 {Converter.Instance.ValueToHex(8, AggregateReactivePwrAbs)} 12 {Converter.Instance.ValueToHex(4, Frequency)} 06 {Converter.Instance.ValueToHex(8, AveragePowerFactor)}";


            return com;
        }

    }
}
