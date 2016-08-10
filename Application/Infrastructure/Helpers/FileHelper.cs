using System;
using System.Collections.Generic;
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
    }
}
