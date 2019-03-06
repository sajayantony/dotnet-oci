using System.IO;

namespace DotNet.Oras  
{
    public static class IoUtils
    {
        static IoUtils(){
        }
        public static void EnsureDirectory(string directory)
        {
            if(!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}