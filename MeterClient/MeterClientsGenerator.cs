using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MeterClient
{
    public class MeterClientsGenerator
    {
        public static List<MeterConfiguration> clients = new List<MeterConfiguration>();

        private string ipAddress = "115.42.79.151";
        private int port = 32220;
        private int communicationInterval = 24 * 60;


        public void generateClients()
        {
            Console.WriteLine("Enter number of clients to generate");
            int num = Convert.ToInt32(Console.ReadLine());

            string fileName = "meter.xlsx";

            clients = new List<MeterConfiguration>();

            int count = 0;

            if (File.Exists(fileName))
            {
                // Read the Excel file using ExcelDataReader
                using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
                {

                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    var encoding = Encoding.GetEncoding(1252);

                    // Create configuration with encoding
                    var readerConfig = new ExcelReaderConfiguration
                    {
                        FallbackEncoding = encoding
                    };

                    using (var reader = ExcelReaderFactory.CreateReader(stream, readerConfig))
                    {
                        // Choose the first worksheet
                        reader.Read();

                        // Create a DataSet to hold the result
                        DataSet result = reader.AsDataSet();

                        // Assuming there is only one DataTable in the DataSet
                        DataTable dataTable = result.Tables[0];

                        // Display the data
                        foreach (DataRow row in dataTable.Rows)
                        {
                            // Assuming First Row is the Header Row
                            if (row == dataTable.Rows[0])
                            {
                                continue;
                            }
                            string meterName = row[1].ToString();
                            string meterPassword = row[2].ToString();

                            // Create a client

                            MeterConfiguration client = new MeterConfiguration();
                            client.msn = meterName;
                            client.password = meterPassword;


                            client.ippo.primary_ip_address = ipAddress;
                            client.ippo.primary_port = port;

                            client.dmdt.communication_interval = communicationInterval;


                            client.saveConfiguration("MeterConfigs/" + client.msn + ".json");

                            clients.Add(client);
                            count++;

                            if (count == num)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"File not found: {fileName}");
            }


        }
    }
}
