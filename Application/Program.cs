using Nitch.Infrastructure.Helpers;
using ParamParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitch
{
    /// <summary>
    /// NITCH - .NET Integrated Template Compiler for HTML
    /// Probably going to call this "Niche" when all is said and done?
    /// </summary>
    /// 
    class Program
    {
        static void Main(string[] args)
        {
            string AppVersion = "0.25";
            string appPath = FileHelper.GetCurrentApplicationDirectory();

            Console.WriteLine("NITCH (.NET Integrated Template Compiler for HTML)");
            Console.WriteLine($"Version: {AppVersion}");
            Console.Write("\n");

            //
            // PROGRAM ARGUMENTS
            //

            Parser parser = new Parser(args);

            if (parser.Parameters.Count == 0)
            {
                // TODO: No parameters - show help
                Console.Write("\n");
                Console.WriteLine("HELP TEXT COMING SOON");
                Console.Write("\n");
            }
            else
            {
                // Attempt to run program
                
                if (parser.HasParam("create"))
                {
                    if (String.IsNullOrEmpty(parser.GetParam("create")))
                    {
                        // Run default project file/folder creation
                        Nitchify builder = new Nitchify(appPath);
                        builder.Create();
                    }
                    else
                    {
                        // Run project file/folder creation at the given path
                        Nitchify builder = new Nitchify(parser.GetParam("create"));
                        builder.Create();
                    }
                }

                if (parser.HasParam("build"))
                {
                    if (String.IsNullOrEmpty(parser.GetParam("build")))
                    {
                        // Run default build in the current folder
                        Nitchify builder = new Nitchify(appPath, Infrastructure.Enumerations.PathingMode.Absolute);
                        builder.Build();
                    }
                    else
                    {
                        // TODO: Run build in specified folder
                        string startPath = parser.GetParam("build");
                        if (System.IO.Directory.Exists(startPath))
                        {
                            // Nitchify builder = new Nitchify(startPath, Infrastructure.Enumerations.PathingMode.Absolute);
                            Nitchify builder = new Nitchify(startPath, Infrastructure.Enumerations.PathingMode.Relative);
                            builder.Build();
                        }
                        else
                        {
                            Console.WriteLine($"ERROR: Path not found: {startPath}");
                            return;
                        }
                    }
                }
            }
        }
    }
}
