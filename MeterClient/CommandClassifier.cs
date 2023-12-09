using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterClient
{
    public enum CommandType
    {
        AARQ,
        DeviceCreation,
        DMDT,
        Nothing
    }
    public static class CommandClassifier
    {
        public static CommandType commandType(string command)
        {
            if (command.Contains("00 01 00 30 00 01 00 38 60 36 A1 09 06 07 60 85 74 05 08 01 01 8A 02 07 80 8B 07 60 85 74 05 08 02 01 AC 0A 80 08") && command.Contains("BE 10 04 0E 01 00 00 00 06 5F 1F 04 00 00 7E 1F04B0"))
            {
                return CommandType.AARQ;
            }
            else if (command.Contains("C1 01 81 00 01 00 00 60 3C 06 FF 02 00 02 02 09 03"))
            {
                return CommandType.DeviceCreation;
            }
            else if (command.Contains("C0 01 81 00 01 00 00"))
            {
                return CommandType.DMDT;
            }
            else
            {
                return CommandType.Nothing;
            }
        }


    }
}
