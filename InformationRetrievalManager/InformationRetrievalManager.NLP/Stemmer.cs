using Iveonik.Stemmers;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// UNDONE doc
    /// </summary>
    public class Stemmer
    {
        #region Private Members

        /// <summary>
        /// Currently used stemmer
        /// </summary>
        private IStemmer _stemmer;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public Stemmer()
        {
            _stemmer = new EnglishStemmer();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Stem the word
        /// </summary>
        /// <param name="word">The word</param>
        /// <returns>Stemmed word</returns>
        public string Stem(string word) => _stemmer.Stem(word);

        #endregion
    }
}
