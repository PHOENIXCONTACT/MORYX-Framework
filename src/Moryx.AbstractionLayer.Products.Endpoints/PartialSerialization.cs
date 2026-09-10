// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.AbstractionLayer.Recipes;
using Moryx.AbstractionLayer.Resources;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Serialization;
using Moryx.Workplans;

namespace Moryx.AbstractionLayer.Products.Endpoints;

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
    private static readonly string[] FilteredProperties = typeof(T).GetProperties().Select(p => p.Name).ToArray();

    static PartialSerialization()
    {
        _serialization = new EntrySerializeSerialization();
    }

    /// <summary>
    /// Create serialization with access to global and local container
    /// </summary>
    /// <param name="localContainer"></param>
    /// <param name="serviceProvider"></param>
    public PartialSerialization(IContainer localContainer, IServiceProvider serviceProvider)
        : base(localContainer, serviceProvider, new EmptyValueProvider())
    {
    }

    /// <see cref="T:Moryx.Serialization.ICustomSerialization"/>
    public override IEnumerable<PropertyInfo> GetProperties(Type sourceType)
    {
        // Only simple properties not defined in the base
        return _serialization.GetProperties(sourceType).Where(SimpleProp);
    }

    /// <summary>
    /// List of all types where 'base'-propertyname like 'name' or 'id' should be allowed in complex properties
    /// </summary>
    private readonly List<Type> _typesOfNewSerialization =
    [
        typeof(ProductType),
        typeof(Resource)
    ];

    protected bool SimpleProp(PropertyInfo prop)
    {
        // Skip reference or domain model properties
        var type = prop.PropertyType;
        if (typeof(ProductType).IsAssignableFrom(type) ||
            typeof(ProductPartLink).IsAssignableFrom(type) ||
            typeof(IEnumerable<ProductPartLink>).IsAssignableFrom(type))
            return false;

        var testType = prop.DeclaringType;
        var found = _typesOfNewSerialization.Contains(testType);
        while (testType.BaseType != null && !found)
        {
            if (_typesOfNewSerialization.Contains(testType.BaseType))
                found = true;
            testType = testType.BaseType;
        }

        if (!found)
        {
            // Filter default properties
            if (FilteredProperties.Contains(prop.Name))
                return false;
        }
        else
        {
            // Filter default properties
            if (_typesOfNewSerialization.Contains(prop.DeclaringType))
                return false;
        }

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
