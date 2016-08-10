using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParamParser
{
    public class Parser
    {
        // TODO: make this readonly as a public member
        public Dictionary<string, string> Parameters { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Parser()
        {
            Parameters = new Dictionary<string, string>();
        }

        /// <summary>
        /// Advanced constructor. Parses arguments array into Parameters.
        /// </summary>
        /// <param name="args">args array from program Main().</param>
        public Parser(string[] args)
        {
            Parameters = new Dictionary<string, string>();

            this.Parse(args);
        }

        /// <summary>
        /// Parses an args array into Parameters.
        /// </summary>
        /// <param name="args">args array from program Main()</param>
        public void Parse(string[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {
                if ((args[i][0] == '-') || (args[i][0] == '/'))
                {
                    Parameters.Add(
                        args[i].Replace("-", string.Empty).Replace("/", string.Empty),
                        (args.Length > (i + 1)) ? args[i + 1] : null
                    );
                }
            }

            return;
        }

        /// <summary>
        /// Determines if the specified key exists in the Parameters list.
        /// </summary>
        /// <param name="key">Key to check.</param>
        /// <returns>True: key exists; False: key does not exist.</returns>
        public bool HasParam(string key)
        {
            return (Parameters.ContainsKey(key));
        }

        /// <summary>
        /// Gets the parameter value for the given key.
        /// </summary>
        /// <param name="key">Key to get.</param>
        /// <returns>Value of parameter or empty string if parameter is empty/null.</returns>
        public string GetParam(string key)
        {
            return (this.HasParam(key)) ? Parameters[key] : string.Empty;
        }
   }
}
