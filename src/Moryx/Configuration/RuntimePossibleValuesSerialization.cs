// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.Container;
using Moryx.Serialization;
using Moryx.Serialization.PossibleValues;
using Moryx.Tools;

namespace Moryx.Configuration;

/// <summary>
///  Base class for runtime config to model transformer
/// </summary>
public class RuntimePossibleValuesSerialization(IContainer container, IServiceProvider serviceProvider, IEmptyPropertyProvider emptyPropertyProvider) : PossibleValuesSerialization(container, serviceProvider, emptyPropertyProvider)
{
    /// <inheritdoc/>
    public override EntryPossible[] PossibleValues(Type memberType, ICustomAttributeProvider attributeProvider)
    {
        return PossibleValues(null, memberType, attributeProvider);
    }

    /// <summary>
    /// Extended method for <see cref="PossibleValuesAttribute.PossibleValuesAttribute"/>
    /// </summary>
    /// <param name="instance">current instance of the class</param>
    /// <param name="memberType">current property</param>
    /// <param name="attributeProvider">attribute provider</param>
    /// <returns></returns>
    public virtual EntryPossible[] PossibleValues(object instance, Type memberType, ICustomAttributeProvider attributeProvider)
    {
        var runtimePossibleValuesAttribute = attributeProvider.GetCustomAttribute<RuntimePossibleValuesAttribute>();
        if (runtimePossibleValuesAttribute is null)
        {
            return base.PossibleValues(memberType, attributeProvider);
        }

        var values = runtimePossibleValuesAttribute.GetValues(instance, Container, ServiceProvider);
        return EntryPossible.FromStrings(values?.Distinct());
    }
}
