using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Ixs.DNA;

namespace InformationRetrievalManager.Core
{
    /// <summary>
    /// Validation, property attribute of type <see cref="TimeSpan"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ValidateTimespanAttribute : BaseValidateAttribute, IValidableAttribute<TimeSpan>
    {
        #region Public Properties

        /// <summary>
        /// Min value
        /// </summary>
        public TimeSpan? MinValue { get; private set; }

        /// <summary>
        /// Max value
        /// </summary>
        public TimeSpan? MaxValue { get; private set; }

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
        /// <param name="pMinValue">Field name containing value for <see cref="MinValue"/> limit</param>
        /// <param name="pMaxValue">Field name containing value for <see cref="MaxValue"/> limit</param>
        public ValidateTimespanAttribute(string validationName, Type declaringType,
            string pMinValue = null,
            string pMaxValue = null
            )
        {
            // Get validation name
            ValidationName = StringHelpers.FormatPascalCase(validationName);

            try
            {
                // MinValue
                if (pMinValue != null)
                {
                    var minValue = declaringType.GetField(pMinValue);
                    if (minValue != null)
                        MinValue = (TimeSpan)minValue.GetValue(null);
                }

                // MaxValue
                if (pMaxValue != null)
                {
                    var maxValue = declaringType.GetField(pMaxValue);
                    if (maxValue != null)
                        MaxValue = (TimeSpan)maxValue.GetValue(null);
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
        public DataValidationResult Validate(TimeSpan value)
        {
            // Validate
            var errorList = Validate(value, null);

            // Resturn result
            return errorList.Count > 0 ? DataValidationResult.Failed(errorList.ToArray()) : DataValidationResult.Success;
        }

        /// <inheritdoc/>
        public ICollection<DataValidationError> Validate(TimeSpan value, object none)
        {
            // Return error list
            var errorList = new Collection<DataValidationError>();

            // If value does not exist...
            if (value == null)
            {
                errorList.Add(new DataValidationError
                {
                    Code = "NULL",
                    Description = Localization.Resource.ValidateTimespanAttribute_NullLimitMessage
                });
                // Failure
                return errorList;
            }

            // If MinValue should be tested...
            if (MinValue != null)
            {
                // If value is less than min value...
                if (value < MinValue)
                    errorList.Add(new DataValidationError
                    {
                        Code = nameof(MinValue),
                        Description = Localization.Resource.ValidateTimespanAttribute_MinValueLimitMessage.Format(ValidationName, MinValue)
                    });
            }

            // If MaxValue should be tested...
            if (MaxValue != null)
            {
                // If value is greater than max value...
                if (value > MaxValue)
                    errorList.Add(new DataValidationError
                    {
                        Code = nameof(MaxValue),
                        Description = Localization.Resource.ValidateTimespanAttribute_MaxValueLimitMessage.Format(ValidationName, MaxValue)
                    });
            }

            // Return error list
            return errorList;
        }

        #endregion
    }
}
