using CsvHelper;
using CsvHelper.Configuration;
using MeterClient.Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace MeterClient.BL.MeterSamplingData
{
    public class MonthlyBillingDataSampling
    {
        public long Id { get; set; } // Bigint Auto-increment (Not NULL) Unique-ID
        public string Msn { get; set; }
        public long Bigint { get; set; }
        public string MeterSerialNumber { get; set; }
        public string GlobalDeviceId { get; set; } // Varchar Unique ID for each device by MDM
        public DateTime MeterDateTime { get; set; } // Datetime Meter Date & Time

        // Active Energy Import
        public double ActiveEnergyPosT1 { get; set; }
        public double ActiveEnergyPosT2 { get; set; }
        public double ActiveEnergyPosT3 { get; set; }
        public double ActiveEnergyPosT4 { get; set; }
        public double ActiveEnergyPosTl { get; set; }

        // Active Energy Export
        public double ActiveEnergyNegT1 { get; set; }
        public double ActiveEnergyNegT2 { get; set; }
        public double ActiveEnergyNegT3 { get; set; }
        public double ActiveEnergyNegT4 { get; set; }
        public double ActiveEnergyNegTl { get; set; }

        // Absolute Active Energy
        public double ActiveEnergyAbsT1 { get; set; }
        public double ActiveEnergyAbsT2 { get; set; }
        public double ActiveEnergyAbsT3 { get; set; }
        public double ActiveEnergyAbsT4 { get; set; }
        public double ActiveEnergyAbsTl { get; set; }

        // Reactive Energy Import
        public double ReactiveEnergyPosT1 { get; set; }
        public double ReactiveEnergyPosT2 { get; set; }
        public double ReactiveEnergyPosT3 { get; set; }
        public double ReactiveEnergyPosT4 { get; set; }
        public double ReactiveEnergyPosTl { get; set; }

        // Reactive Energy Export
        public double ReactiveEnergyNegT1 { get; set; }
        public double ReactiveEnergyNegT2 { get; set; }
        public double ReactiveEnergyNegT3 { get; set; }
        public double ReactiveEnergyNegT4 { get; set; }
        public double ReactiveEnergyNegTl { get; set; }

        // Absolute Reactive Energy as per Pakistan standard
        public double ReactiveEnergyAbsT1 { get; set; }
        public double ReactiveEnergyAbsT2 { get; set; }
        public double ReactiveEnergyAbsT3 { get; set; }
        public double ReactiveEnergyAbsT4 { get; set; }
        public double ReactiveEnergyAbsTl { get; set; }

        // Active MDI Import
        public double ActiveMdiPosT1 { get; set; }
        public double ActiveMdiPosT2 { get; set; }
        public double ActiveMdiPosT3 { get; set; }
        public double ActiveMdiPosT4 { get; set; }
        public double ActiveMdiPosTl { get; set; }

        // Active MDI Export
        public double ActiveMdiNegT1 { get; set; }
        public double ActiveMdiNegT2 { get; set; }
        public double ActiveMdiNegT3 { get; set; }
        public double ActiveMdiNegT4 { get; set; }
        public double ActiveMdiNegTl { get; set; }

        // Absolute Active MDI
        public double ActiveMdiAbsT1 { get; set; }
        public double ActiveMdiAbsT2 { get; set; }
        public double ActiveMdiAbsT3 { get; set; }
        public double ActiveMdiAbsT4 { get; set; }
        public double ActiveMdiAbsTl { get; set; }

        // Cumulative Active MDI Import
        public double CumulativeMdiPosT1 { get; set; }
        public double CumulativeMdiPosT2 { get; set; }
        public double CumulativeMdiPosT3 { get; set; }
        public double CumulativeMdiPosT4 { get; set; }
        public double CumulativeMdiPosTl { get; set; }

        // Cumulative Active MDI Export
        public double CumulativeMdiNegT1 { get; set; }
        public double CumulativeMdiNegT2 { get; set; }
        public double CumulativeMdiNegT3 { get; set; }
        public double CumulativeMdiNegT4 { get; set; }
        public double CumulativeMdiNegTl { get; set; }

        // Absolute Cumulative Active MDI
        public double CumulativeMdiAbsT1 { get; set; }
        public double CumulativeMdiAbsT2 { get; set; }
        public double CumulativeMdiAbsT3 { get; set; }
        public double CumulativeMdiAbsT4 { get; set; }
        public double CumulativeMdiAbsTl { get; set; }

        public DateTime MdcReadDatetime { get; set; } // Reading Date & Time of MDC
        public DateTime DbDatetime { get; set; } // Record Entry Date & Time in Database
        public int IsSynced { get; set; } // 0 By default, 1 if data is safe to delete from MDC

        public string date { get; set; }
        public string time { get; set; }

        public MonthlyBillingDataSampling() { }

        public MonthlyBillingDataSampling(BillingDataSampling bill)
        {
            Msn = bill.Msn;
            MeterSerialNumber = bill.MeterSerialNumber;
            GlobalDeviceId = bill.GlobalDeviceId;
            MeterDateTime = bill.MeterDateTime;

            ActiveEnergyPosT1 = bill.ActiveEnergyPosT1;
            ActiveEnergyPosT2 = bill.ActiveEnergyPosT2;
            ActiveEnergyPosT3 = bill.ActiveEnergyPosT3;
            ActiveEnergyPosT4 = bill.ActiveEnergyPosT4;
            ActiveEnergyPosTl = bill.ActiveEnergyPosTl;

            ActiveEnergyNegT1 = bill.ActiveEnergyNegT1;
            ActiveEnergyNegT2 = bill.ActiveEnergyNegT2;
            ActiveEnergyNegT3 = bill.ActiveEnergyNegT3;
            ActiveEnergyNegT4 = bill.ActiveEnergyNegT4;
            ActiveEnergyNegTl = bill.ActiveEnergyNegTl;

            ActiveEnergyAbsT1 = bill.ActiveEnergyAbsT1;
            ActiveEnergyAbsT2 = bill.ActiveEnergyAbsT2;
            ActiveEnergyAbsT3 = bill.ActiveEnergyAbsT3;
            ActiveEnergyAbsT4 = bill.ActiveEnergyAbsT4;
            ActiveEnergyAbsTl = bill.ActiveEnergyAbsTl;

            ReactiveEnergyPosT1 = bill.ReactiveEnergyPosT1;
            ReactiveEnergyPosT2 = bill.ReactiveEnergyPosT2;
            ReactiveEnergyPosT3 = bill.ReactiveEnergyPosT3;
            ReactiveEnergyPosT4 = bill.ReactiveEnergyPosT4;
            ReactiveEnergyPosTl = bill.ReactiveEnergyPosTl;

            ReactiveEnergyNegT1 = bill.ReactiveEnergyNegT1;
            ReactiveEnergyNegT2 = bill.ReactiveEnergyNegT2;
            ReactiveEnergyNegT3 = bill.ReactiveEnergyNegT3;
            ReactiveEnergyNegT4 = bill.ReactiveEnergyNegT4;
            ReactiveEnergyNegTl = bill.ReactiveEnergyNegTl;

            ReactiveEnergyAbsT1 = bill.ReactiveEnergyAbsT1;
            ReactiveEnergyAbsT2 = bill.ReactiveEnergyAbsT2;
            ReactiveEnergyAbsT3 = bill.ReactiveEnergyAbsT3;
            ReactiveEnergyAbsT4 = bill.ReactiveEnergyAbsT4;
            ReactiveEnergyAbsTl = bill.ReactiveEnergyAbsTl;

            ActiveMdiPosT1 = bill.ActiveMdiPosT1;
            ActiveMdiPosT2 = bill.ActiveMdiPosT2;
            ActiveMdiPosT3 = bill.ActiveMdiPosT3;
            ActiveMdiPosT4 = bill.ActiveMdiPosT4;
            ActiveMdiPosTl = bill.ActiveMdiPosTl;

            ActiveMdiNegT1 = bill.ActiveMdiNegT1;
            ActiveMdiNegT2 = bill.ActiveMdiNegT2;
            ActiveMdiNegT3 = bill.ActiveMdiNegT3;
            ActiveMdiNegT4 = bill.ActiveMdiNegT4;
            ActiveMdiNegTl = bill.ActiveMdiNegTl;

            ActiveMdiAbsT1 = bill.ActiveMdiAbsT1;
            ActiveMdiAbsT2 = bill.ActiveMdiAbsT2;
            ActiveMdiAbsT3 = bill.ActiveMdiAbsT3;
            ActiveMdiAbsT4 = bill.ActiveMdiAbsT4;
            ActiveMdiAbsTl = bill.ActiveMdiAbsTl;

            CumulativeMdiPosT1 = bill.CumulativeMdiPosT1;
            CumulativeMdiPosT2 = bill.CumulativeMdiPosT2;
            CumulativeMdiPosT3 = bill.CumulativeMdiPosT3;
            CumulativeMdiPosT4 = bill.CumulativeMdiPosT4;
            CumulativeMdiPosTl = bill.CumulativeMdiPosTl;

            CumulativeMdiNegT1 = bill.CumulativeMdiNegT1;
            CumulativeMdiNegT2 = bill.CumulativeMdiNegT2;
            CumulativeMdiNegT3 = bill.CumulativeMdiNegT3;
            CumulativeMdiNegT4 = bill.CumulativeMdiNegT4;
            CumulativeMdiNegTl = bill.CumulativeMdiNegTl;

            CumulativeMdiAbsT1 = bill.CumulativeMdiAbsT1;
            CumulativeMdiAbsT2 = bill.CumulativeMdiAbsT2;
            CumulativeMdiAbsT3 = bill.CumulativeMdiAbsT3;
            CumulativeMdiAbsT4 = bill.CumulativeMdiAbsT4;
            CumulativeMdiAbsTl = bill.CumulativeMdiAbsTl;

            IsSynced = bill.IsSynced;

            date = bill.date;
            time = bill.time;
        }

        public void SaveDataToCsv(MonthlyBillingDataSampling data, MeterConfiguration conf)
        {
            try
            {
                string directory = "MeterSamplingData/MBillingData";
                Directory.CreateDirectory(directory);

                string filePath = directory + "/Meter-" + conf.msn + ".csv";

                bool fileExists = File.Exists(filePath);

                using (var writer = new StreamWriter(filePath, true))
                using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = !fileExists }))
                {
                    if (!fileExists)
                    {
                        csv.WriteHeader<MonthlyBillingDataSampling>();
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
                string filePath = "MeterSamplingData/MBillingData/Meter-" + conf.msn + ".csv";

                List<MonthlyBillingDataSampling> dataList;
                if (File.Exists(filePath))
                {
                    using (var reader = new StreamReader(filePath))
                    using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                    {
                        dataList = csv.GetRecords<MonthlyBillingDataSampling>().ToList();
                    }
                }
                else
                {
                    dataList = new List<MonthlyBillingDataSampling>();
                }

                // Keep last 12 monthly snapshots
                DateTime cutoff = DateTime.Now.Date.AddMonths(-12);
                dataList = dataList.Where(item => Convert.ToDateTime(item.date) >= cutoff).ToList();

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

        public static List<MonthlyBillingDataSampling> getData(MeterConfiguration conf, DateTime startDate, DateTime endDate)
        {
            string filePath = "MeterSamplingData/MBillingData/Meter-" + conf.msn + ".csv";

            List<MonthlyBillingDataSampling> dataList;
            if (File.Exists(filePath))
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    dataList = csv.GetRecords<MonthlyBillingDataSampling>().ToList();
                }
            }
            else
            {
                dataList = new List<MonthlyBillingDataSampling>();
            }

            DateTime rangeStartMonth = new DateTime(startDate.Year, startDate.Month, 1);
            DateTime rangeEndMonth = new DateTime(endDate.Year, endDate.Month, 1);

            List<MonthlyBillingDataSampling> filteredRecords = dataList
                .Where(record =>
                {
                    DateTime rec = DateTime.Parse(record.date);
                    DateTime recMonth = new DateTime(rec.Year, rec.Month, 1);
                    return recMonth >= rangeStartMonth && recMonth <= rangeEndMonth;
                })
                .SelectMany(r => Enumerable.Repeat(r, 3))
                .ToList();

            return filteredRecords;
        }

        public string DataInCommand(MeterConfiguration conf)
        {
            DateTime rowDate = DateTime.ParseExact(date + " " + time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            int mdiDay = conf.mdi != null && conf.mdi.mdi_reset_date > 0 ? conf.mdi.mdi_reset_date : 1;
            TimeOnly mdiTime = conf.mdi != null ? conf.mdi.mdi_reset_time : new TimeOnly(0, 0);
            int safeDay = Math.Min(mdiDay, DateTime.DaysInMonth(rowDate.Year, rowDate.Month));
            DateTime _date = new DateTime(rowDate.Year, rowDate.Month, safeDay, mdiTime.Hour, mdiTime.Minute, mdiTime.Second);


            string com = $"02 53 09 0C {Converter.Instance.DateTimeToHex(_date, "03")} 00 FE DA 00 06 {Converter.Instance.ValueToHex(8, ActiveEnergyPosT1)} 06 {Converter.Instance.ValueToHex(8, ActiveEnergyPosT2)} 06 {Converter.Instance.ValueToHex(8, ActiveEnergyPosT3)} 06 {Converter.Instance.ValueToHex(8, ActiveEnergyPosT4)} 06 {Converter.Instance.ValueToHex(8, ActiveEnergyPosTl)} 06 {Converter.Instance.ValueToHex(8, ActiveEnergyNegT1)} 06 {Converter.Instance.ValueToHex(8, ActiveEnergyNegT2)} 06 {Converter.Instance.ValueToHex(8, ActiveEnergyNegT3)} 06 {Converter.Instance.ValueToHex(8, ActiveEnergyNegT4)} 06 {Converter.Instance.ValueToHex(8, ActiveEnergyNegTl)} 06 {Converter.Instance.ValueToHex(8, ActiveEnergyAbsT1)} 06 {Converter.Instance.ValueToHex(8, ActiveEnergyAbsT2)} 06 {Converter.Instance.ValueToHex(8, ActiveEnergyAbsT3)} 06 {Converter.Instance.ValueToHex(8, ActiveEnergyAbsT4)} 06 {Converter.Instance.ValueToHex(8, ActiveEnergyAbsTl)} 06 {Converter.Instance.ValueToHex(8, ReactiveEnergyPosT1)} 06 {Converter.Instance.ValueToHex(8, ReactiveEnergyPosT2)} 06 {Converter.Instance.ValueToHex(8, ReactiveEnergyPosT3)} 06 {Converter.Instance.ValueToHex(8, ReactiveEnergyPosT4)} 06 {Converter.Instance.ValueToHex(8, ReactiveEnergyPosTl)} 06 {Converter.Instance.ValueToHex(8, ReactiveEnergyNegT1)} 06 {Converter.Instance.ValueToHex(8, ReactiveEnergyNegT2)} 06 {Converter.Instance.ValueToHex(8, ReactiveEnergyNegT3)} 06 {Converter.Instance.ValueToHex(8, ReactiveEnergyNegT4)} 06 {Converter.Instance.ValueToHex(8, ReactiveEnergyNegTl)} 06 {Converter.Instance.ValueToHex(8, ReactiveEnergyAbsT1)} 06 {Converter.Instance.ValueToHex(8, ReactiveEnergyAbsT2)} 06 {Converter.Instance.ValueToHex(8, ReactiveEnergyAbsT3)} 06 {Converter.Instance.ValueToHex(8, ReactiveEnergyAbsT4)} 06 {Converter.Instance.ValueToHex(8, ReactiveEnergyAbsTl)} 06 {Converter.Instance.ValueToHex(8, ActiveMdiPosT1)} 06 {Converter.Instance.ValueToHex(8, ActiveMdiPosT2)} 06 {Converter.Instance.ValueToHex(8, ActiveMdiPosT3)} 06 {Converter.Instance.ValueToHex(8, ActiveMdiPosT4)} 06 {Converter.Instance.ValueToHex(8, ActiveMdiPosTl)} 06 {Converter.Instance.ValueToHex(8, ActiveMdiNegT1)} 06 {Converter.Instance.ValueToHex(8, ActiveMdiNegT2)} 06 {Converter.Instance.ValueToHex(8, ActiveMdiNegT3)} 06 {Converter.Instance.ValueToHex(8, ActiveMdiNegT4)} 06 {Converter.Instance.ValueToHex(8, ActiveMdiNegTl)} 06 {Converter.Instance.ValueToHex(8, ActiveMdiAbsT1)} 06 {Converter.Instance.ValueToHex(8, ActiveMdiAbsT2)} 06 {Converter.Instance.ValueToHex(8, ActiveMdiAbsT3)} 06 {Converter.Instance.ValueToHex(8, ActiveMdiAbsT4)} 06 {Converter.Instance.ValueToHex(8, ActiveMdiAbsTl)} 06 {Converter.Instance.ValueToHex(8, CumulativeMdiAbsT1)} 06 {Converter.Instance.ValueToHex(8, CumulativeMdiAbsT2)} 06 {Converter.Instance.ValueToHex(8, CumulativeMdiAbsT3)} 06 {Converter.Instance.ValueToHex(8, CumulativeMdiAbsT4)} 06 {Converter.Instance.ValueToHex(8, CumulativeMdiAbsTl)} 06 {Converter.Instance.ValueToHex(8, CumulativeMdiPosT1)} 06 {Converter.Instance.ValueToHex(8, CumulativeMdiPosT2)} 06 {Converter.Instance.ValueToHex(8, CumulativeMdiPosT3)} 06 {Converter.Instance.ValueToHex(8, CumulativeMdiPosT4)} 06 {Converter.Instance.ValueToHex(8, CumulativeMdiPosTl)} 06 {Converter.Instance.ValueToHex(8, CumulativeMdiNegT1)} 06 {Converter.Instance.ValueToHex(8, CumulativeMdiNegT2)} 06 {Converter.Instance.ValueToHex(8, CumulativeMdiNegT3)} 06 {Converter.Instance.ValueToHex(8, CumulativeMdiNegT4)} 06 {Converter.Instance.ValueToHex(8, CumulativeMdiNegTl)}";



            return com;
        }
    }
}
