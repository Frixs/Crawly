using System;
using System.Text.RegularExpressions;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Split a text into tokens
    /// </summary>
    public class Tokenizer
    {
        #region Constants

        /// <summary>
        /// Default regular expression used in this tokenizer (it is used if the custom one is not set)
        /// </summary>
        public const string DefaultRegex = @"((http|ftp|https)://)?(www)?([\w_-]+(?:(?:\.[A-Za-z_-]+)+))([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?|\d+\.\d+\.(\d+)?|<\/?[A-Za-z0-9]+>|[0-9A-Za-zÀ-úěščřžýáíé]+[0-9A-Za-zÀ-úěščřžýáíé_\-:*]+[0-9A-Za-zÀ-úěščřžýáíé]+|[0-9A-Za-zÀ-úěščřžýáíé]+(?=\s*)";
        // Intended for Regex.Split - finds separators
        //public const string DefaultRegex = @"(?<=\D+)([.,]+)(?=[^0-9A-Za-zÀ-úěščřžýáíé]+)\s*|(?<!(<\/?[0-9A-Za-zÀ-úěščřžýáíé]+))>\s*|\s*<\/?(?!(\/?[0-9A-Za-zÀ-úěščřžýáíé]+>))|((?<=([A-Za-zÀ-úěščřžýáíé](?!>))|([0-9](?!\.){1}))[^0-9A-Za-zÀ-úěščřžýáíé]*)*(^|\s+)[^0-9A-Za-zÀ-úěščřžýáíé]*(?=((?<!<\/?)[0-9A-Za-zÀ-úěščřžýáíé]))|(?<=([A-Za-zÀ-úěščřžýáíé](?!>)))[^0-9A-Za-zÀ-úěščřžýáíé]*($|\s+)([^0-9A-Za-zÀ-úěščřžýáíé]*(?=((?<!<\/?)[0-9A-Za-zÀ-úěščřžýáíé])))*|\s+|[^0-9A-Za-zÀ-úěščřžýáíé]{2,}(?!\s+)\s+|\s+(?<!\s+)[^0-9A-Za-zÀ-úěščřžýáíé]{2,}";

        #endregion

        #region Private Members

        /// <summary>
        /// Custom regular expression used by this tokenizer (if it is not set, it is replaced with the default one)
        /// </summary>
        private string _customRegex = null;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets regular expression currently used by this tokenizer
        /// </summary>
        public string UsingRegex => _customRegex ?? DefaultRegex;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="regex">Custom regex that can be used by this tokenizer</param>
        public Tokenizer(string regex = null)
        {
            if (!string.IsNullOrEmpty(regex))
                _customRegex = regex;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Tokenize the input text by the current regex.
        /// </summary>
        /// <param name="text">The text</param>
        /// <returns>Array of tokens made from the text</returns>
        public virtual string[] Tokenize(string text)
        {
            if (text == null)
                return Array.Empty<string>();

            var matches = Regex.Matches(text, UsingRegex);
            
            var tokens = new string[matches.Count];
            for (int i = 0; i < matches.Count; ++i)
                tokens[i] = matches[i].Value;
            
            return tokens;
        }

        #endregion
    }
}
