// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Moryx.Serialization
{
    /// <summary>
    /// Wrapper to identifiy properties that display the possible values
    /// </summary>
    internal class PossibleValuesWrapperFactory : ITypeWrapperFactory
    {
        /// <summary>
        /// Regex used to check for the naming conventing
        /// </summary>
        private static readonly Regex PossibleValuesRegex = new Regex(@"Possible(?<key>\w+)s");

        /// <summary>
        /// Indicates that this reader can read this property
        /// </summary>
        public bool CanWrap(PropertyInfo property)
        {
            var att = property.GetCustomAttribute<ConfigKeyAttribute>();
            return property.PropertyType == typeof(string[])
                && (PossibleValuesRegex.IsMatch(property.Name) || (att != null && att.PossibleValues));
        }

        /// <summary>
        /// Create wrapper around the property
        /// </summary>
        /// <param name="property">Property that shall be wrapped</param>
        /// <param name="formatProvider"><see cref="IFormatProvider"/> used for parsing and writing</param>
        /// <returns>Wrapped property</returns>
        public PropertyTypeWrapper Wrap(PropertyInfo property, IFormatProvider formatProvider)
        {
            var key = AttributeWrapperFactory.FromAttributeOrNull(property);
            return new PossibleValuesWrapper(property, key ?? PossibleValuesRegex.Match(property.Name).Groups["key"].Value, formatProvider)
            {
                Required = key != null
            };
        }
    }

    internal class PossibleValuesWrapper : PropertyTypeWrapper
    {
        /// <summary>
        /// Create match wrapper for a property
        /// </summary>
        public PossibleValuesWrapper(PropertyInfo property, string key, IFormatProvider formatProvider) : base(property, formatProvider)
        {
            Key = key;
        }

        /// <summary>
        /// Read value from config
        /// </summary>
        protected override object ReadFromConfig(Entry entry)
        {
            return entry.Value.Possible;
        }

        /// <summary>
        /// Read value from property
        /// </summary>
        public override void ReadValue(object source, Entry target)
        {
            // Possible values can not be written to the config
        }
    }
}
