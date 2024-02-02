using MeterClient;
using Microsoft.Extensions.Configuration;

IConfiguration configuration = new ConfigurationBuilder()
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("appsettings.json")
                        .Build();

MeterClientsGenerator.ipAddress = configuration["MDC_Connections:ipAddress"];
MeterClientsGenerator.port = Convert.ToInt32(configuration["MDC_Connections:port"]);
MeterClientsGenerator.communicationInterval = Convert.ToInt32(configuration["MDC_Connections:communicationInterval"]) * 60;
MeterClientsGenerator.fileName = configuration["MDC_Connections:filePath"];


MeterConfigurationUI meterConfigurationUI = new MeterConfigurationUI();

meterConfigurationUI.GenerateFolders("MeterConfigs");
meterConfigurationUI.GenerateFolders("MeterSamplingData");

// Sub folders
meterConfigurationUI.GenerateFolders(Path.Combine("MeterSamplingData", "BillingData"));
meterConfigurationUI.GenerateFolders(Path.Combine("MeterSamplingData", "InstanteneousData"));
meterConfigurationUI.GenerateFolders(Path.Combine("MeterSamplingData", "LPROData"));



await meterConfigurationUI.Main();