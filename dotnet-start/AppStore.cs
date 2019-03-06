using System;
using System.IO;

namespace DotNet.Oras 
{
    public class AppStore

    {
        static string  _rootPath;
        public static string  Root => _rootPath;
        // Create the app
        static AppStore()
        {
            var profile = Environment.GetFolderPath( Environment.SpecialFolder.UserProfile);
            var appRoot = System.IO.Path.Combine(profile, ".dotnet-oras-app");
            IoUtils.EnsureDirectory(appRoot);
            _rootPath = appRoot;
        }
    }
}