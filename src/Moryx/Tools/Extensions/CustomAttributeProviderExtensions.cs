// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Moryx.Tools;

/// <summary>
/// Extensions for the <see cref="ICustomAttributeProvider"/>
/// </summary>
public static class CustomAttributeProviderExtensions
{
    /// <param name="attributeProvider">Provider of the attribute</param>
    extension(ICustomAttributeProvider attributeProvider)
    {
        /// <summary>
        /// Returns the custom attribute of this attribute provider or null if attribute was not found
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        public TAttribute GetCustomAttribute<TAttribute>()
            where TAttribute : Attribute
        {
            return attributeProvider.GetCustomAttribute<TAttribute>(true);
        }

        /// <summary>
        /// Returns the custom attribute of this attribute provider or null if attribute was not found
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute. </param>
        public TAttribute GetCustomAttribute<TAttribute>(bool inherit)
            where TAttribute : Attribute
        {
            return (TAttribute)attributeProvider.GetCustomAttributes(typeof(TAttribute), inherit).FirstOrDefault();
        }

        /// <summary>
        /// Returns an array of attributes defined on this member or an empty array, if no attribute was found
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        public TAttribute[] GetCustomAttributes<TAttribute>()
            where TAttribute : Attribute
        {
            return attributeProvider.GetCustomAttributes<TAttribute>(true);
        }

        /// <summary>
        /// Returns an array of attributes defined on this member or an empty array, if no attribute was found
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute. </param>
        public TAttribute[] GetCustomAttributes<TAttribute>(bool inherit)
            where TAttribute : Attribute
        {
            return (TAttribute[])attributeProvider.GetCustomAttributes(typeof(TAttribute), inherit);
        }

        /// <summary>
        /// Tries to get attributes defining display names on this attribute provider.
        /// If no attribute was found, null will be the result.
        /// The chain follows: <see cref="DisplayAttribute"/> and <see cref="DisplayNameAttribute"/>
        /// </summary>
        /// <returns>The value of the display name or <c>null</c>.</returns>
        public string GetDisplayName()
        {
            var name = attributeProvider.GetCustomAttribute<DisplayAttribute>(false)?.GetName();

            if (string.IsNullOrEmpty(name))
                name = attributeProvider.GetCustomAttribute<DisplayNameAttribute>(false)?.DisplayName;

            return name;
        }

        /// <summary>
        /// Tries to get attributes defining a description on this attribute provider.
        /// If no attribute was found, null will be the result.
        /// The chain follows: <see cref="DisplayAttribute"/> and <see cref="DescriptionAttribute"/>
        /// </summary>
        /// <returns>The value of the description or <c>null</c>.</returns>
        public string GetDescription()
        {
            var description = attributeProvider.GetCustomAttribute<DisplayAttribute>(false)?.GetDescription();

            if (string.IsNullOrEmpty(description))
                description = attributeProvider.GetCustomAttribute<DescriptionAttribute>(false)?.Description;

            return description;
        }
    }
}