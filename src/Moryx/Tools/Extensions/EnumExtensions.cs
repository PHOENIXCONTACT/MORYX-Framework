// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace Moryx.Tools;

/// <summary>
/// Extensions for the <see cref="Enum"/>
/// </summary>
public static class EnumExtensions
{
    /// <param name="enumValue">Enum value decorated with the attribute</param>
    extension(Enum enumValue)
    {
        /// <summary>
        /// Returns the <see cref="FieldInfo"/> corresponding to the <paramref name="enumValue"/>, if it is not a Flag enum. 
        /// Otherwise returns the <see cref="FieldInfo"/>s for each active Flag seperately.
        /// </summary>
        private FieldInfo[] GetFieldInfos()
        {
            var enumType = enumValue.GetType();

            if (enumType.GetCustomAttribute<FlagsAttribute>() is null)
            {
                return [enumType.GetField(enumValue.ToString())];
            }

            var enumNumericValue = Convert.ToUInt64(enumValue, CultureInfo.InvariantCulture);
            return Enum.GetValues(enumType).Cast<Enum>()
                .Where(e => enumValue.HasFlag(e) && (Convert.ToUInt64(e, CultureInfo.InvariantCulture) != 0 || enumNumericValue == 0))
                .Select(e => enumType.GetField(e.ToString())).ToArray();
        }

        /// <summary>
        /// Returns the (first) custom attribute of type <typeparamref name="TAttribute"/> on this 
        /// <paramref name="enumValue"/> or null if attribute was not found.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute. </param>
        public TAttribute GetCustomAttribute<TAttribute>(bool inherit = true)
            where TAttribute : Attribute
        {
            var field = GetFieldInfos(enumValue).FirstOrDefault();
            return field?.GetCustomAttributes(typeof(TAttribute), inherit).FirstOrDefault() as TAttribute;
        }

        /// <summary>
        /// Returns an array of attributes defined on this member or an empty array, if no attribute were found
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute. </param>
        public TAttribute[] GetCustomAttributes<TAttribute>(bool inherit = true)
            where TAttribute : Attribute
        {
            var fields = GetFieldInfos(enumValue);
            return fields.SelectMany(f => f.GetCustomAttributes(typeof(TAttribute), inherit)) as TAttribute[];
        }

        /// <summary>
        /// Tries to get an attribute defining a display name on this <paramref name="enumValue"/>. If no attribute 
        /// was found returns the type name. If multiple attributes were found, returns the first.
        /// </summary>
        public string GetDisplayName()
        {
            var name = enumValue.GetCustomAttribute<DisplayAttribute>(false)?.GetName();

            if (string.IsNullOrEmpty(name))
            {
                name = enumValue.GetFieldInfos().FirstOrDefault().Name;
            }

            return name;
        }

        /// <summary>
        /// Tries to get an attribute defining a description on this <paramref name="enumValue"/>. If no attribute was 
        /// found, null will be the result.
        /// The chain follows: <see cref="DisplayAttribute"/> and <see cref="DescriptionAttribute"/>
        /// </summary>
        public string GetDescription()
        {
            var description = enumValue.GetCustomAttribute<DisplayAttribute>(false)?.GetDescription();

            if (string.IsNullOrEmpty(description))
            {
                description = enumValue.GetCustomAttribute<DescriptionAttribute>(false)?.Description;
            }

            return description;
        }
    }
}