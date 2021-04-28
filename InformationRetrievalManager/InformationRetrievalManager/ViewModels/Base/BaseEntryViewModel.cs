using InformationRetrievalManager.Core;
using System;

namespace InformationRetrievalManager
{
    /// <summary>
    /// A base view model that describes abstract items for entry view models
    /// </summary>
    public abstract class BaseEntryViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// Short description about the input
        /// </summary>
        public string _description;

        #endregion

        #region Public Properties

        /// <summary>
        /// The label to identify what this value is for
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Property for <see cref="_description"/>
        /// </summary>
        public string Description
        {
            get => _description;
            set => _description = string.IsNullOrWhiteSpace(value) ? null : value;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseEntryViewModel()
        {
        }

        #endregion
    }

    /// <summary>
    /// A base view model that describes abstract items for entry view models
    /// </summary>
    /// <typeparam name="T">Type of the value</typeparam>
    public abstract class BaseEntryViewModel<T> : BaseEntryViewModel
    {
        #region Private Members

        /// Value of property <inheritdoc cref="Value"/>.
        private T _value;

        #endregion

        #region Public Properties

        /// <summary>
        /// Custom setter process for <see cref="OriginalValue"/> and <see cref="EditedValue"/>
        /// Does not set the value, it just return it!
        /// </summary>
        /// <remarks>
        ///     Keep the logic default (set it to null).
        ///     Make custom setter ONLY in special cases.
        ///     E.g. Strings have null as default value, 
        ///          but we want to return empty strings instead
        /// </remarks>
        protected abstract Func<T, T> ValueCustomSetterProcess { get; }

        /// <summary>
        /// The input value.
        /// </summary>
        public T Value
        {
            get => _value;
            set => _value = ValueCustomSetterProcess == null ? value : ValueCustomSetterProcess(value);
        }

        /// <summary>
        /// Validation attribute container to bind specific validation into the entry
        /// </summary>
        public IValidableAttribute<T> Validation { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseEntryViewModel() : base()
        {
        }

        #endregion
    }
}
