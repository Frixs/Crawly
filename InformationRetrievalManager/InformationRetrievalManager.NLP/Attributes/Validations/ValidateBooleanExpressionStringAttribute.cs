using InformationRetrievalManager.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Ixs.DNA;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Validation, property attribute of type <see cref="string"/> for specific purpose to validate <see cref="QueryBooleanExpressionParser"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ValidateBooleanExpressionStringAttribute : BaseValidateAttribute, IValidableAttribute<string>
    {
        #region Public Properties

        /// <summary>
        /// Indicates if string is required (TRUE) or it can be empty (FALSE) - never allow null
        /// </summary>
        public bool? IsRequired { get; private set; }

        #endregion

        #region Interface Properties

        /// <inheritdoc/>
        public string ValidationName { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="validationName">Property name to label it in error messages</param>
        /// <param name="declaringType">Declaring type of the fields</param>
        /// <param name="pIsRequired">Field name containing value for <see cref="IsRequired"/> limit</param>
        public ValidateBooleanExpressionStringAttribute(string validationName, Type declaringType,
            string pIsRequired = null
            )
        {
            // Get validation name
            ValidationName = StringHelpers.FormatPascalCase(validationName);

            try
            {
                // IsRequired
                if (pIsRequired != null)
                {
                    var isRequired = declaringType.GetField(pIsRequired);
                    if (isRequired != null)
                        IsRequired = (bool)isRequired.GetValue(null);
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
        public DataValidationResult Validate(string value)
        {
            // Validate
            var errorList = Validate(value, null);

            // Resturn result
            return errorList.Count > 0 ? DataValidationResult.Failed(errorList.ToArray()) : DataValidationResult.Success;
        }

        /// <inheritdoc/>
        public ICollection<DataValidationError> Validate(string value, object none)
        {
            // Return error list
            var errorList = new Collection<DataValidationError>();

            // If value does not exist...
            if (value == null)
            {
                errorList.Add(new DataValidationError
                {
                    Code = "NULL",
                    Description = Localization.Resource.ValidateBooleanExpressionStringAttribute_NullLimitMessage
                });
                // Failure
                return errorList;
            }

            // If IsRequired should be tested...
            if (IsRequired != null && IsRequired == true)
            {
                // If value is empty...
                if (value.Length <= 0)
                    errorList.Add(new DataValidationError
                    {
                        Code = nameof(IsRequired),
                        Description = Localization.Resource.ValidateBooleanExpressionStringAttribute_IsRequiredLimitMessage.Format(ValidationName)
                    });
            }

            // Validate the expression
            var parser = new QueryBooleanExpressionParser();
            // If parsing failed...
            if (!parser.CheckParse(parser.Tokenize(value)))
            {
                errorList.Add(new DataValidationError
                {
                    Code = nameof(parser),
                    Description = Localization.Resource.ValidateBooleanExpressionStringAttribute_ParseFailMessage
                });
            }

            // Return error list
            return errorList;
        }

        #endregion
    }
}
