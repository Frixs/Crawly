using System.Collections.Generic;

namespace InformationRetrievalManager.Core
{
    /// <summary>
    /// Data model holding all necessary configuration for index processing in NLP project.
    /// </summary>
    public class IndexProcessingConfiguration
    {
        #region Limit Constants

        public static readonly ProcessingLanguage Language_DefaultValue = default;

        public static readonly bool CustomRegex_IsRequired = false;
        public static readonly short CustomRegex_MaxLength = 255;
        public static readonly string CustomRegex_DefaultValue = "";

        public static readonly bool ToLowerCasex_DefaultValue = true;

        public static readonly bool RemoveAccentsBeforeStemming_DefaultValue = true;

        public static readonly bool RemoveAccentsAfterStemming_DefaultValue = false;

        #endregion

        /// <summary>
        /// Processing language used for the processing
        /// </summary>
        [ValidateIgnore]
        public ProcessingLanguage Language { get; set; } = Language_DefaultValue;

        /// <summary>
        /// Processing custom regex used for tokenization
        /// </summary>
        [ValidateString(nameof(CustomRegex), typeof(IndexProcessingConfiguration),
            pIsRequired: nameof(CustomRegex_IsRequired),
            pMaxLength: nameof(CustomRegex_MaxLength))]
        public string CustomRegex { get; set; } = null;

        /// <summary>
        /// Custom stop words as an addition for the processing with the standard ones.
        /// </summary>
        [ValidateIgnore]
        public HashSet<string> CustomStopWords { get; set; } = null;

        /// <summary>
        /// Should the toknes be lowercased?
        /// </summary>
        [ValidateIgnore]
        public bool ToLowerCase { get; set; } = ToLowerCasex_DefaultValue;

        /// <summary>
        /// Should we remove accents from tokens before stemming?
        /// </summary>
        [ValidateIgnore]
        public bool RemoveAccentsBeforeStemming { get; set; } = RemoveAccentsBeforeStemming_DefaultValue;

        /// <summary>
        /// Should we remove accents from tokens after stemming?
        /// </summary>
        [ValidateIgnore]
        public bool RemoveAccentsAfterStemming { get; set; } = RemoveAccentsAfterStemming_DefaultValue;
    }
}
