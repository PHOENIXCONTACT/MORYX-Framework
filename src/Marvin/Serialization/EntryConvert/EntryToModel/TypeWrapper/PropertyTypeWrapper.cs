using System;
using System.Reflection;

namespace Marvin.Serialization
{
    /// <summary>
    /// Collection of objects around properties including type, config value and attribute
    /// </summary>
    internal class PropertyTypeWrapper
    {
        /// <summary>
        /// Create match wrapper for a property
        /// </summary>
        public PropertyTypeWrapper(PropertyInfo property, IFormatProvider formatProvider)
        {
            Property = property;
            Required = true;
            FormatProvider = formatProvider;
        }

        public IFormatProvider FormatProvider { get; }

        /// <summary>
        /// Wrapped property
        /// </summary>
        protected PropertyInfo Property { get; set; }

        /// <summary>
        /// Key in the config used to find the value instance
        /// </summary>
        public string Key { get; protected internal set; }

        /// <summary>
        /// Indicator if this property must be present in the config tree
        /// </summary>
        public bool Required { get; protected internal set; }

        /// <summary>
        /// Create a typewrapper for a certain object and config
        /// </summary>
        /// <param name="value">Value that represents this property in the config</param>
        public PropertyInstanceWrapper Instantiate(Entry value)
        {
            return new PropertyInstanceWrapper(this, value);
        }

        /// <summary>
        /// Set property value on target object
        /// </summary>
        public virtual void SetValue(object target, Entry entry)
        {
            var value = ReadFromConfig(entry);
            Property.SetValue(target, value);
        }

        /// <summary>
        /// Read value from property and write to config
        /// </summary>
        public virtual void ReadValue(object source, Entry target)
        {
            var value = Property.GetValue(source);
            target.Value.Current = value?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Read value from config
        /// </summary>
        protected virtual object ReadFromConfig(Entry entry)
        {
            // Synchronus resolution
            return EntryConvert.ToObject(Property.PropertyType, entry.Value.Current, FormatProvider);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{Property.Name} => {Key ?? "{null}"}";
        }
    }
}