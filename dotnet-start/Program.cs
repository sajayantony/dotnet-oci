using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace DotNet.Oras
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                var versionString = Assembly.GetEntryAssembly()
                                        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                        .InformationalVersion
                                        .ToString();

                Console.WriteLine($"run v{versionString}");
                Console.WriteLine("-------------");
                Console.WriteLine("\nUsage:");
                Console.WriteLine("  run <message>");
                return;
            }

            var appDir = DownloadApp(string.Join(' ', args));
            var remaingArgs = args.AsSpan().Slice(1).ToArray();
            EntryPointHandler.RunApp(appDir, remaingArgs);
        }

        static string DownloadApp(string imageReference)
        {
            Console.WriteLine("| Downloading " + imageReference);
            var root = AppStore.Root;
            var appPath = imageReference.Replace(':', '.').Replace('/', '.').ToString();
            var fullPath = Path.Combine(root, appPath);
            var pullArgs = $"pull {imageReference} " ;
            IoUtils.EnsureDirectory(fullPath);

            var psi = new ProcessStartInfo
            {
                FileName = "oras",
                WorkingDirectory = fullPath,
                Arguments = pullArgs,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            using (var proc = Process.Start(psi))
            {
                proc.WaitForExit();
                if (proc.ExitCode != 0)
                {
                    Console.Write($"Error downloading {imageReference}");
                    Environment.Exit(proc.ExitCode);
                }
            }

            return fullPath;
        }

        static int RunApp(string path)
        {

            return 0;
        }
    }
}
