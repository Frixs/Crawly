
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

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BasicProcessing(Tokenizer tokenizer, Stemmer stemmer)
        {
            _tokenizer = tokenizer;
            _stemmer = stemmer;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Indexate the text document
        /// </summary>
        /// <param name="document">The document</param>
        public void Index(string document)
        {

        }

        #endregion
    }
}
