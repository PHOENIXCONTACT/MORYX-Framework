// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Reflection;
using Marvin.Container;
using Marvin.Tools;

namespace Marvin.Products.Management
{
    /// <summary>
    /// Mapper for columns of type <see cref="long"/>
    /// </summary>
    [IntegerStrategyConfiguration]
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
}