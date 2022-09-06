// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moryx.AbstractionLayer.Products;
using Moryx.Configuration;
using Moryx.Serialization;

namespace Moryx.AbstractionLayer.Products.Endpoints
{
    /// <summary>
    /// Specialized serialization that only considers properties of derived classes
    /// </summary>
    public class PartialSerialization<T> : PossibleValuesSerialization
        where T : class
    {
        private static readonly EntrySerializeSerialization _serialization;

        /// <summary>
        /// Properties that shall be excluded from the generic collection
        /// </summary>
        private static readonly string[] FilteredProperties = typeof (T).GetProperties().Select(p => p.Name).ToArray();

        static PartialSerialization()
        {
            _serialization = new EntrySerializeSerialization();
        }

        public PartialSerialization() : base(null, new EmptyValueProvider())
        {
        }

        /// <see cref="T:Moryx.Serialization.ICustomSerialization"/>
        public override IEnumerable<PropertyInfo> GetProperties(Type sourceType)
        {
            // Only simple properties not defined in the base
            return _serialization.GetProperties(sourceType).Where(SimpleProp);
        }

        protected bool SimpleProp(PropertyInfo prop)
        {
            // Skip reference or domain model properties
            var type = prop.PropertyType;
            if (type == typeof(ProductFile) ||
                typeof(IProductType).IsAssignableFrom(type) ||
                typeof (IProductPartLink).IsAssignableFrom(type) ||
                typeof(IEnumerable<IProductPartLink>).IsAssignableFrom(type))
                return false;

            // Filter default properties
            if (FilteredProperties.Contains(prop.Name))
                return false;

            return true;
        }

        /// <see cref="T:Moryx.Serialization.ICustomSerialization"/>
        public override IEnumerable<MappedProperty> WriteFilter(Type sourceType, IEnumerable<Entry> encoded)
        {
            // Only update properties with values from client
            return base.WriteFilter(sourceType, encoded).Where(mapped => mapped.Entry != null);
        }

        private class EmptyValueProvider : IEmptyPropertyProvider
        {
            public void FillEmpty(object obj)
            {
                ValueProviderExecutor.Execute(obj, new ValueProviderExecutorSettings().AddDefaultValueProvider());
            }
        }
    }
}
