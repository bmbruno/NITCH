using Nitch.Infrastructure.Enumerations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitch
{
    /// <summary>
    /// Builder class that handles the compilation of static HTML files.
    /// </summary>
    class Nitchify
    {
        private const string _OUTPUT_DIR_NAME = "_nitch";

        private string _rootFolder { get; set; }
        
        public PathingMode Pathing { get; set; }

        public Nitchify(string rootFolder, PathingMode pathing = PathingMode.Relative)
        {
            //
            // Validation input parameters - rootFolder path is required and must exist
            //

            if (String.IsNullOrEmpty(rootFolder))
                throw new ArgumentNullException(paramName: "rootFolder", message: "'rootFolder' must not be null.");

            if (!Directory.Exists(rootFolder))
                throw new Exception($"No foot folder found at location: {rootFolder}");

            //
            // Set intial parameter values
            //
            
            this._rootFolder = rootFolder;
            this.Pathing = pathing;
        }

        public void Build()
        {
            Console.WriteLine("Starting website build...");

            Console.WriteLine($"Root Folder: {_rootFolder}");
            Console.WriteLine($"File pathing: {Pathing.ToString()}");

            // Load all HTML files in and under root directory
            List<string> htmlFiles = LoadSourceFiles(_rootFolder);

            // TODO: Iterate over all files; process include: tokens

            Console.WriteLine($@"Output directory: {_rootFolder}\{_OUTPUT_DIR_NAME}");

            Console.WriteLine("Build complete!");
        }

        private List<string> LoadSourceFiles(string rootPath)
        {
            List<string> files = new List<string>();

            DirectoryInfo dirInfo = new DirectoryInfo(rootPath);

            foreach (var folder in dirInfo.GetDirectories())
            {
                files.AddRange(LoadSourceFiles(folder.FullName)); // Recurse into subdirectories
            }

            foreach (var file in dirInfo.GetFiles("*.html"))
            {
                files.Add(file.FullName);
            }

            return files;
        }
    }
}
