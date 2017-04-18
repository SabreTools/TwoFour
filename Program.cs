using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TwoFour
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Getting list of all files");
			List<string> files = Directory.EnumerateFiles(Environment.CurrentDirectory, "*", SearchOption.AllDirectories).ToList();

			Console.WriteLine("Moving all files to main folder");
			foreach (string file in files)
			{
				try
				{
					File.Move(file, Path.GetFileName(file));
				}
				catch { }
			}

			Console.WriteLine("Clean up all old directories");
			List<string> olddirs = Directory.EnumerateDirectories(Environment.CurrentDirectory, "*", SearchOption.TopDirectoryOnly).ToList();
			foreach (string dir in olddirs)
			{
				try
				{
					Directory.Delete(dir, true);
				}
				catch { }
			}

			Console.WriteLine("Get the new list of files from the current directory");
			files = Directory.EnumerateFiles(Environment.CurrentDirectory, "*", SearchOption.TopDirectoryOnly).ToList();
			Console.WriteLine("files.Count: " + files.Count);

			Console.WriteLine("Now naively sort based on the current asked type: " + (args.Length > 0 ? args[0] : "4"));
			foreach (string file in files)
			{
				string newdir = "";
				string filename = Path.GetFileName(file);

				Console.WriteLine("file: " + filename);
				if (filename == "TwoFour.exe" || filename.Contains(".romba"))
				{
					continue;
				}

				if (args.Length > 0 && args[0] == "2")
				{
					newdir = Path.Combine(filename.Substring(0, 2), filename.Substring(2, 2));
				}
				else
				{
					newdir = Path.Combine(filename.Substring(0, 2), filename.Substring(2, 2), filename.Substring(4, 2), filename.Substring(6, 2));
				}

				Console.WriteLine("newdir: " + newdir);

				if (newdir != "")
				{
					try
					{
						if (!Directory.Exists(newdir))
						{
							Directory.CreateDirectory(newdir);
						}
						File.Move(file, Path.Combine(newdir, filename));
					}
					catch { }
				}
			}
		}
	}
}
