using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Ixs.DNA;

namespace InformationRetrievalManager.Core
{
    /// <summary>
    /// Validation, property attribute of type <see cref="Collection{T}"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ValidateCollectionAttribute : BaseValidateAttribute, IValidableAttribute<ICollection<object>>
    {
        #region Public Properties

        /// <summary>
        /// Max value
        /// </summary>
        public int? MaxCount { get; private set; }

        #endregion

        #region Interface Properties

        /// <inheritdoc/>
        public string ValidationName { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="validationName">Property name to label validation in error messages</param>
        /// <param name="declaringType">Declaring type of the fields</param>
        /// <param name="pMaxCount">Field name containing value for <see cref="MaxCount"/> limit</param>
        public ValidateCollectionAttribute(string validationName, Type declaringType,
            string pMaxCount = null
            )
        {
            // Get validation name
            ValidationName = StringHelpers.FormatPascalCase(validationName);

            try
            {
                // MaxCount
                if (pMaxCount != null)
                {
                    var maxValue = declaringType.GetField(pMaxCount);
                    if (maxValue != null)
                        MaxCount = (int)maxValue.GetValue(null);
                }
            }
            catch (Exception ex)
            {
                // Fatal error during attribute setup - most probably, type of limit properties does not fit with the input parameters
                Console.Error.WriteLine(ex.Message);
                Debugger.Break();
            }
        }

        #endregion

        #region Interface Methods

        /// <inheritdoc/>
        public DataValidationResult Validate(ICollection<object> value)
        {
            // Validate
            var errorList = Validate(value, null);

            // Resturn result
            return errorList.Count > 0 ? DataValidationResult.Failed(errorList.ToArray()) : DataValidationResult.Success;
        }

        /// <inheritdoc/>
        public ICollection<DataValidationError> Validate(ICollection<object> value, object none)
        {
            // Return error list
            var errorList = new Collection<DataValidationError>();

            // If MaxValue should be tested...
            if (MaxCount != null)
            {
                // If value is greater than max value...
                if (value.Count > MaxCount)
                    errorList.Add(new DataValidationError
                    {
                        Code = nameof(MaxCount),
                        Description = Localization.Resource.ValidateCollectionAttribute_MaxCountLimitMessage.Format(ValidationName, MaxCount)
                    });
            }

            // Return error list
            return errorList;
        }

        #endregion
    }
}
