using Nitch.Infrastructure.Enumerations;
using Nitch.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitch.Infrastructure
{
    public class Log
    {
        private List<LogItem> _logOutput { get; set; }

        private bool _useConsole { get; set; }

        public Log(bool useConsole = true)
        {
            _useConsole = useConsole;
            _logOutput = new List<LogItem>();
        }

        public bool HasErrors
        {
            get
            {
                return _logOutput.Any(u => u.Type == LogType.Exception || u.Type == LogType.Exception);
            }
        }

        public string GetFormattedMessage(LogItem item)
        {
            return $"[{item.Type.ToString().ToUpper()}] {item.Message}";
        }

        public void WriteLogToFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            List<String> output = new List<string>();

            foreach (LogItem item in this._logOutput)
            {
                output.Add(this.GetFormattedMessage(item));
            }

            File.WriteAllLines(filePath, output.ToArray());
        }

        public void Error(string message)
        {
            _logOutput.Add(new LogItem()
            {
                Type = LogType.Error,
                Message = message
            });

            if (this._useConsole)
                Console.WriteLine($"[ERROR] {message}");
        }

        public void Exception (string exceptionMessage, string message)
        {
            _logOutput.Add(new LogItem()
            {
                Type = LogType.Exception,
                Message = message
            });

            if (this._useConsole)
                Console.WriteLine($"[EXCEPTION] {message} \n\n Exception Message: \n\n {exceptionMessage}");
        }

        public void Warning (string message)
        {
            _logOutput.Add(new LogItem()
            {
                Type = LogType.Warning,
                Message = message
            });

            if (this._useConsole)
                Console.WriteLine($"[WARNING] {message}");
        }

        public void Info(string message)
        {
            _logOutput.Add(new LogItem()
            {
                Type = LogType.Info,
                Message = message
            });

            if (this._useConsole)
                Console.WriteLine($"[INFO] {message}");
        }
    }
}
