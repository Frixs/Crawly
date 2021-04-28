using System.Collections.Generic;
using System.Linq;

namespace InformationRetrievalManager.Core
{
    /// <summary>
    /// Represents the result of an validation operation.
    /// </summary>
    public class DataValidationResult
    {
        /// <summary>
        /// Returns <see cref="DataValidationResult"/> indicating a successful operation
        /// </summary>
        public static DataValidationResult Success => new DataValidationResult() { Succeeded = true };

        /// <summary>
        /// Flag indicating whether if the operation succeeded or not.
        /// </summary>
        public bool Succeeded { get; protected set; }

        /// <summary>
        /// An IEnumerable of <see cref="DataValidationError"/> containing an errors that occurred during the operation.
        /// </summary>
        public IEnumerable<DataValidationError> Errors { get; protected set; }

        /// <summary>
        /// Creates an <see cref="DataValidationResult"/> indicating a failed operation, with a list of errors if applicable
        /// </summary>
        /// <param name="errors">An optional array of <see cref="DataValidationError"/> which caused the operation to fail</param>
        /// <returns>An result indicating failed operation, with a list of errors if applicable</returns>
        public static DataValidationResult Failed(params DataValidationError[] errors)
        {
            return new DataValidationResult()
            {
                Succeeded = false,
                Errors = errors
            };
        }

        /// <summary>
        /// Converts the value of the current <see cref="DataValidationResult"/> object to its equivalent string representation
        /// </summary>
        /// <returns>A string representation of the current result object</returns>
        /// <remarks>
        ///     If the operation was successful the ToString will return "Succeeded" 
        ///     otherwise it returned "Failed : " followed by a comma delimited 
        ///     list of error codes from its <see cref="Errors"/> collection, if any.
        /// </remarks>
        public override string ToString()
        {
            return Succeeded ? "Succeeded" : ("Failed : " + string.Join(", ", Errors.Select(o => o.Code)));
        }
    }
}
