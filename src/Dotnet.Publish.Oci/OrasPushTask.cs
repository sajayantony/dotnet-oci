using Microsoft.Build.Framework;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MSBuildTasks
{
    public class OrasPush : Microsoft.Build.Utilities.Task
    {
        [Required]
        public string OrasExe { get; set; }

        [Required]
        public string ImageName { get; set; }

        [Required]
        public string PublishDir { get; set; }

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, ImageName);
            Log.LogMessage(MessageImportance.High, PublishDir);

            var imgRef = GetRepository();

            var psi = new ProcessStartInfo(fileName: OrasExe,
                                           arguments: $"-p {PublishDir} -h {imgRef.Registry} -r {imgRef.Repository} -t {imgRef.Tag}");

            psi.RedirectStandardOutput = true;
            using (var proc = Process.Start(psi))
            {
                proc.WaitForExit();
                Log.LogMessage(MessageImportance.High, proc.StandardOutput.ReadToEnd());
            }
            return true;
        }

        ImageRef GetRepository()
        {
            var startIndex = ImageName.IndexOf('/');
            var imageName = ImageName.Trim();
            var endIndex = imageName.LastIndexOf(':');
            var registry = imageName.Substring(0, startIndex);
            var repoName = imageName.Substring(startIndex + 1, endIndex - startIndex - 1);
            var tag = imageName.Substring(endIndex + 1);

            //Log.LogMessage(MessageImportance.High, $"{ImageName} {startIndex} {endIndex} {registry} {repoName} {tag}");
            return new ImageRef
            {
                Registry = registry,
                Repository = repoName,
                Tag = tag
            };

        }
    }
}
