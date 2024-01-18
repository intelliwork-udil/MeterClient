using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterClient.Helper
{
    public class Converter
    {
        private static Converter instance;

        private Converter()
        {

        }


        public static Converter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Converter();
                }
                return instance;
            }
        }


        public string ValueToHex(int length, double? value = null, int? intVal = null)
        {
            // Step 1: Convert the double or int to a byte array
            byte[] bytes = null;
            if (value != null)
            {
                bytes = BitConverter.GetBytes((double)value);
            }
            else
            {
                bytes = BitConverter.GetBytes((int)intVal);
            }

            // Step 2: Convert each byte to a hexadecimal string
            string hexString = BitConverter.ToString(bytes).Replace("-", "");

            // Step 3: Process the string based on the specified length
            hexString = ProcessString(hexString, length);

            // Step 4: Insert a space after every 2 characters
            string spacedHexString = InsertSpaces(hexString, 2);

            return spacedHexString;
        }

        // Method to process the input string based on the specified length
        private string ProcessString(string inputStr, int length)
        {
            // Extract the last 'length' characters
            string lastChars = inputStr.Length >= length ? inputStr.Substring(inputStr.Length - length) : inputStr;

            // Check if the length is less than 'length'
            if (inputStr.Length < length)
            {
                // Add remaining 0s at the start
                return new string('0', length - inputStr.Length) + lastChars;
            }
            else
            {
                return lastChars;
            }
        }

        // Method to insert spaces in a string at specified intervals
        public string InsertSpaces(string str, int interval)
        {
            return string.Join(" ", SplitByInterval(str, interval));
        }

        // Method to split a string into segments based on a specified interval
        public string[] SplitByInterval(string str, int interval)
        {
            int length = (int)Math.Ceiling((double)str.Length / interval);
            string[] result = new string[length];

            for (int i = 0; i < length; i++)
            {
                int startIndex = i * interval;
                int endIndex = Math.Min(startIndex + interval, str.Length);
                result[i] = str.Substring(startIndex, endIndex - startIndex);
            }

            return result;
        }

        // Method to convert DateTime to a formatted hexadecimal string
        public string DateTimeToHex(DateTime dateTime, string separatorStr)
        {
            // Convert each component of the DateTime to hexadecimal
            string hexYear = dateTime.Year.ToString("X4");
            hexYear = hexYear.Substring(0, 2) + " " + hexYear.Substring(2);
            string hexMonth = dateTime.Month.ToString("X2");
            string hexDay = dateTime.Day.ToString("X2");
            string hexHour = dateTime.Hour.ToString("X2");
            string hexMinute = dateTime.Minute.ToString("X2");
            string hexSecond = dateTime.Second.ToString("X2");

            // Concatenate the hex components with separators
            string hexDateTimeString = hexYear + " " + hexMonth + " " + hexDay + " " + separatorStr + " " + hexHour + " " + hexMinute + " " + hexSecond;

            return hexDateTimeString;
        }
        public int ConvertToYear(int firstNumber, int secondNumber)
        {
            // Convert the second number to hexadecimal
            string hexRepresentation = secondNumber.ToString("X");

            // Combine the first number and the hexadecimal representation
            string hexCombined = $"{firstNumber:X2}{hexRepresentation}";

            // Convert the combined hexadecimal representation back to decimal
            int originalYear = Convert.ToInt32(hexCombined, 16);

            return originalYear;
        }

    }
}
