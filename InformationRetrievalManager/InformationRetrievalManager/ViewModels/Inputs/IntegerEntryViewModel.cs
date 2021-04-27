using System;

namespace InformationRetrievalManager
{
    /// <summary>
    /// The view model for a integer entry to edit an integer value
    /// <summary>
    public class IntegerEntryViewModel : BaseEntryViewModel<int>
    {
        #region Public Properties

        /// <inheritdoc/>
        protected override Func<int, int> ValueCustomSetterProcess => null;

        /// <summary>
        /// The minimal value
        /// </summary>
        public int MinValue { get; set; } = 0; // Default value

        /// <summary>
        /// The maximal value
        /// </summary>
        public int MaxValue { get; set; } = 100; // Default value

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public IntegerEntryViewModel() : base()
        {
        }

        #endregion
    }
}
