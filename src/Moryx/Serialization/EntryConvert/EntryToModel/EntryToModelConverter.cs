// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Moryx.Serialization
{
    /// <summary>
    /// Helper class to convert config entry data to view models and vice versa
    /// </summary>
    public class EntryToModelConverter
    {
        /// <summary>
        /// Fast access property map
        /// </summary>
        private readonly PropertyTypeWrapper[] _properties;

        /// <summary>
        /// Private constructor to enforce create method
        /// </summary>
        private EntryToModelConverter(PropertyTypeWrapper[] properties)
        {
            _properties = properties;
        }

        /// <summary>
        /// Create a new converter instance using the generic type
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns></returns>
        public static EntryToModelConverter Create<T>()
        {
            return Create(typeof(T), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Create a new converter instance using the generic type
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns></returns>
        public static EntryToModelConverter Create<T>(IFormatProvider formatProvider)
        {
            return Create(typeof(T), formatProvider);
        }

        /// <summary>
        /// Create a new converter instance for a given object type
        /// </summary>
        public static EntryToModelConverter Create(object target)
        {
            return Create(target.GetType(), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Create converter from dynamic type object
        /// </summary>
        public static EntryToModelConverter Create(Type type, IFormatProvider formatProvider)
        {
            var properties = (from prop in type.GetProperties()
                              let factory = TypeWrapperFactories.FirstOrDefault(f => f.CanWrap(prop))
                              select factory.Wrap(prop, formatProvider)).ToArray();
            return new EntryToModelConverter(properties);
        }

        /// <summary>
        /// Read values from config to view model
        /// </summary>
        public void FromModel(Entry source, object target)
        {
            foreach (var match in EnumerateConfig(source))
            {
                match.SetValue(target);
            }
        }

        /// <summary>
        /// Copy values from model to config
        /// </summary>
        public void ToModel(object source, Entry target)
        {
            foreach (var match in EnumerateConfig(target))
            {
                match.ReadValue(source);
            }
        }

        /// <summary>
        /// Enumerate a concrete object using the type properties
        /// </summary>
        /// <param name="model">Config model</param>
        /// <returns></returns>
        private IEnumerable<PropertyInstanceWrapper> EnumerateConfig(Entry model)
        {
            // Fast access cache of the config
            var cache = model.SubEntries.ToDictionary(e => e.Identifier, e => e);
            foreach (var property in _properties)
            {
                if (cache.ContainsKey(property.Key))
                    yield return property.Instantiate(cache[property.Key]);
                else if (property.Required)
                    throw new KeyNotFoundException($"Required field '{property.Key}' not found in config");
            }
        }

        /// <summary>
        /// Responsibility chain starting with most specific shifting towards the least specific
        /// </summary>
        private static readonly ITypeWrapperFactory[] TypeWrapperFactories =
        {
            new CollectionWrapperFactory(),
            new DictionaryWrapperFactory(),
            new ClassWrapperFactory(),
            new PossibleValuesWrapperFactory(),
            new AttributeWrapperFactory(),
            new NameWrapperFactory(),
        };
    }
}
