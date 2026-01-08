// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;

namespace Moryx.Operators.Attendances;

public static class IResourceManagementExtensions
{
    extension(IResourceManagement source)
    {
        public IEnumerable<IOperatorAssignable> GetAssignableResources() =>
            source.GetResources<IOperatorAssignable>();

        public IOperatorAssignable GetAssignableResource(long resourceId) =>
            source.GetResource<IOperatorAssignable>(resourceId);
    }
}

