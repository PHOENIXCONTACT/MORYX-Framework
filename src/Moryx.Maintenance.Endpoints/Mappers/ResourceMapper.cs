// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Moryx.Maintenance.Management.Models;

namespace Moryx.Maintenance.Endpoints.Mappers;
internal static class ResourceMapper
{
    public static ResourceModel ToDto(this IMaintainableResource resource)
        => new ResourceModel(resource.Id, resource.Name);
}
