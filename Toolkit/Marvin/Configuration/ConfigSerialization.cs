using System.Linq;
using System.Reflection;
using Marvin.Container;
using Marvin.Serialization;

namespace Marvin.Configuration
{
    /// <summary>
    /// Base class for config to model transformer
    /// </summary>
    public class ConfigSerialization : DefaultSerialization
    {
        /// <summary>
        /// Container used to include current information from current composition into the configuration
        /// </summary>
        protected IContainer Container { get; }

        /// <summary>
        /// Empty property provider to pre-fill newley created objects
        /// </summary>
        protected IEmptyPropertyProvider EmptyPropertyProvider { get; }

        /// <summary>
        /// Initialize base class
        /// </summary>
        public ConfigSerialization(IContainer container, IEmptyPropertyProvider emptyPropertyProvider)
        {
            Container = container;
            EmptyPropertyProvider = emptyPropertyProvider;
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

            var possibleValues = possibleValuesAtt.ResolvePossibleValues(Container);
            return (from value in possibleValues
                    let prototype = possibleValuesAtt.ConvertToConfigValue(Container, value)
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
            var values = valuesAttribute.ResolvePossibleValues(Container);
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
            var values = valuesAttribute.ResolvePossibleValues(Container);
            return values?.Distinct().ToArray();
        }

        /// <see cref="T:Marvin.Serialization.ICustomSerialization"/>
        public override object CreateInstance(MappedProperty mappedRoot, Entry encoded)
        {
            var possibleValuesAtt = mappedRoot.Property.GetCustomAttribute<PossibleConfigValuesAttribute>();
            var instance = possibleValuesAtt != null
                ? possibleValuesAtt.ConvertToConfigValue(Container, encoded.Value.Current)
                : base.CreateInstance(mappedRoot, encoded);

            EmptyPropertyProvider.FillEmpty(instance);

            return instance;
        }

        /// <see cref="T:Marvin.Serialization.ICustomSerialization"/>
        public override object PropertyValue(PropertyInfo property, Entry mappedEntry, object currentValue)
        {
            var value = mappedEntry.Value;

            var att = property.GetCustomAttribute<PossibleConfigValuesAttribute>();
            if (att == null || !att.OverridesConversion || value.Type == EntryValueType.Collection)
                return base.PropertyValue(property, mappedEntry, currentValue);

            // If old and current type are identical, keep the object
            if (value.Type == EntryValueType.Class && currentValue != null && currentValue.GetType().Name == value.Current)
                return currentValue;

            var instance = att.ConvertToConfigValue(Container, mappedEntry.Value.Current);
            if (mappedEntry.Value.Type == EntryValueType.Class)
            {
                EmptyPropertyProvider.FillEmpty(instance);
            }

            return instance;
        }
    }
}