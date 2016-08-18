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
        /// <summary>
        /// Default output directory for compiled files.
        /// </summary>
        private const string _OUTPUT_DIR_NAME = "_nitch";

        /// <summary>
        /// Default pattern for identifying 'master' files.
        /// </summary>
        private const string _MASTER_FILE_TEMPLATE = "master_";

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

            Log.Info($"Root Folder: {_rootFolder}");
            Log.Info($"File pathing: {Pathing.ToString()}");
            Console.Write("\n");

            // Load all HTML files in and under root directory
            List<string> htmlFiles = LoadSourceFiles(_rootFolder, _OUTPUT_DIR_NAME);
            List<OutputFile> outputFiles = new List<OutputFile>();

            // Step 1: Combine each file with master, process tokens, get output for each file
            foreach (string file in htmlFiles)
            {
                Log.Info($"Building file: {file}");
                OutputFile oFile = new OutputFile();
                oFile.FilePath = file;

                try
                {
                    oFile.HTML = ProcessFile(file);
                    outputFiles.Add(oFile);
                }
                catch (Exception exc)
                {
                    Log.Exception(exc.Message, "Error building file.");
                }
            }

            // Step 2: Copy all non-HTML and non-master files to new output directory (respecting folder structure)
            try
            {
                // Always clear existing output and start fresh
                string outputDir = Path.Combine(_rootFolder, _OUTPUT_DIR_NAME);
                if (Directory.Exists(outputDir))
                    Directory.Delete(outputDir, true);

                CopyBaseFilesToOutputFolder(_rootFolder, _OUTPUT_DIR_NAME);
            }
            catch (Exception exc)
            {
                Log.Exception(exc.Message, "Error setting up output directory.");
            }

            // Step 3: Write output HTML files to their proper locations
            foreach (OutputFile file in outputFiles)
            {
                string outputPath = file.FilePath.Replace(_rootFolder, _OUTPUT_DIR_NAME);
                outputPath = Path.Combine(_rootFolder, outputPath);

                try
                {
                    File.WriteAllText(outputPath, file.HTML);
                }
                catch (Exception exc)
                {
                    Log.Exception(exc.Message, $"Error writing output file: {file.FilePath}");
                }
            }

            Console.Write("\n");
            Log.Info($"Files built: {outputFiles.Count}");
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
        private List<string> LoadSourceFiles(string rootPath, string ignoreFolder)
        {
            string[] allFiles = Directory.GetFiles(rootPath, "*.html", SearchOption.AllDirectories);

            // In case a nitch output folder arleady exists, ignore it
            allFiles = allFiles.Where(u => !u.Contains(ignoreFolder)).ToArray();

            // Remove "master_" HTML files from the list so they aren't processed for {{master:}} tokens
            allFiles = FileHelper.RemoveMasterFilesFromList(allFiles, _MASTER_FILE_TEMPLATE);

            return allFiles.ToList();
        }

        /// <summary>
        /// Processes a Nitch HTML file into a temporary buffer of final output. Combines and refrenced master files
        /// </summary>
        /// <param name="filePath">Path to the file that should be compiled.</param>
        /// <returns>Output of compiled file.</returns>
        private string ProcessFile(string filePath)
        {
            // Set up final output buffer
            string fileOutput = string.Empty;

            // Open 'filePath', read into buffer; empty files should not be processed
            string fileBuffer = File.ReadAllText(filePath);

            if (fileBuffer.Trim().Length == 0)
                return string.Empty;

            // Scan for {{master:}} token; master file and its child page will need to be merged
            Tokenizer tokensForMaster = new Tokenizer(fileBuffer);
            tokensForMaster.ProcessToken("{{master:}}");

            // If {{master:}} token is found, load master file contents into buffer, find {{placeholder:}} token, and combine two files (replacement of {{placeholder:}} token with child page content)
            if (tokensForMaster.Tokens.Count > 1)
                throw new Exception("Only one {{master:}} token can be defined per file.");

            if (tokensForMaster.Tokens.Count == 1)
            {
                fileOutput = CombineMasterWithChild(fileBuffer, tokensForMaster.Tokens[0].Value);

                // Remove {{master:}} token from child page
                fileOutput = fileOutput.Replace(tokensForMaster.Tokens[0].RawValue, string.Empty);
            }
            
            // Process {{file:}} tokens in new file (respect absolute/relative pathing option)
            Tokenizer tokensForFiles = new Tokenizer(fileOutput);
            tokensForFiles.ProcessToken("{{file:}}");

            foreach (Token fileToken in tokensForFiles.Tokens)
            {
                try
                {
                    fileOutput = ProcessFileToken(fileToken, filePath, fileOutput);
                }
                catch (Exception exc)
                {
                    Log.Exception(exc.Message.ToString(), $"Error compiling {{{{file:}}}} token at position {fileToken.PositionInFile.ToString()}.");
                }
            }
            
            return fileOutput;
        }

        /// <summary>
        /// Gets the depth of the file or folder represented by the file path.
        /// </summary>
        /// <param name="filePath">Full absolute path of the file.</param>
        /// <returns>Depth of the file or folder in whole integer.</returns>
        private int CalculateFileDepth(string filePath)
        {
            if (!filePath.Contains("/") && !filePath.Contains(@"\"))
                return 0;

            if (String.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");

            filePath = filePath.Replace(@"\", "/").Replace("//", string.Empty);
            
            return filePath.Split('/').Length;
        }

        /// <summary>
        /// Creates a relative file path to the target file based on the location of the current file.
        /// </summary>
        /// <param name="currentFilePath">Absolute path of the source file from the root of the website.</param>
        /// <param name="targetFilePath">Absolute path of the target file from the root of the website.</param>
        /// <returns>Complete relative path.</returns>
        private string GetRelativeFilePath(string currentFilePath, string targetFilePath)
        {
            int fileDepth = CalculateFileDepth(currentFilePath);
            string finalPath = string.Empty;

            for (int i = 1; i <= fileDepth; i++)
            {
                finalPath += "../";
            }

            if (targetFilePath.StartsWith("/"))
                targetFilePath = targetFilePath.Remove(0, 1);

            finalPath += targetFilePath;

            return finalPath;
        }

        /// <summary>
        /// Combines child page content into master page content via placeholder tokens.
        /// </summary>
        /// <param name="childPage">File contents (HTML) of child page.</param>
        /// <param name="pathToMaster">Path to the master page that was defined on the given child page.</param>
        /// <returns>File contents of combined pages. {{placeholder:}} token is removed from master page.</returns>
        private string CombineMasterWithChild(string childPageContent, string pathToMaster)
        {
            string masterFilePath = Path.Combine(_rootFolder, FileHelper.FormatForPathCombining(pathToMaster));

            if (!File.Exists(masterFilePath))
                throw new FileNotFoundException($"No master file found at {pathToMaster}");

            string masterContents = File.ReadAllText(masterFilePath);

            // Get {{placeholder:}} token in master file and replace it with the childPage contents
            Tokenizer placeholderTokens = new Tokenizer(masterContents);
            placeholderTokens.ProcessToken("{{placeholder:}}");

            if (placeholderTokens.Tokens.Count == 0)
                throw new Exception($"No {{{{placeholder:}}}} token found in master file: {pathToMaster}");

            return masterContents.Replace(placeholderTokens.Tokens[0].RawValue, childPageContent);
        }

        /// <summary>
        /// Parses and renders a {{file:}} token value into an absolute or relative path for the HTML output.
        /// </summary>
        /// <param name="fileToken">Token object for File type.</param>
        /// <param name="currentFilePath">Path to the current file. Required for relative pathing options.</param>
        /// <param name="fileContents">File output buffer.</param>
        /// <returns>Output HTML with file token rendered to final HTML.</returns>
        private string ProcessFileToken(Token fileToken, string currentFilePath, string fileContents)
        {
            currentFilePath = currentFilePath.Replace(_rootFolder, string.Empty).Remove(0, 1);

            // Verify file exists at given location in the token
            string filePath = Path.Combine(_rootFolder, FileHelper.FormatForPathCombining(fileToken.Value));
            
            if (File.Exists(filePath))
            {
                // If yes (file found), then output its path (relative/absolute)
                if (this.Pathing == PathingMode.Absolute)
                {
                    // All paths in source are required to be absolute from the website root, so just swap the token for that value
                    fileContents = fileContents.Replace(fileToken.RawValue, fileToken.Value);
                }
                else if (this.Pathing == PathingMode.Relative)
                {
                    // Swap with a relative path
                    string relativePath = GetRelativeFilePath(currentFilePath, fileToken.Value);
                    fileContents = fileContents.Replace(fileToken.RawValue, relativePath);
                }
                else
                {
                    throw new Exception("Nitchify 'Pathing' is null. Pathing type cannot be determined.");
                }

                return fileContents;
            }
            else
            {
                // If no file found, throw warning but do not error - this will let devs catch incorrect file references
                Log.Warning($"File not found for token {fileToken.RawValue} at position {fileToken.PositionInFile.ToString()}");
            }

            return string.Empty;
        }
        
        /// <summary>
        /// Copies source files to output directory, excluding master files.
        /// </summary>
        /// <param name="rootPath">Starting folder path.</param>
        /// <param name="targetPath">Destination directory.</param>
        private void CopyBaseFilesToOutputFolder(string rootPath, string targetPath)
        {
            DirectoryInfo sourceInfo = new DirectoryInfo(_rootFolder);
            DirectoryInfo destinationInfo = new DirectoryInfo(Path.Combine(FileHelper.FormatForPathCombining(_rootFolder), FileHelper.FormatForPathCombining(_OUTPUT_DIR_NAME)));

            FileHelper.CopyAll(sourceInfo, destinationInfo, _MASTER_FILE_TEMPLATE, _OUTPUT_DIR_NAME);
        }
    }
}
