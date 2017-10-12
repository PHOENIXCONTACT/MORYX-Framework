using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Marvin.Container;
using Marvin.Runtime.Configuration;
using Marvin.Runtime.Container;
using Marvin.Serialization;

namespace Marvin.Runtime.Base.Serialization
{
    /// <summary>
    /// Base class for config to model transformer
    /// </summary>
    public class ConfigSerialization : DefaultSerialization
    {
        private readonly IContainer _container;
        private readonly IRuntimeConfigManager _configManager;

        /// <summary>
        /// Initialize base class
        /// </summary>
        public ConfigSerialization(IContainer container, IRuntimeConfigManager configManager)
        {
            _container = container;
            _configManager = configManager;
        }

        /// <see cref="T:Marvin.Serialization.ICustomSerialization"/>
        public override EntryPrototype[] Prototypes(PropertyInfo property)
        {
            // Create prototypes from possible values
            var possibleValuesAtt = property.GetCustomAttribute<PossibleConfigValuesAttribute>();
            if (possibleValuesAtt == null)
            {
                return base.Prototypes(property);
            }

            var possibleValues = possibleValuesAtt.ResolvePossibleValues(_container);
            return (from value in possibleValues
                    let prototype = possibleValuesAtt.ConvertToConfigValue(_container, value)
                    select new EntryPrototype(value, prototype)).ToArray();
        }

        /// <see cref="T:Marvin.Serialization.ICustomSerialization"/>
        public override string[] PossibleValues(PropertyInfo property)
        {
            var valuesAttribute = property.GetCustomAttribute<PossibleConfigValuesAttribute>();
            if (valuesAttribute == null)
            {
                return base.PossibleValues(property);
            }

            // Use attribute
            var values = valuesAttribute.ResolvePossibleValues(_container);
            return values?.Distinct().ToArray();
        }


        /// <see cref="T:Marvin.Serialization.ICustomSerialization"/>
        public override string[] PossibleElementValues(PropertyInfo property)
        {
            var valuesAttribute = property.GetCustomAttribute<PossibleConfigValuesAttribute>();
            if (valuesAttribute == null)
            {
                return base.PossibleElementValues(property);
            }

            // Use attribute
            var values = valuesAttribute.ResolvePossibleValues(_container);
            return values?.Distinct().ToArray();
        }

        /// <see cref="T:Marvin.Serialization.ICustomSerialization"/>
        public override object CreateInstance(MappedProperty mappedRoot, Entry encoded)
        {
            var possibleValuesAtt = mappedRoot.Property.GetCustomAttribute<PossibleConfigValuesAttribute>();
            var instance = possibleValuesAtt != null
                ? possibleValuesAtt.ConvertToConfigValue(_container, encoded.Value.Current)
                : base.CreateInstance(mappedRoot, encoded);
            _configManager.FillEmptyProperties(instance);
            return instance;
        }

        /// <see cref="T:Marvin.Serialization.ICustomSerialization"/>
        public override object PropertyValue(MappedProperty mapped, object currentValue)
        {
            var value = mapped.Entry.Value;

            var att = mapped.Property.GetCustomAttribute<PossibleConfigValuesAttribute>();
            if (att == null || !att.OverridesConversion || value.Type == EntryValueType.Collection)
                return base.PropertyValue(mapped, currentValue);

            // If old and current type are identical, keep the object
            if (value.Type == EntryValueType.Class && currentValue != null && currentValue.GetType().Name == value.Current)
                return currentValue;

            var instance = att.ConvertToConfigValue(_container, mapped.Entry.Value.Current);
            if (mapped.Entry.Value.Type == EntryValueType.Class)
                _configManager.FillEmptyProperties(instance);
            return instance;
        }
    }
}
