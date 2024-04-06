using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterClient.Helper
{
    public class ProgressBarHelper
    {
        public static void DrawTextProgressBar(int progress, int current, int total)
        {
            // Draw empty progress bar
            Console.CursorLeft = 0;
            Console.Write("["); // Start
            Console.CursorLeft = 32;
            Console.Write("]"); // End
            Console.CursorLeft = 1;
            float onechunk = 30.0f / total;

            // Draw filled part
            int position = 1;
            for (int i = 0; i < onechunk * progress; i++)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            // Draw unfilled part
            for (int i = position; i <= 31; i++)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            // Draw totals
            Console.CursorLeft = 35;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(progress.ToString() + " of " + total.ToString() + "    "); // Progress

            Console.Write("    " + current.ToString() + "        ");
        }
    }
}
