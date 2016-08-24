using Nitch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nitch.Infrastructure.Helpers;
using Nitch.Infrastructure;

namespace Nitch
{
    public class Tokenizer
    {
        #region Members

        public List<Token> Tokens { get; set; }

        private string _fileContents { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the Token scanner.
        /// </summary>
        /// <param name="fileContent">Contents of the file to be scanned. Usually NOT a 'master' page.</param>
        public Tokenizer(string fileContent)
        {
            if (String.IsNullOrEmpty(fileContent))
                throw new ArgumentNullException("fileContent");

            _fileContents = fileContent;
            this.Tokens = new List<Token>();
        }

        #endregion
        
        /// <summary>
        /// Parse the file contents for tokens of the given type and populates the internal token list.
        /// </summary>
        public void ProcessToken(string token)
        {
            if (this.Tokens != null && this.Tokens.Count > 0)
                Reset();
            
            // If last two characters of token are }}, remove them for the search
            if (token.EndsWith("}}"))
                token = token.Remove(token.IndexOf("}}"), 2);

            IEnumerable<int> allTokenIndexes = _fileContents.AllIndexesOf(token);

            foreach (int startPos in allTokenIndexes)
            {
                // Get and build token at this location
                string rawToken = GetTokenRawString(startPos);

                if (!String.IsNullOrEmpty(rawToken))
                {
                    // Get token value (parse the string)
                    string tokenValue = GetTokenValue(rawToken);

                    if (!String.IsNullOrEmpty(tokenValue))
                    {
                        Token newToken = new Token()
                        {
                            Value = tokenValue,
                            RawValue = rawToken,
                            PositionInFile = startPos
                        };

                        this.Tokens.Add(newToken);
                    }
                    else
                    {
                        throw new Exception($"Token parse error at position: {startPos}");
                    }
                }
                else
                {
                    throw new Exception($"Invalid token at position: {startPos.ToString()}");
                }
            }
        }

        /// <summary>
        /// Cleans up any 
        /// </summary>
        private void Reset()
        {
            this.Tokens = new List<Token>();
        }

        /// <summary>
        /// Given the starting position of a token, returns the full token based on the standard closing delimiter }}.
        /// </summary>
        /// <param name="startPosition">Starting position of token</param>
        /// <returns>Full string of token, including delimiter characters {{ }}; returns empty string if no token found.</returns>
        private string GetTokenRawString(int startPosition)
        {
            try
            {
                int lengthToCut = ((_fileContents.IndexOf(value: "}}", startIndex: startPosition)) - startPosition) + 2;
                return _fileContents.Substring(startPosition, lengthToCut);
            }
            catch
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// Gets the value of a token based on the ':' delimiter.
        /// </summary>
        /// <param name="rawToken">Token string to parse.</param>
        /// <returns>String of the token's value; returns empty string if token parse errors</returns>
        private string GetTokenValue(string rawToken)
        {
            try
            {
                int startPos = rawToken.IndexOf(value: ":", startIndex: 0) + 1;
                int lengthToCut = rawToken.IndexOf("}}") - startPos;

                return rawToken.Substring(startPos, lengthToCut);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
