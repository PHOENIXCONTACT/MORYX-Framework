// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq.Expressions;
using System.Reflection;
using Moryx.Products.Management.Model;
using Moryx.Tools;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Base class for specialized column mappers
    /// </summary>
    public abstract class ColumnMapper<TColumn> : IPropertyMapper
    {
        /// <summary>
        /// Target type of the mapper
        /// </summary>
        protected Type TargetType { get; }

        /// <summary>
        /// Configuration of the mapper
        /// </summary>
        protected PropertyMapperConfig Config { get; private set; }

        /// <summary>
        /// Accessor for the property on the business object
        /// </summary>
        protected IPropertyAccessor<object, TColumn> ObjectAccessor { get; private set; }

        /// <summary>
        /// Accessor for the property on the entity
        /// </summary>
        protected IPropertyAccessor<IGenericColumns, TColumn> ColumnAccessor { get; private set; }

        /// <summary>
        /// Creates a new instance of this type
        /// </summary>
        protected ColumnMapper(Type targetType)
        {
            TargetType = targetType;
        }

        /// <inheritdoc />
        public void Initialize(PropertyMapperConfig config)
        {
            Config = config;

            // Create accessor for the column property
            Column = typeof(IGenericColumns).GetProperty(config.Column);
            if (Column == null || Column.PropertyType != typeof(TColumn))
                throw new ArgumentException($"Column not found or type mismatch {config.PropertyName}");

            ColumnAccessor = ReflectionTool.PropertyAccessor<IGenericColumns, TColumn>(Column);

            // Retrieve and validate properties
            Property = TargetType.GetProperty(config.PropertyName);
            if (Property == null)
                throw new ArgumentException($"Target type {TargetType.Name} does not have a property {config.PropertyName}");

            // Create delegates for the object property as well
            ObjectAccessor = CreatePropertyAccessor(Property);
        }

        protected virtual IPropertyAccessor<object, TColumn> CreatePropertyAccessor(PropertyInfo objectProp)
        {
            // Default conversions
            return ReflectionTool.PropertyAccessor<object, TColumn>(objectProp);
        }

        /// <inheritdoc />
        public void Start()
        {
        }

        /// <inheritdoc />
        public void Stop()
        {
        }

        /// <summary>
        /// Property on the target object
        /// </summary>
        public PropertyInfo Property { get; private set; }

        /// <summary>
        /// Column it is mapped to
        /// </summary>
        protected PropertyInfo Column { get; private set; }

        /// <inheritdoc />
        public Expression ToColumnExpression(ParameterExpression columnParam, ExpressionType type, object value)
        {
            var columnMember = Expression.Property(columnParam, Config.Column);
            var convertedValue = ToExpressionValue(value);
            var constant = Expression.Constant(convertedValue);
            return Expression.MakeBinary(type, columnMember, constant);
        }

        protected virtual object ToExpressionValue(object value)
        {
            return Convert.ChangeType(value, Column.PropertyType);
        }

        /// <inheritdoc />
        public bool HasChanged(IGenericColumns current, object updated)
        {
            var objectValue = ObjectAccessor.ReadProperty(updated);
            var columnValue = ColumnAccessor.ReadProperty(current);

            if (columnValue == null)
                return objectValue != null;

            return !columnValue.Equals(objectValue);
        }

        /// <inheritdoc />
        public void ReadValue(IGenericColumns source, object target)
        {
            var value = ColumnAccessor.ReadProperty(source);
            ObjectAccessor.WriteProperty(target, value);
        }

        /// <inheritdoc />
        public void WriteValue(object source, IGenericColumns target)
        {
            var value = ObjectAccessor.ReadProperty(source);
            ColumnAccessor.WriteProperty(target, value);
        }
    }
}