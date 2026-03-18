// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Serialization;

namespace Moryx.AbstractionLayer.Products.Endpoints;

internal class ProductImportParametersSerialization : PossibleValuesSerialization
{
    /// <summary>
    /// Instance for <see cref="EntrySerializeSerialization"/> we use to filter properties and methods
    /// </summary>
    private readonly EntrySerializeSerialization _memberFilter = new();

    public ProductImportParametersSerialization(IContainer container, IServiceProvider serviceProvider, IEmptyPropertyProvider emptyPropertyProvider)
        : base(container, serviceProvider, emptyPropertyProvider)
    {
    }

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

    /// <summary>
    /// Follow the rules for <see cref="EntrySerializeSerialization"/>
    /// </summary>
    public override IEnumerable<MappedProperty> WriteFilter(Type sourceType, IEnumerable<Entry> encoded)
    {
        return _memberFilter.WriteFilter(sourceType, encoded);

    }
}
