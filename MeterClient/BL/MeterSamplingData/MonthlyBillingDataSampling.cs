using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
