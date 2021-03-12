using System;
using System.Text.RegularExpressions;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Split a text into tokens
    /// </summary>
    public class Tokenizer
    {
        #region Static Constants

        /// <summary>
        /// Default regular expression used in this tokenizer (it is used if the custom one is not set)
        /// </summary>
        public const string DefaultRegex = @"\W*\s+\W*|\s+|\W+";

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
            _customRegex = regex;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Tokenize the input text by the current regex.
        /// </summary>
        /// <param name="text">The text</param>
        /// <returns>Array of tokens made from the text</returns>
        public string[] Tokenize(string text)
        {
            if (text == null)
                return Array.Empty<string>();

            return Regex.Split(text, UsingRegex);
        }

        #endregion
    }
}
