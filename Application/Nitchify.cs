using Nitch.Infrastructure;
using Nitch.Infrastructure.Enumerations;
using Nitch.Infrastructure.Helpers;
using Nitch.Models;
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
    public class Nitchify
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
            Log.Info("Starting website build...");

            Console.WriteLine($"Root Folder: {_rootFolder}");
            Console.WriteLine($"File pathing: {Pathing.ToString()}");
            Console.Write("\n");

            // Load all HTML files in and under root directory
            List<string> htmlFiles = LoadSourceFiles(_rootFolder);

            foreach (string file in htmlFiles)
            {
                Log.Info($"Building file: {file}");
                string rawFileOutput = ProcessFile(file);
            }

            Console.Write("\n");
            Log.Info($@"Output directory: {_rootFolder}\{_OUTPUT_DIR_NAME}");
            Log.Info("Build complete!");
        }

        /// <summary>
        /// Creates a default project structure, including basic folders and various HTML files to demonstrate the functionality of Nitch.
        /// </summary>
        /// <param name="startPath"></param>
        public void Create()
        {
            Log.Info("Creating default website structure...");

            string newProjectFolder = "new_website_project";
            string[] rootFolders = { "master", "content" };
            string[] contentFolders = { "css", "scripts", "images" };

            // Handle possible duplicate folder names
            int counter = 0;
            string defaultName = newProjectFolder;
            while (Directory.Exists(Path.Combine(_rootFolder, newProjectFolder)))
            {
                counter += 1;
                newProjectFolder = defaultName + counter.ToString();
            }

            try
            {
                // Create folders
                foreach (string folder in rootFolders)
                {
                    string finalPath = Path.Combine(_rootFolder, newProjectFolder, folder);

                    if (!Directory.Exists(finalPath))
                        Directory.CreateDirectory(finalPath);
                }

                // Create subfolders
                foreach (string folder in contentFolders)
                {
                    string finalPath = Path.Combine(_rootFolder, newProjectFolder, "content/", folder);

                    if (!Directory.Exists(finalPath))
                        Directory.CreateDirectory(finalPath);
                }

                Log.Info("Created folders.");
            }
            catch (Exception exc)
            {
                Log.Exception(exc.ToString(), "Exception while creating folders.");
            }
            
            try
            {
                // index.html
                string indexFilePath = Path.Combine(_rootFolder, newProjectFolder, "index.html");
                if (!File.Exists(indexFilePath))
                {
                    File.WriteAllText(indexFilePath, DefaultFiles.indexHTML);
                }

                // master_main.html
                string masterFilePath = Path.Combine(_rootFolder, newProjectFolder, "master", "master_main.html");
                if (!File.Exists(masterFilePath))
                {
                    File.WriteAllText(masterFilePath, DefaultFiles.masterHTML);
                }

                Log.Info("Created files.");
            }
            catch (Exception exc)
            {
                Log.Exception(exc.ToString(), "Exception while creating HTML files.");
            }

            Console.WriteLine($@"New directory: {Path.Combine(_rootFolder, newProjectFolder)}");
            Console.WriteLine("Creation complete!");
        }

        /// <summary>
        /// Recursively Loads all .html files from a root path.
        /// </summary>
        /// <param name="rootPath">Starting folder path.</param>
        /// <returns>List of filepaths.</returns>
        private List<string> LoadSourceFiles(string rootPath)
        {
            List<string> soureFiles = new List<string>();
            string[] allFiles = Directory.GetFiles(rootPath, "*.html", SearchOption.AllDirectories);

            // remove "master_" HTML files from the list
            foreach (string file in allFiles)
            {
                string fileName = Path.GetFileName(file);

                if (!fileName.StartsWith("master_", StringComparison.OrdinalIgnoreCase))
                    soureFiles.Add(file);
            }

            return soureFiles;
        }

        /// <summary>
        /// Processes a Nitch HTML file into
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string ProcessFile(string filePath)
        {
            // Set up final output buffer and pathing tracking
            StringBuilder fileOutput = new StringBuilder();
            int fileDepth = CalculateFileDepth(filePath);

            // Open 'file', read into buffer; empty files should not be processed
            string fileBuffer = File.ReadAllText(filePath);

            if (fileBuffer.Trim().Length == 0)
                return string.Empty;

            fileOutput.Append(fileBuffer);

            // Scan for {{master:}} token; master file and its child page are merged first
            Tokenizer tokenizer = new Tokenizer(fileBuffer);
            tokenizer.ProcessToken("{{master:}}");

            // TODO: If found, process master file ({{file:}} tokens)


            // TODO: Process {{file:}} tokens




            //Tokenizer tokenizer = new Tokenizer(fileBuffer);
            //tokenizer.ProcessToken("{{include:");
            //List<Token> tokensInclude = tokenizer.GetTokenList();
            
            //// TODO: For each {{include:}} token, recurse but respect the sourceFile for pathing values
            //foreach (Token token in tokensInclude)
            //{
            //    string localPath = token.Value.Substring(1, token.Value.Length - 1);
            //    var finalPath = Path.Combine(_rootFolder, localPath);

            //    fileBuffer = File.ReadAllText(finalPath);
            //}

            return string.Empty;
        }

        /// <summary>
        /// Gets the depth of the file or folder represented by the file path.
        /// </summary>
        /// <param name="filePath">Full absolute path of the file.</param>
        /// <returns>Depth of the file or folder in whole integer.</returns>
        private int CalculateFileDepth(string filePath)
        {
            if (String.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");

            if (filePath.Contains("\\"))
                return (filePath.Split('\\').Length);
            else
                return (filePath.Split('/').Length);

        }
        
    }
}
