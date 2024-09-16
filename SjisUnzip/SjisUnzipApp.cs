using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.IO.Compression;

namespace SjisUnzip
{
	public class SjisUnzipApp
	{
		private readonly Encoding sjisEncoding = Encoding.GetEncoding(932);

		public void Main(string[] args)
		{
			var recode = args.Any((s) => s.Equals("-r"));
			var codePageOption = args.Any((s) => s.Equals("-c"));

			if (recode && !codePageOption && args.Length == 2)
			{
				args = args.Where((arg) => arg != "-r").ToArray();
				if (args.Length > 0 && File.Exists(args[0]))
				{
					recodeFile(args[0], sjisEncoding);
				}
			}
			// CURRENTLY DOESN"T WORK
			/*else if (args.Length == 1 && Directory.Exists(args[0]))
			{
				recodeCorruptFilenames(args[0], true, sjisEncoding);
			}*/
			else if (!codePageOption && args.Length == 1 && File.Exists(args[0]) && args[0].EndsWith(".zip", true, CultureInfo.CurrentCulture))
			{
				extractSjisZip(args[0], "./", sjisEncoding);
			}
			else if (!codePageOption && args.Length == 2 && File.Exists(args[0]) && args[0].EndsWith(".zip", true, CultureInfo.CurrentCulture))
			{
				var folderPath = Path.GetDirectoryName(args[0]);
				var newFolderPath = Path.Combine(folderPath, args[1]);
				Directory.CreateDirectory(newFolderPath);
				extractSjisZip(args[0], newFolderPath, sjisEncoding);
			}
			else if (recode && codePageOption && args.Length == 4)
			{
				args = args.Where((arg) => arg != "-r").ToArray();
				if (args.Length > 2)
				{
					var ci = Array.IndexOf(args, "-c");
					try
					{
						var codePage = int.Parse(args[ci + 1]);
						args = args.Where((arg, i) => i != ci && i != ci + 1).ToArray();
                        if (args.Length > 0 && File.Exists(args[0]))
                        {
                            recodeFile(args[0], Encoding.GetEncoding(codePage));
							return;
                        }
                    }
					catch (Exception)
					{
						printUsage();
						return;
					}
				}
				printUsage();
				return;
			}
			else if (codePageOption && args.Length == 3)
			{
                var ci = Array.IndexOf(args, "-c");
                try
                {
                    var codePage = int.Parse(args[ci + 1]);
                    args = args.Where((arg, i) => i != ci && i != ci + 1).ToArray();
                    if (args.Length > 0 && File.Exists(args[0]) && args[0].EndsWith(".zip", true, CultureInfo.CurrentCulture))
                    {
                        extractSjisZip(args[0], "./", Encoding.GetEncoding(codePage));
                        return;
                    }
                }
                catch (Exception)
                {
                    printUsage();
                    return;
                }
                printUsage();
                return;
            }
			else if (codePageOption && args.Length == 4)
			{
                var ci = Array.IndexOf(args, "-c");
                try
                {
                    var codePage = int.Parse(args[ci + 1]);
                    args = args.Where((arg, i) => i != ci && i != ci + 1).ToArray();
                    if (args.Length > 1 && File.Exists(args[0]) && args[0].EndsWith(".zip", true, CultureInfo.CurrentCulture))
                    {
                        var folderPath = Path.GetDirectoryName(args[0]);
                        var newFolderPath = Path.Combine(folderPath, args[1]);
                        Directory.CreateDirectory(newFolderPath);
                        extractSjisZip(args[0], newFolderPath, Encoding.GetEncoding(codePage));
                        return;
                    }
                }
                catch (Exception)
                {
                    printUsage();
                    return;
                }
                printUsage();
                return;
			}
			else
			{
				printUsage();
			}
		}

