using System.Collections.Generic;

namespace InformationRetrievalManager.Core
{
    /// <summary>
    /// Data model holding all necessary configuration for index processing in NLP project.
    /// </summary>
    public class IndexProcessingConfiguration
    {
        /// <summary>
        /// Processing language used for the processing
        /// </summary>
        public ProcessingLanguage Language { get; set; } = default;

        /// <summary>
        /// Processing custom regex used for tokenization
        /// </summary>
        public string CustomRegex { get; set; } = null;

        /// <summary>
        /// Custom stop words as an addition for the processing with the standard ones.
        /// </summary>
        public HashSet<string> CustomStopWords { get; set; } = null;

        /// <summary>
        /// Should the toknes be lowercased?
        /// </summary>
        public bool ToLowerCase { get; set; } = false;

        /// <summary>
        /// Should we remove accents from tokens before stemming?
        /// </summary>
        public bool RemoveAccentsBeforeStemming { get; set; } = false;

        /// <summary>
        /// Should we remove accents from tokens after stemming?
        /// </summary>
        public bool RemoveAccentsAfterStemming { get; set; } = false;
    }
}
