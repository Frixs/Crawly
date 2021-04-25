using InformationRetrievalManager.Core;
using Microsoft.ML;
using Microsoft.ML.Transforms.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Removes stop words
    /// </summary>
    public class StopWordRemover
    {
        #region Private Members

        /// <summary>
        /// Stop words that should be included in the language-specific stopword list
        /// </summary>
        private HashSet<string> _myStopWords; //; ctor

        #endregion

        #region Public Properties

        /// <summary>
        /// Language used to get stop words
        /// </summary>
        public ProcessingLanguage Language { get; } //; ctor

        /// <summary>
        /// Read-only for <see cref="_myStopWords"/>
        /// </summary>
        public IReadOnlyCollection<string> MyStopWords => _myStopWords;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="language">Language to select the correct stopwords</param>
        /// <param name="myStopWords">Stop words that should be included in the language-specific stopword list</param>
        public StopWordRemover(ProcessingLanguage language = ProcessingLanguage.EN, HashSet<string> myStopWords = null)
        {
            Language = language;
            _myStopWords = myStopWords;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Process the words and remove stopwrods by the remover specification
        /// </summary>
        /// <param name="words">The words</param>
        /// <returns>New words with removed stop words</returns>
        public virtual string[] Process(string[] words)
        {
            if (words == null || words.Length == 0)
                return Array.Empty<string>();

            // Create the context
            var mlContext = new MLContext();

            // Assemble the machine-learning engine
            var emptyData = new List<TextData>();
            var data = mlContext.Data.LoadFromEnumerable(emptyData);
            // Estimate the lanaguage basic stop-words
            var estimator = mlContext.Transforms.Text
                .RemoveDefaultStopWords(nameof(TransformedTextData.WordsWithoutStopWords), nameof(TextData.Words), GetEstimatorLanguage());
            // Add for rmeoval custom stop words, if any...
            if (MyStopWords != null)
                estimator.Append(mlContext.Transforms.Text.RemoveStopWords(nameof(TransformedTextData.WordsWithoutStopWords), nameof(TextData.Words), stopwords: MyStopWords.ToArray()));
            // Build it
            var stopWordsModel = estimator.Fit(data);
            var engine = mlContext.Model.CreatePredictionEngine<TextData, TransformedTextData>(stopWordsModel);

            // Process the words
            return engine.Predict(new TextData { Words = words }).WordsWithoutStopWords ?? Array.Empty<string>();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get the correct enumarate language for estimator based on remover's selected language.
        /// </summary>
        /// <returns>Estimator's language enum</returns>
        public StopWordsRemovingEstimator.Language GetEstimatorLanguage()
        {
            switch (Language)
            {
                case ProcessingLanguage.EN:
                    return StopWordsRemovingEstimator.Language.English;
                case ProcessingLanguage.CZ:
                    return StopWordsRemovingEstimator.Language.Czech;
                default:
                    Debugger.Break();
                    return StopWordsRemovingEstimator.Language.English;
            }
        }

        #endregion

        #region Private Helper Classes

        /// <summary>
        /// Text data for ML engine input
        /// </summary>
        private class TextData
        {
            public string[] Words { get; set; }
        }

        /// <summary>
        /// Text data wrapper for ML output
        /// </summary>
        private class TransformedTextData : TextData
        {
            public string[] WordsWithoutStopWords { get; set; }
        }

        #endregion
    }
}
