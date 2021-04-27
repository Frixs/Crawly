using System.Windows.Markup;
using System.Globalization;
using System.Windows.Controls;
using InformationRetrievalManager.Core;

namespace InformationRetrievalManager
{
    /// <summary>
    /// Validation rule for data of type <see cref="string"/>
    /// </summary>
    [ContentProperty(nameof(ValidationProperty))]
    public class DataStringValidationRule : BaseValidationRule
    {
        /// <summary>
        /// Validation dependency property reference
        /// </summary>
        public ValidationStringAttributeProperty ValidationProperty { get; set; }

        /// <inheritdoc/>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // Get value
            var val = (string)GetBoundValue(value);

            // Check if validation is NOT attached...
            if (ValidationProperty == null || ValidationProperty.Value == null)
                // Validation does not exist, leave it without error
                return ValidationResult.ValidResult;

            // Run validation
            var errors = ValidationProperty.Value.Validate(val, null);
            // If validation has any errors...
            if (errors.Count > 0)
                return new ValidationResult(false, errors.AggregateErrors());

            return ValidationResult.ValidResult;
        }
    }
}
