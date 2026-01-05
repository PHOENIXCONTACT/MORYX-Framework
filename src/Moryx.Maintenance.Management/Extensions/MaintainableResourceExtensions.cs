// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Moryx.Maintenance.Management.Models;

namespace Moryx.Maintenance.Management.Extensions;

internal static class MaintainableResourceExtensions
{
    public static ResourceModel ToDto(this IMaintainableResource resource)
        => new(resource.Id, resource.Name);
}
