using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterClient.BL
{
    public class DayProfile
    {
        public string name { get; set; }
        public List<TimeOnly> tariff_slabs { get; set; }

        public DayProfile()
        {
            tariff_slabs = new List<TimeOnly>();
        }

        public DayProfile(string name, List<TimeOnly> tariff_slabs)
        {
            this.name = name;
            this.tariff_slabs = tariff_slabs;
        }
    }

    public class WeekProfile
    {
        public string name { get; set; }
        public List<DayProfile> day_profile { get; set; }

        public WeekProfile()
        {
            day_profile = new List<DayProfile>();
        }

        public WeekProfile(string name, List<DayProfile> day_profile)
        {
            this.name = name;
            this.day_profile = day_profile;
        }
    }

    public class SeasonProfile
    {
        public string name { get; set; }
        public string week_profile_name { get; set; }
        public DateOnly start_date { get; set; }
    }

    public class HolidayProfile
    {
        public string name { get; set; }
        public string day_profile_name { get; set; }
        public DateOnly date { get; set; }
    }

    public class TimeOfUse
    {
        public List<DayProfile> day_profile { get; set; }
        public List<WeekProfile> week_profile { get; set; }

        public List<SeasonProfile> season_profile { get; set; }
        public List<HolidayProfile> holiday_profile { get; set; }

        public DateTime activation_datetime { get; set; }
        public DateTime request_datetime { get; set; }


        public TimeOfUse()
        {
            day_profile = new List<DayProfile>();
            week_profile = new List<WeekProfile>();
            season_profile = new List<SeasonProfile>();
            holiday_profile = new List<HolidayProfile>();
        }

        public string ProcessCommand(string re)
        {
            string command = "";

            if (re.Contains("C0 01 81 00 14 00 00 0D 00 00 FF 0A 00"))
            {
                string ayear = activation_datetime.Year.ToString("X").PadLeft(4, '0');
                string amonth = activation_datetime.Month.ToString("X").PadLeft(2, '0');
                string aday = activation_datetime.Day.ToString("X").PadLeft(2, '0');

                string ahour = activation_datetime.Hour.ToString("X").PadLeft(2, '0');
                string aminute = activation_datetime.Minute.ToString("X").PadLeft(2, '0');
                string asecond = activation_datetime.Second.ToString("X").PadLeft(2, '0');

                ayear = MeterConfigurationUI.AddSpaceEveryNCharacters(ayear, 2);
                amonth = MeterConfigurationUI.AddSpaceEveryNCharacters(amonth, 2);
                aday = MeterConfigurationUI.AddSpaceEveryNCharacters(aday, 2);

                ahour = MeterConfigurationUI.AddSpaceEveryNCharacters(ahour, 2);
                aminute = MeterConfigurationUI.AddSpaceEveryNCharacters(aminute, 2);
                asecond = MeterConfigurationUI.AddSpaceEveryNCharacters(asecond, 2);



                string act_date_time = ayear + " " + amonth + " " + aday + " " + ahour + " " + aminute + " " + asecond;
                command = "C4 01 81 00 09 0C " + act_date_time + " 00 00 00 00 00";
            }

            return command;
        }
    }
}
