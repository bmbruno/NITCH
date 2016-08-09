﻿using Nitch.Infrastructure;
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

        #region Members

        private string _rootFolder { get; set; }
        
        public PathingMode Pathing { get; set; }

        #endregion

        #region Constructors

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

        #endregion

        /// <summary>
        /// Kicks off the Nitchify compilation process.
        /// </summary>
        public void Build()
        {
            Console.WriteLine("Starting website build...");

            Console.WriteLine($"Root Folder: {_rootFolder}");
            Console.WriteLine($"File pathing: {Pathing.ToString()}");
            Console.Write("\n");

            // Load all HTML files in and under root directory
            List<string> htmlFiles = LoadSourceFiles(_rootFolder);

            foreach (string file in htmlFiles)
            {
                Log.Info($"Building file: {file}");
                string rawFileOutput = ProcessFile(file, file);
            }

            Console.Write("\n");
            Console.WriteLine($@"Output directory: {_rootFolder}\{_OUTPUT_DIR_NAME}");

            Console.WriteLine("Build complete!");
        }

        /// <summary>
        /// Recursively Loads all .html files from a root path.
        /// </summary>
        /// <param name="rootPath">Starting folder path.</param>
        /// <returns>List of filepaths.</returns>
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

        private string ProcessFile(string file, string sourceFile)
        {
            // Open 'file', read into buffer; empty files should not be processed
            string fileBuffer = File.ReadAllText(file);

            if (fileBuffer.Trim().Length == 0)
                return string.Empty;

            // TODO: Scan for {{include:}} token
            Tokenizer tokenizer = new Tokenizer(fileBuffer);
            tokenizer.Process();

            var list = tokenizer.GetTokenList();

            // TODO: For each {{include:}} token, recurse but respect the sourceFile for pathing values

            // TODO: Scan for {{file:}} tokens, process each (as absolute or relative, to sourceFile)

            return string.Empty;
        }
    }
}
