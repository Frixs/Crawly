using System;

namespace InformationRetrievalManager
{
    /// <summary>
    /// The view model for a radio button entry
    /// <summary>
    public class RadioEntryViewModel : BaseEntryViewModel<bool>
    {
        #region Protected Members

        /// <inheritdoc/>
        protected override Func<bool, bool> ValueCustomSetterProcess => null;

        #endregion

        #region Public Properties

        /// <summary>
        /// The radio group name
        /// </summary>
        public string GroupName { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public RadioEntryViewModel() : base()
        {
        }

        #endregion
    }
}
