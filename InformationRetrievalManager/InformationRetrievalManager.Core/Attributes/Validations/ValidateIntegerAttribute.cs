using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace InformationRetrievalManager.Core
{
    /// <summary>
    /// Validation, property attribute of type <see cref="int"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ValidateIntegerAttribute : BaseValidateAttribute, IValidableAttribute<int>
    {
        #region Public Properties

        /// <summary>
        /// Min value
        /// </summary>
        public int? MinValue { get; private set; }

        /// <summary>
        /// Max value
        /// </summary>
        public int? MaxValue { get; private set; }

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
        public ValidateIntegerAttribute(string validationName, Type declaringType,
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
                        MinValue = (int)minValue.GetValue(null);
                }

                // MaxValue
                if (pMaxValue != null)
                {
                    var maxValue = declaringType.GetField(pMaxValue);
                    if (maxValue != null)
                        MaxValue = (int)maxValue.GetValue(null);
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
        public DataValidationResult Validate(int value)
        {
            // Validate
            var errorList = Validate(value, null);

            // Resturn result
            return errorList.Count > 0 ? DataValidationResult.Failed(errorList.ToArray()) : DataValidationResult.Success;
        }

        /// <inheritdoc/>
        public ICollection<DataValidationError> Validate(int value, object none)
        {
            // Return error list
            var errorList = new Collection<DataValidationError>();

            // If MinValue should be tested...
            if (MinValue != null)
            {
                // If value is less than min value...
                if (value < MinValue)
                    errorList.Add(new DataValidationError
                    {
                        Code = nameof(MinValue),
                        Description = $"{ValidationName} allows minimal value {MinValue}."
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
                        Description = $"{ValidationName} allows maximal value {MaxValue}."
                    });
            }

            // Return error list
            return errorList;
        }

        #endregion
    }
}
