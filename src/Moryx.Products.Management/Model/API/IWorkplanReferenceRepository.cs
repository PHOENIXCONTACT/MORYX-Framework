// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Repositories;

namespace Moryx.Products.Management.Model;

/// <summary>
/// The public API of the WorkplanReference repository.
/// </summary>
public interface IWorkplanReferenceRepository : IRepository<WorkplanReferenceEntity>
{
    /// <summary>
    /// Creates instance with all not nullable properties prefilled
    /// </summary>
    WorkplanReferenceEntity Create(int referenceType);
}