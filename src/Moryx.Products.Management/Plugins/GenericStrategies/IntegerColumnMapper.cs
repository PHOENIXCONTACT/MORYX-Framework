// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using System.Reflection;
using Moryx.Container;
using Moryx.Tools;

namespace Moryx.Products.Management;

/// <summary>
/// Mapper for columns of type <see cref="long"/>
/// </summary>
[IntegerStrategyConfiguration]
[Component(LifeCycle.Transient, typeof(IPropertyMapper), Name = nameof(IntegerColumnMapper))]
internal class IntegerColumnMapper : ColumnMapper<long>
{
    private Type _enumType;

    public IntegerColumnMapper(Type targetType) : base(targetType)
    {

    }

    protected override IPropertyAccessor<object, long> CreatePropertyAccessor(PropertyInfo objectProp)
    {
        if (objectProp.PropertyType.IsEnum)
        {
            _enumType = Enum.GetUnderlyingType(objectProp.PropertyType);
            return new ConversionAccessor<long, object>(objectProp, ReadEnum, WriteEnum);
        }

        if (objectProp.PropertyType == typeof(DateTime))
            return new ConversionAccessor<long, DateTime>(objectProp, dt => dt.Ticks, DateTime.FromBinary);

        if (objectProp.PropertyType == typeof(DateOnly))
            return new ConversionAccessor<long, DateOnly>(objectProp, d => d.DayNumber, l => DateOnly.FromDayNumber((int)l));

        if (objectProp.PropertyType == typeof(TimeOnly))
            return new ConversionAccessor<long, TimeOnly>(objectProp, t => t.Ticks, l => new TimeOnly(l));

        if (objectProp.PropertyType == typeof(bool))
            return new ConversionAccessor<long, bool>(objectProp, v => v ? 1 : 0, l => l > 0);

        return base.CreatePropertyAccessor(objectProp);
    }

    protected override object ToExpressionValue(object value)
    {
        if (Property.PropertyType.IsEnum)
            return ReadEnum(value);

        if (Property.PropertyType == typeof(DateTime))
            return ((DateTime)value).Ticks;

        if (Property.PropertyType == typeof(DateOnly))
            return (long)((DateOnly)value).DayNumber;

        if (Property.PropertyType == typeof(TimeOnly))
            return ((TimeOnly)value).Ticks;

        if (Property.PropertyType == typeof(bool))
            return (bool)value ? 1L : 0L;

        return base.ToExpressionValue(value);
    }

    private long ReadEnum(object value)
    {
        var underlyingValue = Convert.ChangeType(value, _enumType, CultureInfo.InvariantCulture);
        return (long)Convert.ChangeType(underlyingValue, TypeCode.Int64, CultureInfo.InvariantCulture);
    }
    private object WriteEnum(long value)
    {
        var underlyingValue = Convert.ChangeType(value, _enumType, CultureInfo.InvariantCulture);
        return Enum.ToObject(Property.PropertyType, underlyingValue);
    }
}
