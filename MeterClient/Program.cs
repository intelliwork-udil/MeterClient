using MeterClient;
using Microsoft.Extensions.Configuration;

IConfiguration configuration = new ConfigurationBuilder()
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("appsettings.json")
                        .Build();


MeterClientsGenerator.SetupGlobalConfiguration(configuration);

MeterConfigurationUI meterConfigurationUI = new MeterConfigurationUI();


MeterConfigurationUI.NeedsConnecting = Convert.ToInt32(configuration["MDC_Connections:NeedsConnecting"]);


await meterConfigurationUI.Main();