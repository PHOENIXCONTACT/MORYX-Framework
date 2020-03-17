// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Reflection;
using Marvin.AbstractionLayer;
using Marvin.Container;
using Marvin.Modules;
using Marvin.Products.Model;
using Marvin.Serialization;
using Marvin.Tools;
using Newtonsoft.Json;

namespace Marvin.Products.Management
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

    /// <summary>
    /// Mapper for columns of type <see cref="long"/>
    /// </summary>
    [PropertyStrategyConfiguration(typeof(short), typeof(ushort), typeof(int), typeof(uint), 
        typeof(long), typeof(ulong), typeof(Enum), typeof(DateTime), typeof(bool), ColumnType = typeof(long))]
    [Component(LifeCycle.Transient, typeof(IPropertyMapper), Name = nameof(IntegerColumnMapper))]
    internal class IntegerColumnMapper : ColumnMapper<long>
    {
        public IntegerColumnMapper(Type targetType) : base(targetType)
        {

        }

        protected override IPropertyAccessor<object, long> CreatePropertyAccessor(PropertyInfo objectProp)
        {
            if (objectProp.PropertyType.IsEnum)
                return new EnumAccessor(objectProp);

            if (objectProp.PropertyType == typeof(DateTime))
                return new DateTimeAccessor(objectProp);

            if(objectProp.PropertyType == typeof(bool))
                return new BoolAccessor(objectProp);

            return base.CreatePropertyAccessor(objectProp);
        }

        /// <summary>
        /// Accessor decorator performing enum conversion
        /// </summary>
        private class EnumAccessor : ConversionAccessor<long, object>
        {
            private readonly Type _underlyingType;

            public EnumAccessor(PropertyInfo property) : base(property)
            {
                _underlyingType = Enum.GetUnderlyingType(property.PropertyType);
            }

            public override long ReadProperty(object instance)
            {
                var value = Target.ReadProperty(instance);
                var underlyingValue = Convert.ChangeType(value, _underlyingType);
                return (long)Convert.ChangeType(underlyingValue, TypeCode.Int64);
            }

            public override void WriteProperty(object instance, long value)
            {
                var underlyingValue = Convert.ChangeType(value, _underlyingType);
                var enumValue = Enum.ToObject(Target.Property.PropertyType, underlyingValue);
                Target.WriteProperty(instance, enumValue);
            }
        }

        /// <summary>
        /// Accessor decorator performing DateTime conversion
        /// </summary>
        private class DateTimeAccessor : ConversionAccessor<long, DateTime>
        {
            public DateTimeAccessor(PropertyInfo property) : base(property)
            {
            }

            public override long ReadProperty(object instance)
            {
                return Target.ReadProperty(instance).Ticks;
            }

            public override void WriteProperty(object instance, long value)
            {
                Target.WriteProperty(instance, DateTime.FromBinary(value));
            }
        }

        private class BoolAccessor : ConversionAccessor<long, bool>
        {
            public BoolAccessor(PropertyInfo property) : base(property)
            {
            }

            public override long ReadProperty(object instance)
            {
                var value = Target.ReadProperty(instance);
                return value ? 1 : 0;
            }

            public override void WriteProperty(object instance, long value)
            {
                Target.WriteProperty(instance, value > 0);
            }
        }
    }

    /// <summary>
    /// Mapper for columns of type <see cref="double"/>
    /// </summary>
    [PropertyStrategyConfiguration(typeof(float), typeof(double), typeof(decimal), ColumnType = typeof(double))]
    [Component(LifeCycle.Transient, typeof(IPropertyMapper), Name = nameof(FloatColumnMapper))]
    internal class FloatColumnMapper : ColumnMapper<double>
    {
        public FloatColumnMapper(Type targetType) : base(targetType)
        {
        }
    }

    /// <summary>
    /// Mapper for columns of type <see cref="string"/>
    /// </summary>
    [PropertyStrategyConfiguration(typeof(string), typeof(object), ColumnType = typeof(string), DerivedTypes = true)]
    [Component(LifeCycle.Transient, typeof(IPropertyMapper), Name = nameof(TextColumnMapper))]
    internal class TextColumnMapper : ColumnMapper<string>
    {
        public TextColumnMapper(Type targetType) : base(targetType)
        {

        }

        protected override IPropertyAccessor<object, string> CreatePropertyAccessor(PropertyInfo objectProp)
        {
            // Convert return value to string
            if (objectProp.PropertyType.IsClass && objectProp.PropertyType != typeof(string))
                return new JsonAccessor(objectProp);

            return base.CreatePropertyAccessor(objectProp);
        }

        /// <summary>
        /// Accessor decorator to convert objects to enum and back
        /// </summary>
        private class JsonAccessor : ConversionAccessor<string, object>
        {
            public JsonAccessor(PropertyInfo property) : base(property)
            {
            }

            public override string ReadProperty(object instance)
            {
                var value = Target.ReadProperty(instance);
                return JsonConvert.SerializeObject(value, Target.Property.PropertyType, JsonSettings.Minimal);
            }

            public override void WriteProperty(object instance, string value)
            {
                var deserialized = JsonConvert.DeserializeObject(value, Target.Property.PropertyType);
                Target.WriteProperty(instance, deserialized);
            }
        }
    }

    /// <summary>
    /// Base class for all internal accessor wrappers with type conversion
    /// </summary>
    internal abstract class ConversionAccessor<TColumn, TProperty> : IPropertyAccessor<object, TColumn>
    {
        protected IPropertyAccessor<object, TProperty> Target { get; }

        public string Name => Target.Name;

        public PropertyInfo Property => Target.Property;

        protected ConversionAccessor(PropertyInfo property)
        {
            Target = ReflectionTool.PropertyAccessor<object, TProperty>(property);
        }

        public abstract TColumn ReadProperty(object instance);

        public abstract void WriteProperty(object instance, TColumn value);
    }
}
