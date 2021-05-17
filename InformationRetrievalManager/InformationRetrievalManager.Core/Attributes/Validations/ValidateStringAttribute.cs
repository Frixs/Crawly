using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Ixs.DNA;

namespace InformationRetrievalManager.Core
{
    /// <summary>
    /// Validation, property attribute of type <see cref="string"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ValidateStringAttribute : BaseValidateAttribute, IValidableAttribute<string>
    {
        #region Public Properties

        /// <summary>
        /// Indicates if string allows to be <see langword="null"/> (<see langword="true"/>) or not (<see langword="false"/>).
        /// </summary>
        public bool? AllowNull { get; private set; }

        /// <summary>
        /// Indicates if string is required (TRUE) or it can be empty (FALSE)
        /// </summary>
        public bool? IsRequired { get; private set; }

        /// <summary>
        /// Min length of the string
        /// </summary>
        public ushort? MinLength { get; private set; }

        /// <summary>
        /// Max length of the string
        /// </summary>
        public ushort? MaxLength { get; private set; }

        /// <summary>
        /// Regex - can contain
        /// </summary>
        public string CanContainRegex { get; private set; }

        /// <summary>
        /// Validate JSON format
        /// </summary>
        public bool ValidateJson { get; private set; }

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
        /// <param name="pMinLength">Field name containing value for <see cref="MinLength"/> limit</param>
        /// <param name="pMaxLength">Field name containing value for <see cref="MaxLength"/> limit</param>
        /// <param name="pCanContainRegex">Field name containing value for <see cref="CanContainRegex"/> limit</param>
        public ValidateStringAttribute(string validationName, Type declaringType,
            string pAllowNull = null,
            string pIsRequired = null,
            string pMinLength = null,
            string pMaxLength = null,
            string pCanContainRegex = null,
            bool validateJson = false
            )
        {
            // Get validation name
            ValidationName = StringHelpers.FormatPascalCase(validationName);

            try
            {
                // AllowNull
                if (pAllowNull != null)
                {
                    var allowNull = declaringType.GetField(pAllowNull);
                    if (allowNull != null)
                        AllowNull = (bool)allowNull.GetValue(null);
                }

                // IsRequired
                if (pIsRequired != null)
                {
                    var isRequired = declaringType.GetField(pIsRequired);
                    if (isRequired != null)
                        IsRequired = (bool)isRequired.GetValue(null);
                }

                // MinLength
                if (pMinLength != null)
                {
                    var minLength = declaringType.GetField(pMinLength);
                    if (minLength != null)
                        MinLength = (ushort)minLength.GetValue(null);
                }

                // MaxLength
                if (pMaxLength != null)
                {
                    var maxLength = declaringType.GetField(pMaxLength);
                    if (maxLength != null)
                        MaxLength = (ushort)maxLength.GetValue(null);
                }

                // CanContainRegex
                if (pCanContainRegex != null)
                {
                    var canContainRegex = declaringType.GetField(pCanContainRegex);
                    if (canContainRegex != null)
                        CanContainRegex = (string)canContainRegex.GetValue(null);
                }

                // ValidateJson
                ValidateJson = validateJson;
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
            if (value == null && AllowNull == false)
            {
                errorList.Add(new DataValidationError
                {
                    Code = "NULL",
                    Description = Localization.Resource.ValidateStringAttribute_NullLimitMessage
                });
                // Failure
                return errorList;
            }

            if (value != null)
            {
                // If IsRequired should be tested...
                if (IsRequired != null && IsRequired == true)
                {
                    // If value is empty...
                    if (string.IsNullOrWhiteSpace(value))
                        errorList.Add(new DataValidationError
                        {
                            Code = nameof(IsRequired),
                            Description = Localization.Resource.ValidateStringAttribute_IsRequiredLimitMessage.Format(ValidationName)
                        });
                }

                // If MinLength should be tested...
                if (MinLength != null)
                {
                    // If value exceeds the min length...
                    if (value.Length < MinLength)
                        errorList.Add(new DataValidationError
                        {
                            Code = nameof(MinLength),
                            Description = Localization.Resource.ValidateStringAttribute_MinLengthLimitMessage.Format(ValidationName, MinLength)
                        });
                }

                // If MaxLength should be tested...
                if (MaxLength != null)
                {
                    // If value exceeds the max length...
                    if (value.Length > MaxLength)
                        errorList.Add(new DataValidationError
                        {
                            Code = nameof(MaxLength),
                            Description = Localization.Resource.ValidateStringAttribute_MaxLengthLimitMessage.Format(ValidationName, MaxLength)
                        });
                }

                // If CanContainRegex should be tested...
                if (CanContainRegex != null)
                {
                    if (IsRequired != true && string.IsNullOrEmpty(value))
                    {
                        // Do not validate if value is not required and value is empty at the same time
                    }
                    // If value does not match regex...
                    else if (!Regex.IsMatch(value, CanContainRegex))
                        errorList.Add(new DataValidationError
                        {
                            Code = nameof(CanContainRegex),
                            Description = Localization.Resource.ValidateStringAttribute_CanContainRegexLimitMessage.Format(ValidationName)
                        });
                }

                // If ValidateJson should be tested...
                if (ValidateJson)
                {
                    if (IsRequired != true && string.IsNullOrEmpty(value))
                    {
                        // Do not validate if value is not required and value is empty at the same time
                    }
                    else if (!string.IsNullOrEmpty(value) && !IsValidJson(value))
                        errorList.Add(new DataValidationError
                        {
                            Code = nameof(ValidateJson),
                            Description = Localization.Resource.ValidateStringAttribute_ValidateJsonLimitMessage.Format(ValidationName)
                        });
                }
            }

            // Return error list
            return errorList;
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Check if string is valid JSON
        /// </summary>
        /// <param name="strInput">The string input</param>
        /// <returns>TRUE = is valid, FALSE otherwise</returns>
        private static bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || // For object
                (strInput.StartsWith("[") && strInput.EndsWith("]")) // For array
                )
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
                catch (Exception) // Some other exception
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
