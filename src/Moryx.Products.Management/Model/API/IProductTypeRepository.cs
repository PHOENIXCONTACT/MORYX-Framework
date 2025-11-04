// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Repositories;

namespace Moryx.Products.Management.Model;

/// <summary>
/// The public API of the ProductEntity repository.
/// </summary>
public interface IProductTypeRepository : IRepository<ProductTypeEntity>
{
    /// <summary>
    /// Creates instance with all not nullable properties prefilled
    /// </summary>
    ProductTypeEntity Create(string identifier, short revision, string name, string typeName);

    /// <summary>
    /// This method returns the first matching ProductEntity for given parameters
    /// </summary>
    /// <param name="identifier">Value for MaterialNumber the ProductEntity has to match</param>
    /// <param name="revision">Value for Revision the ProductEntity has to match</param>
    ProductTypeEntity GetByIdentity(string identifier, short revision);
}