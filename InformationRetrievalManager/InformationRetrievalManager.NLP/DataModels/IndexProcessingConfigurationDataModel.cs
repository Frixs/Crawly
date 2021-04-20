using System;
using System.Collections.Generic;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Data model holding all necessary configuration for <see cref="IndexProcessing"/>.
    /// </summary>
    public class IndexProcessingConfigurationDataModel
    {
        /// <summary>
        /// Processing language used for the processing
        /// </summary>
        public ProcessingLanguage Language { get; } = default;

        /// <summary>
        /// Processing custom regex used for tokenization
        /// </summary>
        public string CustomRegex { get; } = null;

        /// <summary>
        /// Custom stop words as an addition for the processing with the standard ones.
        /// </summary>
        public HashSet<string> CustomStopWords { get; } = null;

        /// <summary>
        /// Should the toknes be lowercased?
        /// </summary>
        public bool ToLowerCase { get; } = false;

        /// <summary>
        /// Should we remove accents from tokens before stemming?
        /// </summary>
        public bool RemoveAccentsBeforeStemming { get; } = false;

        /// <summary>
        /// Should we remove accents from tokens after stemming?
        /// </summary>
        public bool RemoveAccentsAfterStemming { get; } = false;
    }
}
