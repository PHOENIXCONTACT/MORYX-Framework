// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Repositories;

namespace Moryx.Products.Management.Model;

/// <summary>
/// The public API of the PartLink repository.
/// </summary>
public interface IPartLinkRepository : IRepository<PartLinkEntity>
{
    /// <summary>
    /// Creates instance with all not nullable properties prefilled
    /// </summary>
    PartLinkEntity Create(string propertyName);
}