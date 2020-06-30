// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Moryx.AbstractionLayer.Resources;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Runtime.Configuration;
using Moryx.Serialization;
using Moryx.Tools;
using IContainer = Moryx.Container.IContainer;

namespace Moryx.Resources.Management
{
    /// <summary>
    /// Implementation of <see cref="ICustomSerialization"/> for types derived from <see cref="Resource"/>
    /// TODO: This is partially copied from <see cref="PossibleValuesSerialization"/>, that shall be refactored/removed in Runtime 3.0
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
        /// Only export properties flagged with <see cref="System.Runtime.Serialization.DataMemberAttribute"/>
        /// </summary>
        public override IEnumerable<PropertyInfo> GetProperties(Type sourceType)
        {
            // Always filter resource references
            var properties = base.GetProperties(sourceType).ToList();
            var flagged = properties.Where(p => Attribute.IsDefined(p, typeof(EditorBrowsableAttribute))).ToList();

            // On resources only return flagged types
            if (typeof(Resource).IsAssignableFrom(sourceType))
                return flagged;

            // Otherwise decide based on Attribute usage
            return Attribute.IsDefined(sourceType, typeof(EditorBrowsableAttribute)) || flagged.Count == 0
                ? properties // Return all properties if the entire type is flagged as EditorBrowsable or the attribute was not used at all
                : flagged; // Otherwise filter by EditorBrowsableAttribute per property
        }

        /// <see cref="T:Moryx.Serialization.ICustomSerialization"/>
        public override EntryPrototype[] Prototypes(Type memberType, ICustomAttributeProvider attributeProvider)
        {
            // Create prototypes from possible values
            var possibleValuesAtt = attributeProvider.GetCustomAttribute<PossibleValuesAttribute>();
            if (possibleValuesAtt == null)
            {
                return base.Prototypes(memberType, attributeProvider);
            }

            var possibleValues = possibleValuesAtt.GetValues(Container);
            var list = new List<EntryPrototype>();
            foreach (var value in possibleValues)
            {
                var prototype = possibleValuesAtt.Parse(Container, value);
                ConfigManager.FillEmpty(prototype);
                list.Add(new EntryPrototype(value, prototype));
            }
            return list.ToArray();
        }

        /// <see cref="T:Moryx.Serialization.ICustomSerialization"/>
        public override string[] PossibleValues(Type memberType, ICustomAttributeProvider attributeProvider)
        {
            var valuesAttribute = attributeProvider.GetCustomAttribute<PossibleValuesAttribute>();
            if (valuesAttribute == null)
            {
                return base.PossibleValues(memberType, attributeProvider);
            }

            // Use attribute
            var values = valuesAttribute.GetValues(Container);
            return values?.Distinct().ToArray();
        }

        /// <see cref="T:Moryx.Serialization.ICustomSerialization"/>
        public override object CreateInstance(Type memberType, ICustomAttributeProvider attributeProvider, Entry encoded)
        {
            var possibleValuesAtt = attributeProvider.GetCustomAttribute<PossibleValuesAttribute>();
            var instance = possibleValuesAtt != null
                ? possibleValuesAtt.Parse(Container, encoded.Value.Current)
                : base.CreateInstance(memberType, encoded);
            ConfigManager.FillEmpty(instance);
            return instance;
        }

        /// <see cref="T:Moryx.Serialization.ICustomSerialization"/>
        public override object ConvertValue(Type memberType, ICustomAttributeProvider attributeProvider, Entry mappedEntry, object currentValue)
        {
            var value = mappedEntry.Value;
            var att = attributeProvider.GetCustomAttribute<PossibleValuesAttribute>();
            if (att == null || !att.OverridesConversion || value.Type == EntryValueType.Collection)
                return base.ConvertValue(memberType, attributeProvider, mappedEntry, currentValue);

            // If old and current type are identical, keep the object
            if (value.Type == EntryValueType.Class && currentValue != null && currentValue.GetType().Name == value.Current)
                return currentValue;

            var instance = att.Parse(Container, mappedEntry.Value.Current);
            if (mappedEntry.Value.Type == EntryValueType.Class)
                ConfigManager.FillEmpty(instance);
            return instance;
        }

        public override IEnumerable<MethodInfo> GetMethods(Type sourceType)
        {
            var methods = base.GetMethods(sourceType);

            methods = Attribute.IsDefined(sourceType, typeof(EditorBrowsableAttribute))
                ? methods.Where(method => method.DeclaringType != typeof(object)) // Filter methods defined by object
                : methods.Where(method => Attribute.IsDefined(method, typeof(EditorBrowsableAttribute))); // Filter methods carrying the editor visible attribute

            return methods;
        }
    }
}
