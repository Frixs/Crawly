using InformationRetrievalManager.Core;
using InformationRetrievalManager.Relational;
using System;
using System.Linq;
using Ixs.DNA;

namespace InformationRetrievalManager
{
    /// <summary>
    /// Context for control <see cref="ProcessingConfigurationForm"/>.
    /// </summary>
    public class ProcessingConfigurationFormContext : BaseViewModel
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
                Description = "The language used to process the texts.",
                Validation = null,
                Value = IndexProcessingConfigurationDataModel.Language_DefaultValue,
                ValueList = Enum.GetValues(typeof(ProcessingLanguage)).Cast<ProcessingLanguage>().ToList()
            };
            CustomRegexEntry = new TextEntryViewModel
            {
                Label = "Custom Regex",
                Description = "The custom regex used for searching tokens.",
                Validation = ValidationHelpers.GetPropertyValidateAttribute<IndexProcessingConfigurationDataModel, string, ValidateStringAttribute>(o => o.CustomRegex),
                Value = IndexProcessingConfigurationDataModel.CustomRegex_DefaultValue,
                Placeholder = "E.g. [a-zA-Z]+",
                MaxLength = IndexProcessingConfigurationDataModel.CustomRegex_MaxLength
            };
            CustomStopWordsEntry = new TextEntryViewModel
            {
                Label = "Custom Stop Words",
                Description = "Additional stop-words that are additively added to the standard language set during removal. Use '{0}' to split the words.".Format(IndexProcessingConfiguration.CustomStopWords_Separator),
                Validation = null,
                Value = "",
                Placeholder = "E.g. hello,world",
                MaxLength = 999
            };
            ToLowerCaseEntry = new CheckEntryViewModel
            {
                Label = "To Lower Case",
                Description = "It puts the entire text int lower-case form during the processing.",
                Validation = null,
                Value = IndexProcessingConfigurationDataModel.ToLowerCase_DefaultValue
            };
            RemoveAccentsBeforeStemmingEntry = new CheckEntryViewModel
            {
                Label = "Before Stemming",
                Description = "Remove accents from the entire text before the Stemming process.",
                Validation = null,
                Value = IndexProcessingConfigurationDataModel.RemoveAccentsBeforeStemming_DefaultValue
            };
            RemoveAccentsAfterStemmingEntry = new CheckEntryViewModel
            {
                Label = "After Stemming",
                Description = "Remove accents from the entire text after the Stemming process.",
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

        /// <summary>
        /// Set the context readonly access to each entry in this context.
        /// </summary>
        /// <returns>Return self for chaining.</returns>
        public ProcessingConfigurationFormContext ReadOnly(bool readOnlyInputs)
        {
            LanguageEntry.IsReadOnly =
            CustomRegexEntry.IsReadOnly =
            CustomStopWordsEntry.IsReadOnly =
            ToLowerCaseEntry.IsReadOnly =
            RemoveAccentsBeforeStemmingEntry.IsReadOnly =
            RemoveAccentsAfterStemmingEntry.IsReadOnly = readOnlyInputs;

            return this;
        }

        #endregion
    }
}
