// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Moryx.Tools;

namespace Moryx.Serialization
{
    /// <summary>
    /// Filters methods and properties with <see cref="EntrySerializeAttribute"/> set.
    /// </summary>
    public class EntrySerializeSerialization : DefaultSerialization
    {
        private static readonly Lazy<EntrySerializeSerialization> LazyInstance
            = new Lazy<EntrySerializeSerialization>(LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static EntrySerializeSerialization Instance => LazyInstance.Value;

        /// <inheritdoc />
        public override IEnumerable<ConstructorInfo> GetConstructors(Type sourceType)
        {
            var constructors = from ctor in base.GetConstructors(sourceType)
                let mode = EvaluateSerializeMode(ctor)
                where mode.HasValue && mode.Value == EntrySerializeMode.Always
                select ctor;

            return constructors;
        }

        /// <inheritdoc />
        public override IEnumerable<MethodInfo> GetMethods(Type sourceType)
        {
            var methods = sourceType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(m => !m.IsSpecialName);
            methods = from method in methods
                let mode = EvaluateSerializeMode(method)
                where mode.HasValue && mode.Value == EntrySerializeMode.Always
                select method;

            return methods;
        }

        /// <inheritdoc />
        public override IEnumerable<PropertyInfo> GetProperties(Type sourceType)
        {
            var properties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToArray();

            var sourceTypeMode = EvaluateSerializeMode(sourceType);
            var propertyModes = properties.Select(EvaluateSerializeMode).ToArray();

            var alwaysProperties = propertyModes
                .Where(pm => pm.Mode.HasValue && pm.Mode == EntrySerializeMode.Always)
                .Select(pm => pm.Property).ToArray();

            var neverProperties = propertyModes
                .Where(pm => pm.Mode.HasValue && pm.Mode == EntrySerializeMode.Never)
                .Select(pm => pm.Property).ToArray();

            // Class: Always
            // -> All properties except "Never"
            if (sourceTypeMode.HasValue && sourceTypeMode.Value == EntrySerializeMode.Always)
                return properties.Except(neverProperties);

            // Class: Never
            // -> Only "Always" properties
            if (sourceTypeMode.HasValue && sourceTypeMode.Value == EntrySerializeMode.Never)
                return alwaysProperties;

            // Class: Not defined

            // "Always" and "Never" properties, "Always" properties but no "Never"
            // Only "Never" properties
            if (alwaysProperties.Length > 0)
                return alwaysProperties;

            // No "Always" properties, but "Never" properties
            // -> Properties except "Never"
            if (neverProperties.Length > 0)
                return properties.Except(neverProperties);

            // No property mode defined, No "Always", No "Never"
            // -> All property
            return properties;
        }

        /// <summary>
        /// Checks if the <see cref="EntrySerializeAttribute"/> is existent and activated
        /// </summary>
        private static EntrySerializeMode? EvaluateSerializeMode(ICustomAttributeProvider attributeProvider)
        {
            var entrySerializeAttr = attributeProvider.GetCustomAttribute<EntrySerializeAttribute>();
            return entrySerializeAttr?.Mode;
        }

        private static PropertyMode EvaluateSerializeMode(PropertyInfo property)
        {
            return new PropertyMode
            {
                Property = property,
                Mode = EvaluateSerializeMode((ICustomAttributeProvider)property)
            };
        }

        private class PropertyMode
        {
            public PropertyInfo Property { get; set; }

            public EntrySerializeMode? Mode { get; set; }
        }
    }
}
