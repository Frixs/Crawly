using System;
using System.Collections.Generic;
using System.Linq;

namespace InformationRetrievalManager.Core
{
    /// <summary>
    /// Extensions methods for <see cref="DataValidationError"/> / <see cref="DataValidationResult"/>
    /// </summary>
    public static class DataValidationExtensions
    {
        /// <summary>
        /// Extensions methods for <see cref="DataValidationError"/> class
        /// Combines all errors into a single string
        /// </summary>
        /// <param name="errors">The error to aggregate</param>
        /// <returns>Returns a string with each error separated by a new line</returns>
        public static string AggregateErrors(this IEnumerable<DataValidationError> errors)
        {
            // Get all erros into a list
            return errors?.ToList()
                // Grab their description
                .Select(f => f.Description)
                // And combine them with a newline separator
                .Aggregate((a, b) => $"{a}{Environment.NewLine}{b}");
        }
    }
}
