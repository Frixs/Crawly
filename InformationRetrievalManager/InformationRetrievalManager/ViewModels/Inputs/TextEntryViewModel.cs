using System;

namespace InformationRetrievalManager
{
    /// <summary>
    /// The view model for a text entry to edit a string value
    /// <summary>
    public class TextEntryViewModel : BaseEntryViewModel<string>
    {
        #region Protected Members

        /// <inheritdoc/>
        /// <remarks>
        ///     Make sure to always return empty string instead of null
        ///     if user does not enter anything.
        /// </remarks>
        protected override Func<string, string> ValueCustomSetterProcess => (value) => string.IsNullOrWhiteSpace(value) ? string.Empty : value;

        #endregion

        #region Public Properties

        /// <summary>
        /// Placeholder in the input field
        /// </summary>
        public string Placeholder { get; set; } = "";

        /// <summary>
        /// The maximal length of value
        /// </summary>
        public int MaxLength { get; set; } = 100; // Default value

        #endregion

        #region Constructor 

        /// <summary>
        /// Default constructor
        /// </summary>
        public TextEntryViewModel() : base()
        {
        }

        #endregion
    }
}
