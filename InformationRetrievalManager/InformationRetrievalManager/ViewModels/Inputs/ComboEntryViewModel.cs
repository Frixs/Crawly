using System;
using System.Collections.ObjectModel;

namespace InformationRetrievalManager
{
    /// <summary>
    /// The view model for a combobox entry to edit a T value
    /// <summary>
    /// <typeparam name="T">Type of the value</typeparam>
    public class ComboEntryViewModel<T> : BaseEntryViewModel<T>
    {
        #region Public Properties

        /// <inheritdoc/>
        protected override Func<T, T> ValueCustomSetterProcess => null;

        /// <summary>
        /// List of possible values in combo box
        /// </summary>
        public ObservableCollection<T> ValueList { get; set; } = new ObservableCollection<T>();

        /// <summary>
        /// If <see cref="T"/> is object and we need to display its property, this nameof property specify that
        /// </summary>
        public string DisplayMemberPath { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ComboEntryViewModel() : base()
        {
        }

        #endregion
    }
}
