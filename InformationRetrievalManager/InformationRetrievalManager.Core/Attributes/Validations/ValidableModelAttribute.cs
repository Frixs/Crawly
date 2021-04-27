using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace InformationRetrievalManager.Core
{
    /// <summary>
    /// Mark the class (model) as validable and add functinality into it to validate it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ValidableModelAttribute : Attribute, IValidableAttribute<object>
    {
        #region Public Properties

        /// <summary>
        /// Declaring type (type of the model)
        /// </summary>
        public Type DeclaringType { get; }

        #endregion

        #region Interface Properties

        /// <inheritdoc/>
        public string ValidationName => throw new NotImplementedException();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="declaringType">Declaring type</param>
        public ValidableModelAttribute(Type declaringType)
        {
            DeclaringType = declaringType;
        }

        #endregion

        #region Interface Methods

        /// <inheritdoc/>
        public DataValidationResult Validate(object value)
        {
            // Validate
            var errorList = Validate(value, null);

            // Return validation result based on error list
            return errorList.Count > 0 ? DataValidationResult.Failed(errorList.ToArray()) : DataValidationResult.Success;
        }

        /// <inheritdoc/>
        public ICollection<DataValidationError> Validate(object value, object none)
        {
            // Check if the value type corresponds to the declaring type...
            if (value == null || DeclaringType != value.GetType())
            {
                Console.Error.WriteLine("Fatal error occurred during validation");
                Debugger.Break();
            }

            // Result errors
            var errorList = new List<DataValidationError>();

            // Get all properties
            PropertyInfo[] properties = value.GetType().GetProperties();

            // Go through the properties...
            foreach (PropertyInfo pi in properties)
            {
                // Get validation attributes...
                foreach (var attribute in pi.GetCustomAttributes(typeof(BaseValidateAttribute)))
                {
                    // Validate Ignore
                    if (attribute.GetType() == typeof(ValidateIgnoreAttribute))
                    {
                        // Ignore validation
                    }
                    // Validate String
                    else if (attribute.GetType() == typeof(ValidateStringAttribute))
                    {
                        errorList.AddRange(
                            ((ValidateStringAttribute)attribute).Validate((string)pi.GetValue(value, null), null)
                            );
                    }
                    // Validate Integer
                    else if (attribute.GetType() == typeof(ValidateIntegerAttribute))
                    {
                        errorList.AddRange(
                            ((ValidateIntegerAttribute)attribute).Validate(
                                pi.PropertyType == typeof(int)
                                    ? (int)pi.GetValue(value, null)
                                    : (pi.PropertyType == typeof(short)
                                        ? (short)pi.GetValue(value, null)
                                        : (byte)pi.GetValue(value, null)),
                                null)
                            );
                    }
                    // Validate TimeSpan
                    else if (attribute.GetType() == typeof(ValidateTimespanAttribute))
                    {
                        errorList.AddRange(
                            ((ValidateTimespanAttribute)attribute).Validate((TimeSpan)pi.GetValue(value, null), null)
                            );
                    }
                    // Validate Collection
                    else if (attribute.GetType() == typeof(ValidateCollectionAttribute))
                    {
                        // Get property collection value
                        var pValue = ((IEnumerable)pi.GetValue(value, null)).Cast<object>().ToList();

                        errorList.AddRange(
                            ((ValidateCollectionAttribute)attribute).Validate(pValue, null)
                            );
                    }
                    // Otherwise, not specified...
                    else
                    {
                        Console.Error.WriteLine($"Fatal error occurred during validation - Some validation attributes are not specified in this switch (Type: {attribute.GetType()}).");
                        Debugger.Break();
                    }
                }
            }

            // Return error list
            return errorList;
        }

        #endregion
    }
}
