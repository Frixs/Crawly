using InformationRetrievalManager.Core;
using InformationRetrievalManager.Relational;
using System;
using System.Linq;

namespace InformationRetrievalManager
{
    /// <summary>
    /// Context for control <see cref="ProcessingConfigurationForm"/>.
    /// </summary>
    public class ProcessingConfigurationFormContext
    {
        #region Public Properties

        public ComboEntryViewModel<ProcessingLanguage> LanguageEntry { get; set; }
        public TextEntryViewModel CustomRegexEntry { get; set; }
        public TextEntryViewModel CustomStopWordsEntry { get; set; }
        public CheckEntryViewModel ToLowerCaseEntry { get; set; }
        public CheckEntryViewModel RemoveAccentsBeforeStemmingEntry { get; set; }
        public CheckEntryViewModel RemoveAccentsAfterStemmingEntry { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ProcessingConfigurationFormContext()
        {
            LanguageEntry = new ComboEntryViewModel<ProcessingLanguage>()
            {
                Label = "Language",
                Description = "",
                Validation = null,
                Value = IndexProcessingConfigurationDataModel.Language_DefaultValue,
                ValueList = Enum.GetValues(typeof(ProcessingLanguage)).Cast<ProcessingLanguage>().ToList()
            };
            CustomRegexEntry = new TextEntryViewModel
            {
                Label = "Custom Regex",
                Description = "",
                Validation = ValidationHelpers.GetPropertyValidateAttribute<IndexProcessingConfigurationDataModel, string, ValidateStringAttribute>(o => o.CustomRegex),
                Value = IndexProcessingConfigurationDataModel.CustomRegex_DefaultValue,
                Placeholder = "",
                MaxLength = IndexProcessingConfigurationDataModel.CustomRegex_MaxLength
            };
            CustomStopWordsEntry = new TextEntryViewModel
            {
                Label = "Custom Stop Words",
                Description = "",
                Validation = null,
                Value = "",
                Placeholder = "",
                MaxLength = 999
            };
            ToLowerCaseEntry = new CheckEntryViewModel
            {
                Label = "To Lower Case",
                Description = "",
                Validation = null,
                Value = IndexProcessingConfigurationDataModel.ToLowerCasex_DefaultValue
            };
            RemoveAccentsBeforeStemmingEntry = new CheckEntryViewModel
            {
                Label = "Remove Accents (before stemming)",
                Description = "",
                Validation = null,
                Value = IndexProcessingConfigurationDataModel.RemoveAccentsBeforeStemming_DefaultValue
            };
            RemoveAccentsAfterStemmingEntry = new CheckEntryViewModel
            {
                Label = "Remove Accents (after stemming)",
                Description = "",
                Validation = null,
                Value = IndexProcessingConfigurationDataModel.RemoveAccentsAfterStemming_DefaultValue
            };
        }

        /// <summary>
        /// Initialize the context with values for each entry in this context.
        /// </summary>
        /// <returns>Return self for chaining.</returns>
        public ProcessingConfigurationFormContext Set(
            ProcessingLanguage language,
            string customRegex,
            string customStopWords,
            bool toLowerCase,
            bool removeAccentsBeforeStemming,
            bool removeAccentsAfterStemming)
        {
            LanguageEntry.Value = language;
            CustomRegexEntry.Value = customRegex;
            CustomStopWordsEntry.Value = customStopWords;
            ToLowerCaseEntry.Value = toLowerCase;
            RemoveAccentsBeforeStemmingEntry.Value = removeAccentsBeforeStemming;
            RemoveAccentsAfterStemmingEntry.Value = removeAccentsAfterStemming;

            return this;
        }

        #endregion
    }
}
