// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;

namespace Moryx.Operators.Extensions;

public static class ResourceManagementExtensions
{
    public static IEnumerable<IOperatorAssignable> GetAssignableResources(this ResourceManagement source) =>
        source.GetResources<IOperatorAssignable>();

    public static IOperatorAssignable GetAssignableResource(this ResourceManagement source, long resourceId) =>
        source.GetResource<IOperatorAssignable>(resourceId);
}

