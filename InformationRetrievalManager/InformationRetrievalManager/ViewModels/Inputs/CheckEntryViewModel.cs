using System;

namespace InformationRetrievalManager
{
    /// <summary>
    /// The view model for a checkbox entry
    /// <summary>
    public class CheckEntryViewModel : BaseEntryViewModel<bool>
    {
        #region Public Properties

        /// <inheritdoc/>
        protected override Func<bool, bool> ValueCustomSetterProcess => null;

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
