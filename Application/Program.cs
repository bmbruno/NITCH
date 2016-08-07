using ParamParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitch
{
    class Program
    {
        static void Main(string[] args)
        {
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
                }
            }

            
        }
    }
}
