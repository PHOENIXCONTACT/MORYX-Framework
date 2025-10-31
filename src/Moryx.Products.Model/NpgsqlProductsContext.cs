// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.PostgreSQL;

namespace Moryx.Products.Model
{
    /// <summary>
    /// Npgsql specific implementation of <see cref="ProductsContext"/>
    /// </summary>
    [NpgsqlDbContext(typeof(ProductsContext))]
    public class NpgsqlProductsContext : ProductsContext
    {
        /// <inheritdoc />
        public NpgsqlProductsContext()
        {
        }

        /// <inheritdoc />
        public NpgsqlProductsContext(DbContextOptions options) : base(options)
        {
        }
    }
}
