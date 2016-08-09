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

        private List<Token> _tokens { get; set; }

        private string _fileContents { get; set; }

        private int _listPosition { get; set; }

        #endregion

        #region Constructors

        public Tokenizer(string content)
        {
            if (String.IsNullOrEmpty(content))
                throw new Exception("'content' is an empty string. This is not allowed.");

            _fileContents = content;
            _tokens = new List<Token>();
            _listPosition = 0;
        }

        #endregion
        
        /// <summary>
        /// Processes the file and gets all tokens.
        /// </summary>
        public void Process()
        {
            IEnumerable<int> allIncludeIndexes = _fileContents.AllIndexesOf("{{include:");

            foreach (int startPos in allIncludeIndexes)
            {
                // Get and build token at this location
                string rawToken = GetTokenRawString(startPos);

                if (!String.IsNullOrEmpty(rawToken))
                {
                    // Get token value (parse the string)
                    string tokenValue = GetTokenValue(rawToken);

                    Token newToken = new Token()
                    {
                        Type = Infrastructure.Enumerations.TokenType.Include,
                        Value = "",
                        RawValue = rawToken,
                        PositionInFile = startPos
                    };

                    _tokens.Add(newToken);
                }
                else
                {
                    Log.Warning($"Invalid token at position: {startPos.ToString()}");
                }
            }
        }

        /// <summary>
        /// Gets the full list of tokens.
        /// </summary>
        /// <returns>List of Token objects.</returns>
        public List<Token> GetTokenList()
        {
            return _tokens;
        }

        /// <summary>
        /// Gets the next token in the token list. Returns null if at the end of the list. Updates position in list.
        /// </summary>
        /// <returns>Token object. Null if at end of list.</returns>
        public Token GetNextToken()
        {
            if ((_listPosition + 1) <= _tokens.Count)
            {
                _listPosition += 1;
                return _tokens[_listPosition];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the first token in the token list. Returns null if the list is empty. Updates position in list.
        /// </summary>
        /// <returns></returns>
        public Token GetFirstToken()
        {
            if (_tokens.Count > 0)
            {
                _listPosition = 0;
                return _tokens[_listPosition];
            }
            else
            {
                return null;
            }
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

        private string GetTokenValue(string rawToken)
        {
            int startPos = rawToken.IndexOf(":") + 2;
            int lengthToCut = rawToken.IndexOf("\"}}") - startPos;

            return rawToken.Substring(startPos, lengthToCut);
        }
    }
}
