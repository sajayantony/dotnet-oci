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
            foreach (var s in args)
            {
                Console.WriteLine(s);
            }
            //ShowBot(string.Join(' ', args));
        }



        static void ShowBot(string imageReference)
        {
            Console.WriteLine("Downloading app.." + imageReference);
            var root = AppStore.Root;
            var appPath = imageReference.Replace(':','.').Replace('/','.').ToString();
            var fullPath = Path.Combine(root, appPath);

            IoUtils.EnsureDirectory(fullPath);
            var psi = new ProcessStartInfo
            {
                FileName = "oras",
                WorkingDirectory = fullPath,
                Arguments = $"pull {imageReference}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            using (var proc = Process.Start(psi))
            {
                proc.WaitForExit();
                Console.WriteLine(proc.StandardError.ReadToEnd());
                Console.WriteLine(proc.StandardOutput.ReadToEnd());
                
                if(proc.ExitCode != 0)
                {
                    Console.Write($"Error downloading {imageReference}");
                }
            }
        }
    }
}
