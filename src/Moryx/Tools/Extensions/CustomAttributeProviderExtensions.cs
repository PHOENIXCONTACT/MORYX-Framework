using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Marvin.Tools
{
    /// <summary>
    /// Extensions for the <see cref="ICustomAttributeProvider"/>
    /// </summary>
    public static class CustomAttributeProviderExtensions
    {
        /// <summary>
        /// Returns the custom attribute of this attribute provider or null if attribute was not found
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        /// <param name="attributeProvider">Provider of the attribute</param>
        public static TAttribute GetCustomAttribute<TAttribute>(this ICustomAttributeProvider attributeProvider)
            where TAttribute : Attribute
        {
            return attributeProvider.GetCustomAttribute<TAttribute>(true);
        }

        /// <summary>
        /// Returns the custom attribute of this attribute provider or null if attribute was not found
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        /// <param name="attributeProvider">Provider of the attribute</param>
        /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute. </param>
        public static TAttribute GetCustomAttribute<TAttribute>(this ICustomAttributeProvider attributeProvider, bool inherit)
            where TAttribute : Attribute
        {
            return (TAttribute)attributeProvider.GetCustomAttributes(typeof(TAttribute), inherit).FirstOrDefault();
        }

        /// <summary>
        /// Returns an array of attributes defined on this member or an empty array, if no attribute was found
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        /// <param name="attributeProvider">Provider of the attribute</param>
        public static TAttribute[] GetCustomAttributes<TAttribute>(this ICustomAttributeProvider attributeProvider)
            where TAttribute : Attribute
        {
            return attributeProvider.GetCustomAttributes<TAttribute>(true);
        }

        /// <summary>
        /// Returns an array of attributes defined on this member or an empty array, if no attribute was found
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        /// <param name="attributeProvider">Provider of the attribute</param>
        /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute. </param>
        public static TAttribute[] GetCustomAttributes<TAttribute>(this ICustomAttributeProvider attributeProvider, bool inherit)
            where TAttribute : Attribute
        {
            return (TAttribute[])attributeProvider.GetCustomAttributes(typeof(TAttribute), inherit);
        }

        /// <summary>
        /// Tries to get attributes defining display names on this attribute provider.
        /// If no attribute was found, null will be the result.
        /// The chain follows: <see cref="DisplayAttribute"/>, <see cref="ClassDisplayAttribute"/> and <see cref="DisplayNameAttribute"/>
        /// </summary>
        /// <param name="attributeProvider">Provider of the attributes</param>
        /// <returns>The value of the display name or <c>null</c>.</returns>
        public static string GetDisplayName(this ICustomAttributeProvider attributeProvider)
        {
            var name = attributeProvider.GetCustomAttribute<DisplayAttribute>(false)?.GetName();
            if (string.IsNullOrEmpty(name))
                name = attributeProvider.GetCustomAttribute<ClassDisplayAttribute>(false)?.GetName();

            if (string.IsNullOrEmpty(name))
                name = attributeProvider.GetCustomAttribute<DisplayNameAttribute>(false)?.DisplayName;

            return name;
        }

        /// <summary>
        /// Tries to get attributes defining a description on this attribute provider.
        /// If no attribute was found, null will be the result.
        /// The chain follows: <see cref="DisplayAttribute"/>, <see cref="ClassDisplayAttribute"/> and <see cref="DescriptionAttribute"/>
        /// </summary>
        /// <param name="attributeProvider">Provider of the attributes</param>
        /// <returns>The value of the description or <c>null</c>.</returns>
        public static string GetDescription(this ICustomAttributeProvider attributeProvider)
        {
            var description = attributeProvider.GetCustomAttribute<DisplayAttribute>(false)?.GetDescription();

            if (string.IsNullOrEmpty(description))
                description = attributeProvider.GetCustomAttribute<ClassDisplayAttribute>(false)?.GetDescription();

            if (string.IsNullOrEmpty(description))
                description = attributeProvider.GetCustomAttribute<DescriptionAttribute>(false)?.Description;

            return description;
        }
    }
}
