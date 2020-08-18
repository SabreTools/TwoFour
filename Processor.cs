using System;
using System.IO;
using System.Linq;

namespace TwoFour
{
    /// <summary>
    /// Depth changing processor
    /// </summary>
    public class Processor
    {
        /// <summary>
        /// Directory to process
        /// </summary>
        private string directory;

        /// <summary>
        /// Number of bytes deep the folder should be rebuilt to
        /// </summary>
        private int depth;

        /// <summary>
        /// Create a new processor
        /// </summary>
        /// <param name="directory">Directory to process</param>
        /// <param name="depth">Byte depth</param>
        public Processor(string directory, int depth)
        {
            this.directory = directory;
            this.depth = depth;
        }

        /// <summary>
        /// Process a given directory in the requested way
        /// </summary>
        /// <returns>True if the folder was processed, false on unrecoverable error</returns>
        public bool ProcessFolder()
        {
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
                return false;
            }

            Console.WriteLine($"Traversing {directory} and rebuilding to {depth}-deep folders");
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
                string expected = GetSubdirectory(filename);

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

            return true;
        }

        /// <summary>
        /// Get the expected subdirectory path for a given file path
        /// </summary>
        /// <param name="filename">Filename to get subdirectory for</param>
        /// <returns>String representing the subdirectory, null on error</returns>
        private string GetSubdirectory(string filename)
        {
            // If we don't have a long enough filename, return null
            if (filename.Length < 2 * depth)
                return null;

            // Otherwise, get the proper value
            string path = string.Empty;
            for (int i = 0; i < depth; i++)
            {
                path += $"{filename.Substring(i * 2, 2)}\\";
            }

            return path;
        }
    }
}
