// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using System.Reflection;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Serialization;

namespace Moryx.Maintenance.Endpoints;

internal sealed class MaintenanceOrderSerialization(IContainer container, IServiceProvider serviceProvider)
    : PossibleValuesSerialization(container, serviceProvider, new ValueProviderExecutor(new ValueProviderExecutorSettings()))
{
    ///// <summary>
    ///// Instance for <see cref="EntrySerializeSerialization"/> we use to filter properties and methods
    ///// </summary>
    private readonly EntrySerializeSerialization _memberFilter = new();

    /// <summary>
    /// Follow the rules for <see cref="EntrySerializeSerialization"/>
    /// </summary>
    public override IEnumerable<PropertyInfo> GetProperties(Type sourceType)
    {
        return _memberFilter.GetProperties(sourceType);
    }

    /// <summary>
    /// Follow the rules for <see cref="EntrySerializeSerialization"/>
    /// </summary>
    public override IEnumerable<MethodInfo> GetMethods(Type sourceType)
    {
        return _memberFilter.GetMethods(sourceType);
    }

    public override IEnumerable<MappedProperty> WriteFilter(Type sourceType, IEnumerable<Entry> encoded)
    {
        return _memberFilter.WriteFilter(sourceType, encoded);

    }
}
