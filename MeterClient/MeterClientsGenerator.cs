using ExcelDataReader;
using MeterClient.Helper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MeterClient
{
    /// <summary>
    /// Class responsible for generating and managing meter configurations and client data from an Excel source.
    /// It handles global configuration setup and provides logic to filter and load specific ranges of meters.
    /// </summary>
    public class MeterClientsGenerator
    {
        /// <summary>
        /// A list containing the generated and filtered meter configurations.
        /// </summary>
        public static List<MeterConfiguration> clients = new List<MeterConfiguration>();

        /// <summary>
        /// Global IP address for the MDC connection.
        /// </summary>
        public static string ipAddress = "";

        /// <summary>
        /// The port number for communication.
        /// </summary>
        public static int port = 0;

        /// <summary>
        /// The communication interval for each meter (converted to seconds).
        /// </summary>
        public static int communicationInterval = 0;

        /// <summary>
        /// The file path for the source Excel data.
        /// </summary>
        public static string fileName = "";

        /// <summary>
        /// Sets up the global configuration for the generator based on the provided <see cref="IConfiguration"/>.
        /// </summary>
        /// <param name="configuration">The configuration object holding the connection parameters.</param>
        public static void SetupGlobalConfiguration(IConfiguration configuration)
        {
            MeterClientsGenerator.ipAddress = configuration["MDC_Connections:ipAddress"];
            MeterClientsGenerator.port = Convert.ToInt32(configuration["MDC_Connections:port"]);
            MeterClientsGenerator.communicationInterval = Convert.ToInt32(configuration["MDC_Connections:communicationInterval"]) * 60;
            MeterClientsGenerator.fileName = configuration["MDC_Connections:filePath"];
        }



        /// <summary>
        /// This method starts reading the Excel file provided in the global configuration, 
        /// generates client configurations, saves them as JSON, and filters them based on user input.
        /// </summary>
        public void generateClients()
        {

            var _clients = new List<MeterConfiguration>();

            //int count = 0;

            Console.WriteLine("Wait Loading Meters");

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

                        int totalRows = dataTable.Rows.Count - 1;

                        // Display the data
                        for (int i = 1; i <= totalRows; i++)
                        {
                            int progress = (i * 100) / totalRows;

                            // Update progress bar
                            ProgressBarHelper.DrawTextProgressBar(progress, i, 100);

                            DataRow row = dataTable.Rows[i];
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

                            client.dmdt.communication_type = 1;

                            //client.saveConfiguration("MeterConfigs/" + client.msn + ".json");

                            _clients.Add(client);
                            //count++;

                            //if (count == num)
                            //{
                            //    break;
                            //}
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"File not found: {fileName}");
            }

            Console.WriteLine("");

            Console.WriteLine($"{_clients.Count} Meters Loaded");

            int maxLength = _clients.Max(obj => obj.msn.Length);


            // Initialize a substring with the maximum length
            char[] substring = new char[maxLength];
            int startIndex = 0;
            // Iterate through each character index
            for (int i = 0; i < maxLength; i++)
            {
                char? currentChar = null;

                // Check each msn value for the character at index i
                foreach (var obj in _clients)
                {
                    if (i < obj.msn.Length)
                    {
                        if (currentChar == null)
                        {
                            currentChar = obj.msn[i];
                        }
                        else if (currentChar != obj.msn[i])
                        {
                            currentChar = null;
                            break;
                        }
                    }
                }

                // If the character is repeating at index i, add it to the substring
                if (currentChar.HasValue)
                {
                    substring[i] = currentChar.Value;

                    startIndex = i + 1;
                }
                else
                {
                    break; // Exit the loop if no character is repeating at index i
                }
            }

            // Convert the char array to a string and remove any trailing null characters
            string commandSubstring = new string(substring).TrimEnd('\0');


            var commonStr = commandSubstring.Length;

            string firstVal = _clients.First().msn.Substring(startIndex);
            string lastVal = _clients.Last().msn.Substring(startIndex);

            Console.WriteLine("Enter number of clients to Run");
            Console.WriteLine($"Starting: {firstVal}, Ending: {lastVal}");


            int firstMSN = -1;

            while (firstMSN < Convert.ToInt32(firstVal))
            {
                try
                {
                    Console.WriteLine($"Give First Value");
                    firstMSN = Convert.ToInt32(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Enter Valid Value");
                }

            }


            int lastMSN = -2;

            while (lastMSN <= firstMSN)
            {
                try
                {
                    Console.WriteLine($"Give Last Value");
                    lastMSN = Convert.ToInt32(Console.ReadLine());

                    if (lastMSN <= Convert.ToInt32(lastVal) && lastMSN >= firstMSN)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Enter Valid Value");
                        lastMSN = -1;
                    }
                }
                catch
                {
                    lastMSN = -1;
                    Console.WriteLine("Enter Valid Value");
                }
            }
            string FMSn = "";

            if ((commandSubstring + firstMSN.ToString()).Length < 10)
            {
                int len = 10 - (commandSubstring + firstMSN.ToString()).Length;
                string str = "";
                for (int j = 0; j < len; j++)
                {
                    str += "0";
                }


                FMSn = commandSubstring + str + firstMSN.ToString();
            }
            else
            {
                FMSn = commandSubstring + firstMSN.ToString();
            }


            string LMSn = "";

            if ((commandSubstring + lastMSN.ToString()).Length < 10)
            {
                int len = 10 - (commandSubstring + lastMSN.ToString()).Length;
                string str = "";
                for (int j = 0; j < len; j++)
                {
                    str += "0";
                }


                LMSn = commandSubstring + str + lastMSN.ToString();
            }
            else
            {
                LMSn = commandSubstring + lastMSN.ToString();
            }

            var filteredList = _clients.Where(obj => string.Compare(obj.msn, FMSn) >= 0 && string.Compare(obj.msn, LMSn) <= 0).ToList();


            //clients = new List<MeterConfiguration>();
            //clients.Add(_clients[0]);


            clients = filteredList;

        }
    }
}
