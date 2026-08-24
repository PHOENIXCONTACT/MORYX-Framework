// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;

namespace Moryx.Tools;

/// <summary>
/// Property accessor for fast, dynamic access to properties
/// </summary>
internal abstract class PropertyAccessor<TConcrete, TProperty>
{
    protected Func<TConcrete, TProperty> PropertyGetter { get; }

    protected Action<TConcrete, TProperty> PropertySetter { get; }

    public string Name => Property.Name;

    public PropertyInfo Property { get; }

    protected PropertyAccessor(PropertyInfo property)
    {
        Property = property;

        if (property.CanRead)
        {
            PropertyGetter = (Func<TConcrete, TProperty>)Delegate.CreateDelegate(typeof(Func<TConcrete, TProperty>), property.GetMethod);
        }
        else
        {
            PropertyGetter = EmptyGetter;
        }

        if (property.CanWrite)
        {
            PropertySetter = (Action<TConcrete, TProperty>)Delegate.CreateDelegate(typeof(Action<TConcrete, TProperty>), property.SetMethod);
        }
        else
        {
            PropertySetter = EmptySetter;
        }
    }

    private static TProperty EmptyGetter(TConcrete instance) => default(TProperty);

    private static void EmptySetter(TConcrete instance, TProperty value) { }
}

/// <summary>
/// Accessor where instance type and property are known
/// </summary>
internal class DirectAccessor<TConcrete, TProperty> : PropertyAccessor<TConcrete, TProperty>, IPropertyAccessor<TConcrete, TProperty>
{
    public DirectAccessor(PropertyInfo property) : base(property)
    {
    }

    public TProperty ReadProperty(TConcrete instance)
    {
        return PropertyGetter(instance);
    }

    public void WriteProperty(TConcrete instance, TProperty value)
    {
        PropertySetter(instance, value);
    }
}

/// <summary>
/// Accessor for properties on derived types
/// </summary>
internal class InstanceCastAccessor<TConcrete, TBase, TProperty> : PropertyAccessor<TConcrete, TProperty>, IPropertyAccessor<TBase, TProperty>
    where TConcrete : TBase
{
    public InstanceCastAccessor(PropertyInfo property) : base(property)
    {
    }

    public TProperty ReadProperty(TBase instance)
    {
        return PropertyGetter((TConcrete)instance);
    }

    public void WriteProperty(TBase instance, TProperty value)
    {
        PropertySetter((TConcrete)instance, value);
    }
}

/// <summary>
/// Accessor for properties with derived types on derived instances
/// </summary>
internal class ValueCastAccessor<TConcrete, TBase, TProperty, TValue> : PropertyAccessor<TConcrete, TProperty>, IPropertyAccessor<TBase, TValue>
    where TConcrete : TBase
    where TProperty : TValue
{
    public ValueCastAccessor(PropertyInfo property) : base(property)
    {
    }

    public TValue ReadProperty(TBase instance)
    {
        return PropertyGetter((TConcrete)instance);
    }

    public void WriteProperty(TBase instance, TValue value)
    {
        PropertySetter((TConcrete)instance, (TProperty)value);
    }
}

/// <summary>
/// Accessor for properties with derived types on derived instances
/// </summary>
internal class PropertyCastAccessor<TConcrete, TBase, TProperty, TValue> : PropertyAccessor<TConcrete, TProperty>, IPropertyAccessor<TBase, TValue>
    where TConcrete : TBase
    where TValue : TProperty
{
    public PropertyCastAccessor(PropertyInfo property) : base(property)
    {
    }

    public TValue ReadProperty(TBase instance)
    {
        return (TValue)PropertyGetter((TConcrete)instance);
    }

    public void WriteProperty(TBase instance, TValue value)
    {
        PropertySetter((TConcrete)instance, value);
    }
}

/// <summary>
/// Fallback accessor if there is no known inheritance of values
/// </summary>
internal class ConversionAccessor<TConcrete, TBase, TProperty, TValue> : PropertyAccessor<TConcrete, TProperty>, IPropertyAccessor<TBase, TValue>
    where TConcrete : TBase
{
    /// <summary>
    /// Conversion targets for ChangeType. It can not handle
    /// <see cref="Nullable{T}"/> as target type, but converting to the underlying type and casting
    /// the result to the nullable type afterward is equivalent.
    /// </summary>
    private static readonly Type _valueTarget = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
    private static readonly Type _propertyTarget = Nullable.GetUnderlyingType(typeof(TProperty)) ?? typeof(TProperty);

    public ConversionAccessor(PropertyInfo property) : base(property)
    {
    }

    public TValue ReadProperty(TBase instance)
    {
        var value = (object)PropertyGetter((TConcrete)instance);

        // Check if the value can be cast or needs conversion
        if (value is TValue castable)
        {
            return castable;
        }

        // Convert.ChangeType would throw for value types, so a null value is
        // represented by the default of the target type
        if (value == null)
        {
            return default;
        }

        return (TValue)Convert.ChangeType(value, _valueTarget);
    }

    public void WriteProperty(TBase instance, TValue value)
    {
        // Check if the value can be cast or needs conversion
        if (value is TProperty)
        {
            PropertySetter((TConcrete)instance, (TProperty)(object)value);
        }
        else if (value is null)
        {
            PropertySetter((TConcrete)instance, default);
        }
        else
        {
            PropertySetter((TConcrete)instance, (TProperty)Convert.ChangeType(value, _propertyTarget));
        }
    }
}
