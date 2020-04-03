// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Marvin.AbstractionLayer;
using Marvin.AbstractionLayer.Products;
using Marvin.Configuration;
using Marvin.Container;
using Marvin.Serialization;
using Marvin.Tools;

namespace Marvin.Products.Management.Modification
{
    internal class PartialSerialization<T> : DefaultSerialization
        where T : class
    {
        /// <summary>
        /// Properties that shall be excluded from the generic collection
        /// </summary>
        private static readonly string[] FilteredProperties = typeof (T).GetProperties().Select(p => p.Name).ToArray();

        /// <see cref="T:Marvin.Serialization.ICustomSerialization"/>
        public override IEnumerable<PropertyInfo> GetProperties(Type sourceType)
        {
            // Only simple properties not defined in the base
            return base.GetProperties(sourceType).Where(SimpleProp);
        }

        protected bool SimpleProp(PropertyInfo prop)
        {
            // Check if property can be written
            if (prop.SetMethod == null)
                return false;

            // Check type
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

        /// <see cref="T:Marvin.Serialization.ICustomSerialization"/>
        public override IEnumerable<MappedProperty> WriteFilter(Type sourceType, IEnumerable<Entry> encoded)
        {
            return base.WriteFilter(sourceType, encoded).Where(mapped => mapped.Entry != null);
        }

        public override string[] PossibleValues(Type memberType, ICustomAttributeProvider attributeProvider)
        {
            var possibleAtt = attributeProvider.GetCustomAttribute<PossibleValuesAttribute>();
            if (possibleAtt != null)
                return possibleAtt.GetValues(null).ToArray(); // TODO: Replace with proper possible values handling

            return base.PossibleValues(memberType, attributeProvider);
        }

    }
}
