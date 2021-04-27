using System.Windows.Markup;
using System.Globalization;
using System.Windows.Controls;
using InformationRetrievalManager.Core;

namespace InformationRetrievalManager
{
    /// <summary>
    /// Validation rule for data of type <see cref="int"/>
    /// </summary>
    [ContentProperty(nameof(ValidationProperty))]
    public class DataIntegerValidationRule : BaseValidationRule
    {
        /// <summary>
        /// Validation dependency property reference
        /// </summary>
        public ValidationIntegerAttributeProperty ValidationProperty { get; set; }

        /// <inheritdoc/>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // Get value
            var val = (int)GetBoundValue(value);

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
