// See https://aka.ms/new-console-template for more information
using MeterClient;
using System;
using System.Reactive.Linq;

Console.WriteLine("Hello, World!");




//var timer = Observable.Interval(TimeSpan.FromMilliseconds(100));

//timer
//    .Select(tick => "DD 04 81 BA A7 B8") // Replace this with your logic to generate the hex command
//    .Timeout(TimeSpan.FromMinutes(3)) // Set a timeout of 3 minutes for each command
//    .Subscribe(
//        hexCommand => MeterConfiguration.Instance.DoSomething(hexCommand),
//        ex => Console.WriteLine($"Timeout occurred: {ex.Message}")
//    );


//Console.ReadKey();






//MeterConfiguration.Instance.loadConfiguration("C:\\Users\\Umair\\Desktop\\1.json");
bool isReceived = false;
string? receivedCommand = null;
string? receivedSecondCommand = null;
while (true)
{
    if (!isReceived)
    {
        receivedCommand = MeterConfiguration.Instance.GetFirstHexCommand("DD 04 81 BA A7 B8");
    }
    if (receivedCommand != null)
    {
        isReceived = true;


        receivedSecondCommand = MeterConfiguration.Instance.GetSecondCommand("00 01 00 30 00 01 00 38 60 36 A1 09 06 07 60 85 74 05 08 01 01 8A 02 07 80 8B 07 60 85 74 05 08 02 01 AC 0A 80 08 31 32 33 34 35 36 37 38 BE 10 04 0E 01 00 00 00 06 5F 1F 04 00 00 7E 1F 04 B0 ");

        break;
    }

    if (!isReceived)
    {
        Thread.Sleep(3000);
    }
}
