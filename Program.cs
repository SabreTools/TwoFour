using System;
using System.IO;
using System.Linq;

namespace TwoFour
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // If we don't have the right inputs, show help and exit
            if (args.Length != 2)
            {
                Console.WriteLine("Exactly 2 arguments are required");
                DisplayHelp();
                return;
            }

            // Get rebuild mode from first arg
            bool? fourdeep = null;
            if (args[0] == "2" || args[0] == "two" || args[0] == "rvx" || args[0] == "romvaultx" || args[0] == "romroot")
                fourdeep = false;
            else if (args[0] == "4" || args[0] == "four" || args[0] == "romba" || args[0] == "depot")
                fourdeep = true;

            // If neither matched, show help and exit
            if (fourdeep == null)
            {
                Console.WriteLine($"{args[0]} is not a valid flag");
                DisplayHelp();
                return;
            }

            // Get processing folder from second arg
            string directory = args[1];
            try
            {
                // Get the full path for processing
                directory = Path.GetFullPath(directory);
                if (!Directory.Exists(directory))
                    throw new DirectoryNotFoundException();

                // Make sure it ends with a directory separator
                if (!directory.EndsWith("\\"))
                    directory += "\\";
            }
            catch
            {
                Console.WriteLine($"{directory} is not a valid directory");
                DisplayHelp();
                return;
            }

            // Now that we have both, run the processing accordingly
            ProcessFolder(fourdeep.Value, directory);
        }

        /// <summary>
        /// Display the help text
        /// </summary>
        private static void DisplayHelp()
        {
            Console.WriteLine("Usage: TwoFour.exe [mode] [path\\to\\folder]");
            Console.WriteLine();
            Console.WriteLine("Valid modes for 2-deep: 2, two, rvx, romvaultx, romroot");
            Console.WriteLine("Valid modes for 4-deep: 4, four, romba, depot");
        }

        /// <summary>
        /// Process a given directory in the requested way
        /// </summary>
        /// <param name="fourdeep">True to get the 4-deep path, false to get the 2-deep path</param>
        /// <param name="directory">Directory to process</param>
        private static void ProcessFolder(bool fourdeep, string directory)
        {
            Console.WriteLine($"Traversing {directory} and rebuilding as {(fourdeep ? "Romba depot" : "RVX RomRoot")}");
            foreach (string file in Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories))
            {
                Console.WriteLine($"Processing {file}");

                // Get relevant parts of the path
                string oldDir = Path.GetDirectoryName(file);
                string current = oldDir.Substring(directory.Length);
                string filename = Path.GetFileName(file);

                // Make sure subdirectory ends with a directory separator
                if (!current.EndsWith("\\"))
                    current += "\\";

                // Get the expected new subdirectory
                string expected = GetSubdirectory(filename, fourdeep);

                // If we have a match between current and expected, don't do anything
                if (string.Equals(current, expected, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"{file} already in the right directory!");
                    continue;
                }

                // Get the new full path to the output directory
                string subdirectory = Path.Combine(directory, expected);

                // Make sure subdirectory ends with a directory separator
                if (!subdirectory.EndsWith("\\"))
                    subdirectory += "\\";

                // If the directory doesn't exist, create it
                if (!Directory.Exists(subdirectory))
                {
                    try
                    {
                        Console.WriteLine($"Creating {subdirectory}");
                        Directory.CreateDirectory(subdirectory);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An exception occurred while creating {subdirectory}: {ex}");
                        continue;
                    }
                   
                }

                // Now move the file accordingly
                try
                {
                    Console.WriteLine($"Moving {filename}");
                    File.Move(file, Path.Combine(subdirectory, filename));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An exception occurred while moving {filename}: {ex}");
                    continue;
                }

                // If the old path is a subset of the new path and it's empty, remove it
                if (current.Contains(expected) && Directory.EnumerateFiles(oldDir, "*", SearchOption.AllDirectories).Any())
                {
                    try
                    {
                        Console.WriteLine($"Removing {oldDir}");
                        Directory.Delete(oldDir);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An exception occurred while removing {oldDir}: {ex}");
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// Get the expected subdirectory path for a given file path
        /// </summary>
        /// <param name="filename">Filename to get subdirectory for</param>
        /// <param name="fourdeep">True to get the 4-deep path, false to get the 2-deep path</param>
        /// <returns>String representing the subdirectory, null on error</returns>
        private static string GetSubdirectory(string filename, bool fourdeep)
        {
            // If we don't have a long enough filename, return null
            if (fourdeep && filename.Length < 8)
                return null;
            else if (!fourdeep && filename.Length < 4)
                return null;

            // Otherwise, get the proper value
            if (fourdeep)
                return $"{filename.Substring(0, 2)}\\{filename.Substring(2, 2)}\\{filename.Substring(4, 2)}\\{filename.Substring(6, 2)}\\";
            else
                return $"{filename.Substring(0, 2)}\\{filename.Substring(2, 2)}\\";
        }
    }
}
