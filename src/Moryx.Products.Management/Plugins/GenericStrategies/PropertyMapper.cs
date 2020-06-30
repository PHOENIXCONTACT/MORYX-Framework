// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Reflection;
using Moryx.Products.Model;
using Moryx.Tools;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Base class for specialized column mappers
    /// </summary>
    public abstract class ColumnMapper<TColumn> : IPropertyMapper
    {
        protected Type TargetType { get; }

        protected PropertyMapperConfig Config { get; private set; }

        /// <summary>
        /// Accessor for the property on the business object
        /// </summary>
        protected IPropertyAccessor<object, TColumn> ObjectAccessor { get; private set; }

        /// <summary>
        /// Accessor for the property on the entity
        /// </summary>
        protected IPropertyAccessor<IGenericColumns, TColumn> ColumnAccessor { get; private set; }

        protected ColumnMapper(Type targetType)
        {
            TargetType = targetType;
        }

        public void Initialize(PropertyMapperConfig config)
        {
            Config = config;

            // Create accessor for the column property
            var columnProp = typeof(IGenericColumns).GetProperty(config.Column);
            if (columnProp == null || columnProp.PropertyType != typeof(TColumn))
                throw new ArgumentException($"Column not found or type mismatch {config.PropertyName}");

            ColumnAccessor = ReflectionTool.PropertyAccessor<IGenericColumns, TColumn>(columnProp);

            // Retrieve and validate properties
            var objectProp = TargetType.GetProperty(config.PropertyName);
            if (objectProp == null)
                throw new ArgumentException($"Target type {TargetType.Name} does not have a property {config.PropertyName}");

            // Create delegates for the object property as well
            ObjectAccessor = CreatePropertyAccessor(objectProp);
        }

        protected virtual IPropertyAccessor<object, TColumn> CreatePropertyAccessor(PropertyInfo objectProp)
        {
            // Default conversions
            return ReflectionTool.PropertyAccessor<object, TColumn>(objectProp);
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public string PropertyName => Config.PropertyName;

        public bool HasChanged(IGenericColumns current, object updated)
        {
            var objectValue = ObjectAccessor.ReadProperty(updated);
            var columnValue = ColumnAccessor.ReadProperty(current);

            return !columnValue.Equals(objectValue);
        }

        public void ReadValue(IGenericColumns source, object target)
        {
            var value = ColumnAccessor.ReadProperty(source);
            ObjectAccessor.WriteProperty(target, value);
        }

        public void WriteValue(object source, IGenericColumns target)
        {
            var value = ObjectAccessor.ReadProperty(source);
            ColumnAccessor.WriteProperty(target, value);
        }
    }
}