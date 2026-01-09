// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Repositories;

namespace Moryx.Resources.Management.Model;

/// <summary>
/// The public API of the ResourceEntity repository.
/// </summary>
public interface IResourceRepository : IRepository<ResourceEntity>
{
    /// <summary>
    /// Creates instance with all not nullable properties prefilled
    /// </summary>
    ResourceEntity Create(string name, string type);
}