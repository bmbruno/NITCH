using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitch.Infrastructure
{
    public static class Log
    {
        public static void Error(string message)
        {
            Console.WriteLine($"[ERROR] {message}");
        }

        public static void Exception (string exceptionMessage, string message)
        {
            Console.WriteLine($"[EXCEPTION] {message} \n\n Exception Message: \n\n {exceptionMessage}");
        }

        public static void Warning (string message)
        {
            Console.WriteLine($"[WARNING] {message}");
        }

        public static void Info(string message)
        {
            Console.WriteLine($"[INFO] {message}");
        }
    }
}
