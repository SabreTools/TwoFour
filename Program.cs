using System;

namespace TwoFour
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // If we don't have the right number of inputs, show help and exit
            if (args.Length < 2)
            {
                Console.WriteLine("At least 2 arguments are required");
                DisplayHelp();
                return;
            }

            // Get rebuild depth from first arg
            int depth;
            if (args[0] == "rvx" || args[0] == "romvaultx" || args[0] == "romroot")
                depth = 2;
            else if (args[0] == "romba" || args[0] == "depot")
                depth = 4;
            else if (!int.TryParse(args[0], out depth))
                depth = -1;

            // If neither matched, show help and exit
            if (depth < 0)
            {
                Console.WriteLine($"{args[0]} is not a valid depth");
                DisplayHelp();
                return;
            }

            // Process each directory from remaining args
            for (int i = 1; i < args.Length; i++)
            {
                Processor processor = new Processor(args[i], depth);
                if (!processor.ProcessFolder())
                {
                    DisplayHelp();
                    return;
                }
            }
        }

        /// <summary>
        /// Display the help text
        /// </summary>
        private static void DisplayHelp()
        {
            Console.WriteLine("Usage: TwoFour.exe [mode] [path\\to\\folder] ...");
            Console.WriteLine();
            Console.WriteLine("Special modes for 2-deep: rvx, romvaultx, romroot");
            Console.WriteLine("Special modes for 4-deep: romba, depot");
            Console.WriteLine("All other positive numbers are allowed");
        }
    }
}
