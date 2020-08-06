// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Repositories;

namespace Moryx.Products.Model
{
    /// <summary>
    /// The public API of the ProductRecipeEntity repository.
    /// </summary>
    public interface IProductRecipeEntityRepository : IRepository<ProductRecipeEntity>
    {
        ProductRecipeEntity Create(string type, string name, int classification, int state);
    }
}
