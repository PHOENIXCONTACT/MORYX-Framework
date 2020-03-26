// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Model;

namespace Marvin.Products.Model
{
    /// <summary>
    /// Repository for product files
    /// </summary>
    public interface IProductFileEntityRepository : IRepository<ProductFileEntity>
    {
        /// <summary>
        /// Create a new <see cref="ProductFileEntity"/>
        /// </summary>
        ProductFileEntity Create(string fileName, string mimeType);
    }
}
