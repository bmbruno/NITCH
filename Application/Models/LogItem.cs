using Nitch.Infrastructure.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitch.Models
{
    public class LogItem
    {
        public LogType Type { get; set; }

        public string Message { get; set; }
    }
}
