using System;
using System.Reflection;

namespace InformationRetrievalManager.Core
{
    /// <summary>
    /// Type extensions
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Reflection based attribute getter
        /// </summary>
        /// <typeparam name="T">Attribute type</typeparam>
        /// <param name="type">The type</param>
        /// <returns>Attribute new instance</returns>
        public static T GetAttribute<T>(this Type type) where T : Attribute
        {
            return (T)type.GetCustomAttribute(typeof(T));
        }

        /// <summary>
        /// Reflection based property attribute getter
        /// </summary>
        /// <typeparam name="T">Attribute type</typeparam>
        /// <param name="type">Class type of the property</param>
        /// <param name="propertyName">The property name</param>
        /// <returns>Attribute new instance</returns>
        public static T GetPropertyAttribute<T>(this Type type, string propertyName) where T : Attribute
        {
            return (T)type.GetProperty(propertyName)?.GetCustomAttribute(typeof(T));
        }
    }
}
