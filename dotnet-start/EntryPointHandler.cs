using System;
using System.Diagnostics;
using System.IO;
using DotNet.Oras;
using Newtonsoft.Json;

public static class EntryPointHandler
{
    public static int RunApp(string appDirectory, string[] args)
    {
        var manifest = GetManifest(appDirectory);
        return RunProc(manifest, appDirectory, args);
    }
    static Manifest GetManifest(string appDirectory)
    {
        var entryFile = System.IO.Directory.GetFiles(
                appDirectory,
                "*.runtimeconfig.json");

        if (entryFile.Length != 1)
        {
            var paths = string.Join(',', entryFile);
            Console.WriteLine($"Found {entryFile.Length} entry point: {paths}");
            Console.WriteLine("Unable to determine entry points.");
            Environment.Exit(1);
        }

        var fileNameWithoutExtension = entryFile[0].Substring(0, entryFile[0].LastIndexOf(".runtimeconfig.json"));
        var dllName = fileNameWithoutExtension + ".dll";

        if (!File.Exists(dllName))
        {
            Console.WriteLine($"ERR: Unable to find {dllName}");
        }

        return new Manifest
        {
            entrypoint = dllName
        };
    }


    // Handle console streaming and other things. 
    public static int RunProc(Manifest manifest, string appDirectory, string[] args)
    {

        Console.WriteLine("| Starting application");
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            WorkingDirectory = Environment.CurrentDirectory,
            Arguments = String.Join(' ', manifest.entrypoint, args),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using (var proc = Process.Start(psi))
        {
            proc.WaitForExit();
            Console.WriteLine(proc.StandardError.ReadToEnd());
            Console.WriteLine(proc.StandardOutput.ReadToEnd());

            return proc.ExitCode;
        }
    }
}