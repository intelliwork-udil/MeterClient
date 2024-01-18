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
        MDSM,
        AUXR,
        WSIM,
        IPPO,
        OPPO,
        TIOU,
        TIME_SYNCHRONIZATION,
        SANCTIONED_LOAD_CONTROL,
        LOAD_SHEDDING_SCHEDULLING,
        DMDT,
        MDI,
        INST_DATA_READ,
        BILL_DATA_READ,
        LPRO_DATA_READ,
        Nothing
    }
    public static class CommandClassifier
    {
        public static CommandType commandType(string command)
        {
            if (command == null)
            {
                return CommandType.Nothing;
            }
            else if (command.Contains("00 01 00 30 00 01 00 38 60 36 A1 09 06 07 60 85 74 05 08 01 01 8A 02 07 80 8B 07 60 85 74 05 08 02 01 AC 0A 80 08") && command.Contains("BE 10 04 0E 01 00 00 00 06 5F 1F 04 00 00 7E 1F 04 B0"))
            {
                return CommandType.AARQ;
            }
            else if (command.Contains("C1 01 81 00 01 00 00") || command.Contains("C1 01 81 00 16 00 00 0F 00 00 FF 04 00 01 02 02 04 09 04"))
            {
                return CommandType.DeviceCreation;
            }
            else if (command.Contains("C0 01 81 00 07 01 00 63 01 01 FF 02 01 01 02 04 02 04 12 00 08 09 06 00 00 01 00 00 FF 0F 02 12 00 00 09 0C"))
            {
                return CommandType.INST_DATA_READ;
            }
            else if (command.Contains("C0 01 81 00 07 01 00 63 01 00 FF 02 01 01 02 04 02 04 12 00 08 09 06 00 00 01 00 00 FF 0F 02 12 00 00 09 0C"))
            {
                return CommandType.LPRO_DATA_READ;
            }
            else if (command.Contains("C0 01 81 00 07 01 00 63 02 00 FF 02 01 01 02 04 02 04 12 00 08 09 06 00 00 01 00 00 FF 0F 02 12 00 00 09 0C"))
            {
                return CommandType.BILL_DATA_READ;
            }
            else if (command.Contains("C0 01 81 00 07 01 00 ") || command.Contains("C0 01 81 00 03 00 00 5E 5C 28 FF 02 00") || command.Contains("C0 01 81 00 01 00 00 5E 5C 2C FF 02 00"))
            {
                return CommandType.MDSM;
            }
            else if (command.Contains("C0 01 81 00 46 00 00 60 03 0A FF 02 00") || command.Contains("C0 01 81 00 46 00 00 60 03 0A FF 03 00"))
            {
                return CommandType.AUXR;
            }
            else if (command.Contains("00 01 00 30 00 01 00 0D C0 01 81 00 01 00 00 60 0C 80 FF 02 00") || command.Contains("00 01 00 30 00 01 00 0D C0 01 81 00 01 00 00 60 0C 81 FF 02 00") || command.Contains("00 01 00 30 00 01 00 0D C0 01 81 00 01 00 00 60 0C 82 FF 02 00"))
            {
                return CommandType.WSIM;
            }
            else if (command.Contains("C0 01 81 00 01 00 00 19 04 80 FF 02 00"))
            {
                return CommandType.IPPO;
            }
            else if (command.Contains("C0 01 81 00 01 00 00 60 3C 10 FF 02 00"))
            {
                return CommandType.OPPO;
            }
            else if (command.Contains("C0 01 81 00 14 00 00 0D 00 00 FF 0A 00"))
            {
                return CommandType.TIOU;
            }
            else if (command.Contains("00 01 00 30 00 01 00 0D C0 01 81 00 01 01 00 00 09 02 FF 02 00") || command.Contains("00 01 00 30 00 01 00 0D C0 01 81 00 01 01 00 00 07 01 FF 02 00") || command.Contains("00 01 00 30 00 01 00 1B C1 01 81 00 08 00 00 01 00 00 FF 02 00 09 0C"))
            {
                return CommandType.TIME_SYNCHRONIZATION;
            }
            else if (command.Contains("C1 01 81 00 01 00 00 60 3C 0A FF 02 00 03") || command.Contains("C1 01 81 00 03 01 00 0F 23 00 00 02 00 06") || command.Contains("C1 01 81 00 03 01 00 0F 2C 00 00 02 00 12") || command.Contains("C1 01 81 00 03 01 00 0F 2C 00 02 02 00 12 00"))
            {
                //5,6,7,
                return CommandType.SANCTIONED_LOAD_CONTROL;
            }
            else if (command.Contains(""))
            {
                return CommandType.LOAD_SHEDDING_SCHEDULLING;
            }
            else if (command.Contains("C0 01 81 00 16 00 00 0F 00 00 FF 04 00"))
            {
                return CommandType.MDI;
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
