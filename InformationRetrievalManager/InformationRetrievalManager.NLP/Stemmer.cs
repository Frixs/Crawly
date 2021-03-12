using Iveonik.Stemmers;
using System.Diagnostics;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Stemmer wrapper
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
        /// <param name="language">Language specification for this stemmer</param>
        public Stemmer(StemmerLanguage language = StemmerLanguage.EN)
        {
            switch (language)
            {
                case StemmerLanguage.EN:
                    _stemmer = new EnglishStemmer();
                    break;
                case StemmerLanguage.CZ:
                    _stemmer = new CzechStemmer();
                    break;
                default:
                    _stemmer = new EnglishStemmer();
                    Debugger.Break();
                    break;
            }
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
