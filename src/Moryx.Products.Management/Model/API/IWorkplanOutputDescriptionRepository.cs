// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Repositories;

namespace Moryx.Products.Management.Model;

/// <summary>
/// The public API of the OutputDescriptionEntity repository.
/// </summary>
public interface IWorkplanOutputDescriptionRepository : IRepository<WorkplanOutputDescriptionEntity>
{
    /// <summary>
    /// Creates instance with all not nullable properties prefilled
    /// </summary>
    WorkplanOutputDescriptionEntity Create(int index, int outputType, long mappingValue);
}
