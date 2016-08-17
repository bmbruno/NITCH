using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitch.Infrastructure.Helpers
{
    public static class FileHelper
    {
        /// <summary>
        /// Returns the directory where the application is currently running.
        /// </summary>
        /// <returns>String of path.</returns>
        public static string GetCurrentApplicationDirectory()
        {
            string appLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            return System.IO.Path.GetDirectoryName(appLocation);
        }

        /// <summary>
        /// If found, removes a leading slash from a path. This allows it to be combined properly when using System.IO.Path.Combine.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string FormatForPathCombining(string filePath)
        {
            if (filePath.StartsWith("/") || filePath.StartsWith(@"\"))
                return filePath.Remove(0, 1);

            return filePath;
        }

        /// <summary>
        /// Copy all files and subdirectories to a new location. Excludes "master_" files and (optionally an output folder). Recursive.
        /// Reference: http://stackoverflow.com/questions/58744/best-way-to-copy-the-entire-contents-of-a-directory-in-c-sharp
        /// </summary>
        /// <param name="source">Source directory.</param>
        /// <param name="target">Target directory location.</param>
        /// <param name="masterName">Pattern of master filenames.</param>
        /// <param name="ignoreFolder">Folder to ignore when copying.</param>
        public static void CopyAll(DirectoryInfo source, DirectoryInfo target, string masterFilePattern, string ignoreFolder = null)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                if (!fi.Name.StartsWith(masterFilePattern))
                    fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories().Where(u => u.Name != ignoreFolder))
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir, masterFilePattern, ignoreFolder);
            }
        }

        public static string[] RemoveMasterFilesFromList(string[] allFiles)
        {
            List<string> sourceFiles = new List<string>();

            foreach (string file in allFiles)
            {
                string fileName = Path.GetFileName(file);

                if (!fileName.StartsWith("master_", StringComparison.OrdinalIgnoreCase))
                    sourceFiles.Add(file);
            }

            return sourceFiles.ToArray();
        }
    }
}
