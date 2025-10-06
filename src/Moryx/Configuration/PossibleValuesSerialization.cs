// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.Container;
using Moryx.Serialization;
using Moryx.Tools;

namespace Moryx.Configuration
{
    /// <summary>
    /// Base class for config to model transformer
    /// </summary>
    public class PossibleValuesSerialization : DefaultSerialization
    {
        /// <summary>
        /// Container used to include current information from current composition into the configuration
        /// </summary>
        protected IContainer Container { get; }

        /// <summary>
        /// Access to level 1 service registration
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Empty property provider to pre-fill newley created objects
        /// </summary>
        protected IEmptyPropertyProvider EmptyPropertyProvider { get; }

        /// <summary>
        /// Initialize base class
        /// </summary>
        [Obsolete("Construct possible values with ServiceProvider for attributes that rely on it")]
        public PossibleValuesSerialization(IContainer container, IEmptyPropertyProvider emptyPropertyProvider)
            : this(container, null, emptyPropertyProvider) { }

        /// <summary>
        /// Initialize base class
        /// </summary>
        public PossibleValuesSerialization(IContainer container, IServiceProvider serviceProvider, IEmptyPropertyProvider emptyPropertyProvider)
        {
            Container = container;
            ServiceProvider = serviceProvider;
            EmptyPropertyProvider = emptyPropertyProvider;
        }

        /// <see cref="T:Moryx.Serialization.ICustomSerialization"/>
        public override EntryPrototype[] Prototypes(Type memberType, ICustomAttributeProvider attributeProvider)
        {
            var possibleValuesAtt = attributeProvider.GetCustomAttribute<PossibleValuesAttribute>();
            // We can not create prototypes for possible primitives
            if (possibleValuesAtt == null || IsPrimitiveCollection(memberType))
                return base.Prototypes(memberType, attributeProvider);

            // Create prototypes from possible values
            var list = new List<EntryPrototype>();
            foreach (var value in possibleValuesAtt.GetValues(Container, ServiceProvider))
            {
                var prototype = possibleValuesAtt.Parse(Container, ServiceProvider, value);
                EmptyPropertyProvider.FillEmpty(prototype);
                list.Add(new EntryPrototype(value, prototype));
            }
            return list.ToArray();
        }

        /// <see cref="T:Moryx.Serialization.ICustomSerialization"/>
        public override string[] PossibleValues(Type memberType, ICustomAttributeProvider attributeProvider)
        {
            var valuesAttribute = attributeProvider.GetCustomAttribute<PossibleValuesAttribute>();
            // Possible values for primitive collections only apply to members
            if (valuesAttribute == null || IsPrimitiveCollection(memberType))
                return base.PossibleValues(memberType, attributeProvider);

            // Use attribute
            var values = valuesAttribute.GetValues(Container, ServiceProvider);
            return values?.Distinct().ToArray();
        }

        /// <summary>
        /// Check if a property is a collection of primitives
        /// </summary>
        private static bool IsPrimitiveCollection(Type memberType)
        {
            if (!EntryConvert.IsCollection(memberType))
                return false;

            var elementType = EntryConvert.ElementType(memberType);
            return EntryConvert.ValueOrStringType(elementType);
        }


        /// <see cref="T:Moryx.Serialization.ICustomSerialization"/>
        public override string[] PossibleElementValues(Type memberType, ICustomAttributeProvider attributeProvider)
        {
            var valuesAttribute = attributeProvider.GetCustomAttribute<PossibleValuesAttribute>();
            if (valuesAttribute == null)
            {
                return base.PossibleElementValues(memberType, attributeProvider);
            }

            // Use attribute
            var values = valuesAttribute.GetValues(Container, ServiceProvider);
            return values?.Distinct().ToArray();
        }

        /// <see cref="T:Moryx.Serialization.ICustomSerialization"/>
        public override object CreateInstance(Type memberType, ICustomAttributeProvider attributeProvider, Entry encoded)
        {
            var possibleValuesAtt = attributeProvider.GetCustomAttribute<PossibleValuesAttribute>();
            var instance = possibleValuesAtt != null
                ? possibleValuesAtt.Parse(Container, ServiceProvider, encoded.Value.Current)
                : base.CreateInstance(memberType, attributeProvider, encoded);

            EmptyPropertyProvider.FillEmpty(instance);

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

            var instance = att.Parse(Container, ServiceProvider, mappedEntry.Value.Current);
            if (mappedEntry.Value.Type == EntryValueType.Class)
            {
                EmptyPropertyProvider.FillEmpty(instance);
            }

            return instance;
        }
    }
}
