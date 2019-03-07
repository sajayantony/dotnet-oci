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
        var manifestFilePath = Path.Join(appDirectory, "manifest.json");
        var manifestJson = File.ReadAllText(manifestFilePath);
        var manifest = JsonConvert.DeserializeObject<Manifest>(manifestJson);
        return manifest;
    }


    // Handle console streaming and other things. 
    public static int RunProc(Manifest manifest, string appDirectory, string[] args)
    {
        var entryPoint = Path.Join(Path.GetFullPath(appDirectory), manifest.entrypoint);

        Console.WriteLine("| Starting application.");
        Console.WriteLine("- " + Path.GetRelativePath(AppStore.Root, entryPoint));
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            WorkingDirectory = Environment.CurrentDirectory,
            Arguments = String.Join(' ', entryPoint, args),
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