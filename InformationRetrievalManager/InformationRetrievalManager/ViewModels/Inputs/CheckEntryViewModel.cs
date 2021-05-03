using System;

namespace InformationRetrievalManager
{
    /// <summary>
    /// The view model for a checkbox entry
    /// <summary>
    public class CheckEntryViewModel : BaseEntryViewModel<bool>
    {
        #region Protected Members

        /// <inheritdoc/>
        protected override Func<bool, bool> ValueCustomSetterProcess => null;

        #endregion

        #region Public Properties

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public CheckEntryViewModel() : base()
        {
        }

        #endregion
    }
}
