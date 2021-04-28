using System.Collections.Generic;

namespace InformationRetrievalManager.Core
{
    /// <summary>
    /// Base interface for all validation attribute types.
    /// Works like a marker for <see cref="IValidableAttribute{T}"/> validations.
    /// </summary>
    public interface IValidableAttribute
    {
        #region Public Properties

        /// <summary>
        /// Formatted property name / label of this validation
        /// </summary>
        string ValidationName { get; }

        #endregion
    }

    /// <summary>
    /// Interface for all validation attribute types.
    /// All validators under this adds logic to validate specific type of the attributed property.
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    public interface IValidableAttribute<T> : IValidableAttribute
    {
        #region Public Methods

        /// <summary>
        /// Validate value
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <returns>Validation result instance</returns>
        DataValidationResult Validate(T value);

        /// <summary>
        /// Validate value
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <param name="none">Placeholder to trigger this method - leave null</param>
        /// <returns>Collection of errors - no erros (empty collection) = value is valid</returns>
        ICollection<DataValidationError> Validate(T value, object none);

        #endregion
    }
}
