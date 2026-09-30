// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.Container;
using Moryx.Serialization;
using Moryx.Tools;

namespace Moryx.Configuration;

/// <summary>
/// Base class for config to model transformer
/// </summary>
public class PossibleValuesSerialization : DefaultSerialization
{
    /// <summary>
    /// Container used to include current information from current composition into the configuration
    /// </summary>
    protected IContainer Container { get; }

    /// <summary>
    /// Access to level 1 service registration
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Empty property provider to pre-fill newley created objects
    /// </summary>
    protected IEmptyPropertyProvider EmptyPropertyProvider { get; }

    /// <summary>
    /// Initialize base class
    /// </summary>
    public PossibleValuesSerialization(IContainer container, IServiceProvider serviceProvider, IEmptyPropertyProvider emptyPropertyProvider)
    {
        Container = container;
        ServiceProvider = serviceProvider;
        EmptyPropertyProvider = emptyPropertyProvider;
    }

    /// <see cref="T:Moryx.Serialization.ICustomSerialization"/>
    public override EntryPrototype[] Prototypes(Type memberType, ICustomAttributeProvider attributeProvider)
    {
        var possibleValuesAtt = attributeProvider.GetCustomAttribute<PossibleValuesAttribute>();
        // We can not create prototypes for possible primitives
        if (possibleValuesAtt == null || IsPrimitiveCollection(memberType))
            return base.Prototypes(memberType, attributeProvider);

        // Create prototypes from possible values
        var list = new List<EntryPrototype>();
        foreach (var value in possibleValuesAtt.GetValues(Container, ServiceProvider))
        {
            var prototype = possibleValuesAtt.Parse(Container, ServiceProvider, value);
            EmptyPropertyProvider.FillEmpty(prototype);
            list.Add(new EntryPrototype(value, prototype));
        }
        return list.ToArray();
    }

    /// <see cref="T:Moryx.Serialization.ICustomSerialization"/>
    public override EntryPossible[] PossibleValues(Type memberType, ICustomAttributeProvider attributeProvider) => PossibleValues(null, memberType, attributeProvider);

    /// <summary>
    /// Extended method for <see cref="PossibleValuesAttribute.GetValues(IContainer, IServiceProvider, Serialization.PossibleValues.PossibleValuesContext)"/>
    /// </summary>
    /// <param name="instance">current instance of the class</param>
    /// <param name="memberType">current property</param>
    /// <param name="attributeProvider">attribute provider</param>
    /// <returns></returns>
    public virtual EntryPossible[] PossibleValues(object instance, Type memberType, ICustomAttributeProvider attributeProvider)
    {
        var possibleValuesAttribute = attributeProvider.GetCustomAttribute<PossibleValuesAttribute>();
        // Possible values for primitive collections only apply to members
        if (possibleValuesAttribute == null || IsPrimitiveCollection(memberType))
            return base.PossibleValues(memberType, attributeProvider);

        // Use attribute
        return GetEntryPossibles(instance, possibleValuesAttribute);
    }

    private EntryPossible[] GetEntryPossibles(object instance, PossibleValuesAttribute possibleValuesAttribute)
    {
        IEnumerable<string> values;
        if (possibleValuesAttribute.RequiresPossibleValuesContext)
        {
            var context = new PossibleValuesContext(instance, new Dictionary<object, object>());
            values = possibleValuesAttribute.GetValues(Container, ServiceProvider, context);
        }
        else
        {
            values = possibleValuesAttribute.GetValues(Container, ServiceProvider);
        }
        return EntryPossible.FromStrings(values?.Distinct());
    }

    /// <summary>
    /// Check if a property is a collection of primitives
    /// </summary>
    private static bool IsPrimitiveCollection(Type memberType)
    {
        if (!EntryConvert.IsCollection(memberType))
            return false;

        var elementType = EntryConvert.ElementType(memberType);
        return EntryConvert.ValueOrStringType(elementType);
    }

    /// <see cref="T:Moryx.Serialization.ICustomSerialization"/>
    public override EntryPossible[] PossibleElementValues(Type memberType, ICustomAttributeProvider attributeProvider)
    {
        var valuesAttribute = attributeProvider.GetCustomAttribute<PossibleValuesAttribute>();
        if (valuesAttribute == null)
        {
            return base.PossibleElementValues(memberType, attributeProvider);
        }

        // Use attribute
        return GetEntryPossibles(null, valuesAttribute);
    }

    /// <see cref="T:Moryx.Serialization.ICustomSerialization"/>
    public override object CreateInstance(Type memberType, ICustomAttributeProvider attributeProvider, Entry encoded)
    {
        var possibleValuesAtt = attributeProvider.GetCustomAttribute<PossibleValuesAttribute>();
        var instance = possibleValuesAtt != null
            ? possibleValuesAtt.Parse(Container, ServiceProvider, encoded.Value.Current)
            : base.CreateInstance(memberType, attributeProvider, encoded);

        EmptyPropertyProvider.FillEmpty(instance);

        return instance;
    }

    /// <see cref="T:Moryx.Serialization.ICustomSerialization"/>
    public override object ConvertValue(Type memberType, ICustomAttributeProvider attributeProvider, Entry mappedEntry, object currentValue)
    {
        var value = mappedEntry.Value;

        var att = attributeProvider.GetCustomAttribute<PossibleValuesAttribute>();
        if (att == null || !att.OverridesConversion || value.Type == EntryValueType.Collection)
            return base.ConvertValue(memberType, attributeProvider, mappedEntry, currentValue);

        // If old and current type are identical, keep the object
        if (value.Type == EntryValueType.Class && currentValue != null && currentValue.GetType().Name == value.Current)
            return currentValue;

        var instance = att.Parse(Container, ServiceProvider, mappedEntry.Value.Current);
        if (mappedEntry.Value.Type == EntryValueType.Class)
        {
            EmptyPropertyProvider.FillEmpty(instance);
        }

        return instance;
    }
}
