// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq.Expressions;
using System.Reflection;
using Moryx.Container;
using Moryx.Products.Management.Model;

namespace Moryx.Products.Management;

/// <summary>
/// Null property mapper which does not read or write properties from any column
/// </summary>
[Component(LifeCycle.Transient, typeof(IPropertyMapper), Name = nameof(NullPropertyMapper))]
internal class NullPropertyMapper : IPropertyMapper
{
    private readonly Type _targetType;

    public PropertyInfo Property { get; private set; }

    public NullPropertyMapper(Type targetType)
    {
        _targetType = targetType;
    }

    public void Initialize(PropertyMapperConfig config)
    {
        // Retrieve and validate properties
        Property = _targetType.GetProperty(config.PropertyName);
    }

    public void Start()
    {

    }

    public void Stop()
    {
    }

    public bool HasChanged(IGenericColumns current, object updated)
    {
        return false;
    }

    public void ReadValue(IGenericColumns source, object target)
    {
    }

    public void WriteValue(object source, IGenericColumns target)
    {
    }

    public Expression ToColumnExpression(ParameterExpression columnParam, ExpressionType type, object value)
    {
        return Expression.Constant(true);
    }
}