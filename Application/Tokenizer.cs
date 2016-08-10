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

        public Tokenizer(string content)
        {
            if (String.IsNullOrEmpty(content))
                throw new Exception("'content' is an empty string. This is not allowed.");

            _fileContents = content;
            this.Tokens = new List<Token>();
        }

        #endregion
        
        /// <summary>
        /// Parse the file contents for tokens of the given type and populates the internal token list.
        /// </summary>
        public void ProcessToken(string token)
        {
            if (this.Tokens != null && this.Tokens.Count > 0)
            {
                Reset();
            }

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
                            Type = Infrastructure.Enumerations.TokenType.Include,
                            Value = tokenValue,
                            RawValue = rawToken,
                            PositionInFile = startPos
                        };

                        this.Tokens.Add(newToken);
                    }
                    else
                    {
                        Log.Warning($"Token parse error at position: {startPos}");
                    }
                }
                else
                {
                    Log.Warning($"Invalid token at position: {startPos.ToString()}");
                }
            }
        }

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
                int startPos = rawToken.IndexOf(value: ":", startIndex: 0) + 2;
                int lengthToCut = rawToken.IndexOf("\"}}") - startPos;

                return rawToken.Substring(startPos, lengthToCut);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