		static void printUsage()
		{
			"Usage: .\\sjisunzip.exe [-c <Codepage Number>] someFile.zip [toFolder]".wl();
			"    -c: Sets the codepage the extracter will use (default 932). Common ones are 932 (JP), 936 (CN), and 1252 (EN)".wl();
			"Usage: .\\sjisunzip.exe -r [-c <Codepage Number>] someFile.zip".wl();
			"    -r: Recode file to {filename}_utf8.zip".wl();
			"    -c: Sets the codepage the extracter will use (default 932). Common ones are 932 (JP), 936 (CN), and 1252 (EN)".wl();
			//"Usage: sjisunzip ./some_folder_with_corrupt_filenames".wl();)
			"Examples:".wl();
			"    .\\sjisunzip.exe aFile.zip".wl();
			"    .\\sjisunzip.exe aFile.zip MyNewFolder".wl();
            "    .\\sjisunzip.exe -c 936 aFile.zip".wl();
            "    .\\sjisunzip.exe -c 936 aFile.zip MyNewFolder".wl();
			"    .\\sjisunzip.exe -r aFile.zip".wl();
			"    .\\sjisunzip.exe -r -c 936 aFile.zip".wl();
		}

		private void extractSjisZip(string fileName, string toFolder, Encoding encoding)
		{
			"Writing to folder {0}...".wl(toFolder);

			using (var zipFile = new ZipArchive(new FileStream(fileName, FileMode.Open, FileAccess.Read), 
				ZipArchiveMode.Read, false, encoding))
			{
				zipFile.ExtractToDirectory(toFolder);
			}

			"Done.".wl();
		}

		private void recodeFile(string srcFile, Encoding encoding)
		{
			var zipFile = new ZipArchive(new FileStream(srcFile, FileMode.Open), ZipArchiveMode.Read, false, encoding);
			var newFilePath = Path.Combine(Path.GetDirectoryName(srcFile), Path.GetFileNameWithoutExtension(srcFile) + "_utf8.zip");

			using (var newZip = new ZipArchive(new FileStream(newFilePath, FileMode.CreateNew), ZipArchiveMode.Create, false, Encoding.UTF8))
			{
				foreach (var zipEntry in zipFile.Entries)
				{
					var newFile = newZip.CreateEntry(zipEntry.FullName, CompressionLevel.Fastest);

					newFile.LastWriteTime = zipEntry.LastWriteTime;

					using (Stream newStream = newFile.Open(), oldStream = zipEntry.Open())
					{
						"Moved {0}".wl(newFile.FullName);
						oldStream.CopyTo(newStream);
					}
				}
			}

			"Finished recoding {0} entries.".wl(zipFile.Entries.Count);
        }

		readonly Func<char, bool> dirSeparatorComparator = c => c == Path.DirectorySeparatorChar;

		private void recodeCorruptFilenames(string directoryPath, bool recurse, Encoding encoding)
		{
			var rootDir = new DirectoryInfo(directoryPath);
			var dirs = rootDir.GetDirectories("*", recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToList();
			var files = rootDir.GetFiles("*", recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

			files.Where(fi => fi.Name.ContainsNonAscii() && !fi.Name.ContainsJapanese())
				.ToList()
				.ForEach(
					fi => fi.Rename(fi.Name.DecodeMojibake(encoding))
				);
			
			// Sort reversed based on directory depth, rename deepest first and this won't rename a root before a leaf.
			dirs.Sort((d2, d1) => d1.FullName.Count(dirSeparatorComparator).CompareTo(d2.FullName.Count(dirSeparatorComparator)));

			dirs.Where(di => di.Name.ContainsNonAscii() && !di.Name.ContainsJapanese())
				.ToList()
				.ForEach(
					di => di.Rename(di.Name.DecodeMojibake(encoding))
				);

			if (rootDir.Name.ContainsNonAscii() && !rootDir.Name.ContainsJapanese())
			{
				rootDir.Rename(rootDir.Name.DecodeMojibake(encoding));
			}
		}
	}
}
