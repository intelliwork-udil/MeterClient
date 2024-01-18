using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeterClient.Helper;

namespace MeterClient.BL.MeterSamplingData
{
    public class InstanteneousDataSampling
    {
        public int current_tariff_register { get; set; }
        public double signal_strength { get; set; }
        public long msn { get; set; }
        public double frequency { get; set; }
        public double current_phase_a { get; set; }
        public double current_phase_b { get; set; }
        public double current_phase_c { get; set; }
        public double voltage_phase_a { get; set; }
        public double voltage_phase_b { get; set; }
        public double voltage_phase_c { get; set; }
        public double aggregate_active_pwr_pos { get; set; }
        public double aggregate_active_pwr_neg { get; set; }
        public double aggregate_active_pwr_abs { get; set; }
        public double aggregate_reactive_pwr_pos { get; set; }
        public double aggregate_reactive_pwr_neg { get; set; }
        public double aggregate_reactive_pwr_abs { get; set; }
        public double average_pf { get; set; }
        public int is_synced { get; set; }
        public string date { get; set; }
        public string time { get; set; }

        // Constructor to initialize properties with random values
        public InstanteneousDataSampling(MeterConfiguration conf)
        {
            Random random = new Random();
            current_tariff_register = random.Next(1, 101);
            signal_strength = random.Next(1, 101);
            msn = Convert.ToInt64(conf.msn);
            frequency = random.Next(1, 101);
            current_phase_a = random.Next(1, 101);
            current_phase_b = random.Next(1, 101);
            current_phase_c = random.Next(1, 101);
            voltage_phase_a = random.Next(1, 101);
            voltage_phase_b = random.Next(1, 101);
            voltage_phase_c = random.Next(1, 101);
            aggregate_active_pwr_pos = random.Next(1, 101);
            aggregate_active_pwr_neg = random.Next(1, 101);
            aggregate_active_pwr_abs = random.Next(1, 101);
            aggregate_reactive_pwr_pos = random.Next(1, 101);
            aggregate_reactive_pwr_neg = random.Next(1, 101);
            aggregate_reactive_pwr_abs = random.Next(1, 101);
            average_pf = random.Next(1, 101);
            is_synced = random.Next(0, 2); // 0 or 1

            DateTime Timestamp = DateTime.Now;

            date = Timestamp.ToString("yyyy-MM-dd");
            time = Timestamp.ToString("HH:mm:ss");
        }

        public InstanteneousDataSampling() { }


        public void SaveDataToCsv(InstanteneousDataSampling data, MeterConfiguration conf)
        {

            try
            {

                string filePath = "MeterSamplingData/InstanteneousData/Meter-" + conf.msn + ".csv";

                // Check if the file exists
                bool fileExists = File.Exists(filePath);

                using (var writer = new StreamWriter(filePath, true))
                using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = !fileExists }))
                {
                    // If the file doesn't exist, write the header
                    if (!fileExists)
                    {
                        csv.WriteHeader<InstanteneousDataSampling>();
                    }

                    // Write the new data record
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


                string filePath = "MeterSamplingData/InstanteneousData/Meter-" + conf.msn + ".csv";

                // Read all data from the CSV file
                List<InstanteneousDataSampling> dataList;
                if (File.Exists(filePath))
                {
                    using (var reader = new StreamReader(filePath))
                    using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                    {
                        dataList = csv.GetRecords<InstanteneousDataSampling>().ToList();
                    }
                }
                else
                {
                    dataList = new List<InstanteneousDataSampling>();
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


        public static List<InstanteneousDataSampling> getData(MeterConfiguration conf, DateTime startDate, DateTime endDate)
        {
            string filePath = "MeterSamplingData/InstanteneousData/Meter-" + conf.msn + ".csv";

            // Read all data from the CSV file
            List<InstanteneousDataSampling> dataList;
            if (File.Exists(filePath))
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    dataList = csv.GetRecords<InstanteneousDataSampling>().ToList();
                }
            }
            else
            {
                dataList = new List<InstanteneousDataSampling>();
            }


            // Assuming dataList is a List<InstanteneousDataSampling> containing your records
            List<InstanteneousDataSampling> filteredRecords = dataList
                .Where(record =>
                    DateTime.Parse($"{record.date} {record.time}") >= startDate &&
                    DateTime.Parse($"{record.date} {record.time}") <= endDate)
                .ToList();


            return filteredRecords;
        }

        public string DataInCommand()
        {

            DateTime _date = DateTime.ParseExact(date + " " + time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            string com = $"02 11 09 0C {Converter.Instance.DateTimeToHex(_date, "02")} 00 FF 10 00 06 {Converter.Instance.ValueToHex(8, aggregate_active_pwr_pos)} 06 {Converter.Instance.ValueToHex(8, aggregate_active_pwr_neg)} 06 {Converter.Instance.ValueToHex(8, aggregate_active_pwr_abs)} 06 {Converter.Instance.ValueToHex(8, aggregate_reactive_pwr_pos)} 06 {Converter.Instance.ValueToHex(8, aggregate_reactive_pwr_neg)} 06 {Converter.Instance.ValueToHex(8, aggregate_reactive_pwr_abs)} 12 {Converter.Instance.ValueToHex(4, frequency)} 06 {Converter.Instance.ValueToHex(8, average_pf)} 12 {Converter.Instance.ValueToHex(4, voltage_phase_a)} 12 {Converter.Instance.ValueToHex(4, voltage_phase_b)} 12 {Converter.Instance.ValueToHex(4, voltage_phase_c)} 06 {Converter.Instance.ValueToHex(8, current_phase_a)} 06 {Converter.Instance.ValueToHex(8, current_phase_b)} 06 {Converter.Instance.ValueToHex(8, current_phase_c)} 09 02 {Converter.Instance.ValueToHex(4, signal_strength)} 11 {Converter.Instance.ValueToHex(2, current_tariff_register)}";


            return com;
        }
    }
}
