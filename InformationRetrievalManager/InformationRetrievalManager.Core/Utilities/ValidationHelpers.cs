using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace InformationRetrievalManager.Core
{
    /// <summary>
    /// Helpers for the validate attributes.
    /// </summary>
    public static class ValidationHelpers
    {
        /// <summary>
        /// Finds and gets validate attribute of specific property.
        /// </summary>
        /// <typeparam name="TModel">Model type related to the property.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <typeparam name="TValidateAttribute">Searched validate attribute</typeparam>
        /// <param name="property">Property selection</param>
        /// <returns>Validate attribute of specific property.</returns>
        public static TValidateAttribute GetPropertyValidateAttribute<TModel, TProperty, TValidateAttribute>(Expression<Func<TModel, TProperty>> property)
            where TValidateAttribute : BaseValidateAttribute
        {
            return typeof(TModel).GetPropertyAttribute<TValidateAttribute>(GetPropertyName(property));
        }

        /// <summary>
        /// Validates model according to assigned validate attributes.
        /// </summary>
        /// <param name="modelType">Model type validation</param>
        /// <param name="value">Model to validate</param>
        /// <returns>Validation results</returns>
        public static ICollection<DataValidationError> ValidateModel(Type modelType, object value)
        {
            return modelType.GetAttribute<ValidableModelAttribute>()
                .Validate(value, null);
        }

        #region Helpers

        /// <summary>
        /// Returns the name of the specified property of the specified type.
        /// </summary>
        /// <typeparam name="T">The type the property is a member of.</typeparam>
        /// <param name="property">The property.</param>
        /// <returns>The property name.</returns>
        private static string GetPropertyName<TModel, TProperty>(Expression<Func<TModel, TProperty>> property)
        {
            LambdaExpression lambda = property;
            MemberExpression memberExpression;

            if (lambda.Body is UnaryExpression unaryExpression)
                memberExpression = (MemberExpression)(unaryExpression.Operand);
            else
                memberExpression = (MemberExpression)(lambda.Body);

            return ((PropertyInfo)memberExpression.Member).Name;
        }

        #endregion
    }
}
