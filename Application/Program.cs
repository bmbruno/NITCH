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

            Console.WriteLine("NITCH (.NET Integrated Template Compiler for HTML)");
            Console.WriteLine($"Version: {AppVersion}");

            //
            // PROGRAM ARGUMENTS
            //

            Parser parser = new Parser(args);
            
            if (parser.HasParam("create"))
            {
                if (String.IsNullOrEmpty(parser.GetParam("create")))
                {
                    // TODO: Run default project structure creator
                }
                else
                {
                    // TODO: Run project structure creator based on config settings (config must be next to executable)
                }
            }

            if (parser.HasParam("build"))
            {
                if (String.IsNullOrEmpty(parser.GetParam("build")))
                {
                    // TODO: Run default build in the current folder
                }
                else
                {
                    // TODO: Run build in specified folder
                    string startPath = parser.GetParam("build");
                    if (System.IO.Directory.Exists(startPath))
                    {
                        Nitchify builder = new Nitchify(startPath, Infrastructure.Enumerations.PathingMode.Absolute);
                        builder.Build();
                    }
                    else
                    {
                        Console.WriteLine($"ERROR: Path not found: {startPath}");
                        return;
                    }
                }
            }
            else
            {
                // Run default build in the current folder
                string appLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string appPath = System.IO.Path.GetDirectoryName(appLocation);

                Nitchify builder = new Nitchify(appPath, Infrastructure.Enumerations.PathingMode.Absolute);
                // builder.Build(); // TODO: uncomment this line
            }
        }
    }
}
