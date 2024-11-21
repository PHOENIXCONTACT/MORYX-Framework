// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.Sqlite.Attributes;

namespace Moryx.Products.Model
{
    /// <summary>
    /// Sqlite specific implementation of <see cref="ProductsContext"/>
    /// </summary>
    [SqliteContext]
    public class SqliteProductsContext : ProductsContext
    {
        /// <inheritdoc />
        public SqliteProductsContext()
        {
        }

        /// <inheritdoc />
        public SqliteProductsContext(DbContextOptions options) : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
