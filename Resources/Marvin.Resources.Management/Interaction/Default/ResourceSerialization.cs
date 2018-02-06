using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Marvin.AbstractionLayer.Resources;
using Marvin.Container;
using Marvin.Runtime.Base.Serialization;
using Marvin.Runtime.Configuration;
using Marvin.Serialization;

namespace Marvin.Resources.Management
{
    /// <summary>
    /// Implementation of <see cref="ICustomSerialization"/> for types derived from <see cref="Resource"/>
    /// This is partially copied from <see cref="ConfigSerialization"/>, that shall be refactored/removed in Runtime 3.0
    /// </summary>
    [Component(LifeCycle.Singleton, typeof(ICustomSerialization))]
    internal class ResourceSerialization : DefaultSerialization
    {
        /// <summary>
        /// Injected local container. This is an absolute exception to the rule because this component
        /// operates on the META level of component composition.
        /// </summary>
        public IContainer Container { get; set; }

        /// <summary>
        /// ConfigManager to fill properties with defaults
        /// </summary>
        public IRuntimeConfigManager ConfigManager { get; set; }

        /// <summary>
        /// Only export properties flagged with <see cref="DataMember"/>
        /// </summary>
        public override IEnumerable<PropertyInfo> ReadFilter(Type sourceType)
        {
            // Always filter resource references
            var properties = base.ReadFilter(sourceType).ToList();
            var flagged = properties.Where(p => Attribute.IsDefined(p, typeof(EditorVisibleAttribute))).ToList();

            // On resources only return flagged types
            if (typeof(Resource).IsAssignableFrom(sourceType))
                return flagged;
            
            // Otherwise decide based on Attribute usage
            return Attribute.IsDefined(sourceType, typeof(EditorVisibleAttribute)) || flagged.Count == 0
                ? properties // Return all properties if the entire type is flagged as EditorVisible or the attribute was not used at all
                : flagged; // Otherwise filter by EditorVisibleAttribute per property
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
            var list = new List<EntryPrototype>();
            foreach (var value in possibleValues)
            {
                var prototype = possibleValuesAtt.ConvertToConfigValue(Container, value);
                ConfigManager.FillEmptyProperties(prototype);
                list.Add(new EntryPrototype(value, prototype));
            }
            return list.ToArray();
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

        /// <inheritdoc />
        public override string[] PossibleValues(ParameterInfo parameter)
        {
            var valuesAttribute = parameter.GetCustomAttribute<PossibleConfigValuesAttribute>();
            if (valuesAttribute == null)
            {
                return base.PossibleValues(parameter);
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
            ConfigManager.FillEmptyProperties(instance);
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
                ConfigManager.FillEmptyProperties(instance);
            return instance;
        }

        public override object ParameterValue(ParameterInfo parameter, Entry mappedEntry)
        {
            var value = mappedEntry.Value;
            var att = parameter.GetCustomAttribute<PossibleConfigValuesAttribute>();
            if (att == null || !att.OverridesConversion || value.Type == EntryValueType.Collection)
                return base.ParameterValue(parameter, mappedEntry);

            var instance = att.ConvertToConfigValue(Container, mappedEntry.Value.Current);
            if (mappedEntry.Value.Type == EntryValueType.Class)
                ConfigManager.FillEmptyProperties(instance);

            return instance;
        }

        public override IEnumerable<MethodInfo> MethodFilter(Type sourceType)
        {
            var methods = base.MethodFilter(sourceType);

            if (Attribute.IsDefined(sourceType, typeof(EditorVisibleAttribute)))
                // Filter methods defined by object
                methods = methods.Where(method => method.DeclaringType != typeof(object));
            else
                // Filter methods carrying the editor visible attribute
                methods = methods.Where(method => Attribute.IsDefined(method, typeof(EditorVisibleAttribute)));

            return methods;
        }
    }
}