using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            //string startDateString = "2023-01-01";
            //string startTimeString = "08:00:00";
            //string endDateString = "2023-01-05";
            //string endTimeString = "17:00:00";

            //DateTime startDate = DateTime.ParseExact(startDateString + " " + startTimeString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            //DateTime endDate = DateTime.ParseExact(endDateString + " " + endTimeString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            //// Assuming dataList is a List<InstanteneousDataSampling> containing your records
            //List<InstanteneousDataSampling> filteredRecords = dataList
            //    .Where(record =>
            //        DateTime.ParseExact(record.date + " " + record.time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) >= startDate &&
            //        DateTime.ParseExact(record.date + " " + record.time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) <= endDate)
            //    .ToList();


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

            string com = $"02 11 09 0C {DateTimeToHex(_date)} 00 FF 10 00 06 {ValueToHex(8, aggregate_active_pwr_pos)} 06 {ValueToHex(8, aggregate_active_pwr_neg)} 06 {ValueToHex(8, aggregate_active_pwr_abs)} 06 {ValueToHex(8, aggregate_reactive_pwr_pos)} 06 {ValueToHex(8, aggregate_reactive_pwr_neg)} 06 {ValueToHex(8, aggregate_reactive_pwr_abs)} 12 {ValueToHex(4, frequency)} 06 {ValueToHex(8, average_pf)} 12 {ValueToHex(4, voltage_phase_a)} 12 {ValueToHex(4, voltage_phase_b)} 12 {ValueToHex(4, voltage_phase_c)} 06 {ValueToHex(8, current_phase_a)} 06 {ValueToHex(8, current_phase_b)} 06 {ValueToHex(8, current_phase_c)} 09 02 {ValueToHex(4, signal_strength)} 11 {ValueToHex(2, current_tariff_register)}";


            return com;
        }

        public string ValueToHex(int length, double? value = null, int? intVal = null)
        {
            // Step 1: Convert the double to a byte array
            byte[] bytes = null;
            if (value != null)
            {
                bytes = BitConverter.GetBytes((double)value);
            }
            else
            {
                bytes = BitConverter.GetBytes((int)intVal);
            }

            // Step 2: Convert each byte to a hexadecimal string
            string hexString = BitConverter.ToString(bytes).Replace("-", "");

            hexString = ProcessString(hexString, length);

            // Step 3: Insert a space after every 2 characters
            string spacedHexString = InsertSpaces(hexString, 2);

            return spacedHexString;
        }

        private string ProcessString(string inputStr, int length)
        {
            // Extract the last 4 characters
            string lastFourChars = inputStr.Length >= length ? inputStr.Substring(inputStr.Length - length) : inputStr;

            // Check if the length is less than length
            if (inputStr.Length < length)
            {
                // Add remaining 0 on the start
                return new string('0', length - inputStr.Length) + lastFourChars;
            }
            else
            {
                return lastFourChars;
            }
        }

        public string InsertSpaces(string str, int interval)
        {
            return string.Join(" ", SplitByInterval(str, interval));
        }

        public string[] SplitByInterval(string str, int interval)
        {
            int length = (int)Math.Ceiling((double)str.Length / interval);
            string[] result = new string[length];

            for (int i = 0; i < length; i++)
            {
                int startIndex = i * interval;
                int endIndex = Math.Min(startIndex + interval, str.Length);
                result[i] = str.Substring(startIndex, endIndex - startIndex);
            }

            return result;
        }

        string DateTimeToHex(DateTime dateTime)
        {

            string hexYear = dateTime.Year.ToString("X4");

            hexYear = hexYear.Substring(0, 2) + " " + hexYear.Substring(2);

            string hexMonth = dateTime.Month.ToString("X2");
            string hexDay = dateTime.Day.ToString("X2");
            string hexHour = dateTime.Hour.ToString("X2");
            string hexMinute = dateTime.Minute.ToString("X2");
            string hexSecond = dateTime.Second.ToString("X2");

            // Concatenate the hex components
            string hexDateTimeString = hexYear + " " + hexMonth + " " + hexDay + " 02 " + hexHour + " " + hexMinute + " " + hexSecond;

            return hexDateTimeString;
        }
    }
}
