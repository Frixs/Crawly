
using System;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// UNDONE doc
    /// </summary>
    public class BasicProcessing
    {
        #region Private Members

        /// <summary>
        /// Tokenizer of this processing
        /// </summary>
        private Tokenizer _tokenizer;

        /// <summary>
        /// Stemmer of this processing
        /// </summary>
        private Stemmer _stemmer;

        #endregion

        #region Public Properties

        /// <summary>
        /// Indicates if this processing puts the document into the lower case
        /// </summary>
        public bool ToLowerCase { get; } //; ctor

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BasicProcessing(Tokenizer tokenizer, Stemmer stemmer, bool toLowerCase = false)
        {
            _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
            _stemmer = stemmer ?? throw new ArgumentNullException(nameof(stemmer));

            ToLowerCase = toLowerCase;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Indexate the text document
        /// </summary>
        /// <param name="document">The document</param>
        public void Index(string document)
        {
            // To lower
            if (ToLowerCase)
                document = document.ToLower();

            // Remove newlines from the document to prepare the doc for tokenization
            document = document.Replace(Environment.NewLine, " ");
            
            // Tokenize
            var tokens = _tokenizer.Tokenize(document);
            for (int i = 0; i < tokens.Length; ++i)
            {
                Console.Write($"'{tokens[i]}', ");
            }
            Console.WriteLine();
        }

        #endregion
    }
}
