using Nitch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        }

        /// <summary>
        /// Gets the full list of tokens.
        /// </summary>
        /// <returns></returns>
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
    }
}
