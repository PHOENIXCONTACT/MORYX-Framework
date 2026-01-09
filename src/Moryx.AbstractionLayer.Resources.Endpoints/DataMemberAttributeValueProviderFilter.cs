// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using System.Runtime.Serialization;
using Moryx.Configuration;

namespace Moryx.AbstractionLayer.Resources.Endpoints;

internal class DataMemberAttributeValueProviderFilter : IValueProviderFilter
{
    private readonly bool _filterDataMembers;

    public DataMemberAttributeValueProviderFilter(bool filterDataMembers)
    {
        _filterDataMembers = filterDataMembers;
    }

    public bool CheckProperty(PropertyInfo propertyInfo)
    {
        if (_filterDataMembers)
            return propertyInfo.GetCustomAttribute<DataMemberAttribute>() == null;
        return propertyInfo.GetCustomAttribute<DataMemberAttribute>() != null;
    }
}