// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.Attributes;
using Moryx.Model.Sqlite;
using Moryx.Model.Sqlite.Attributes;

namespace Moryx.Products.Model
{
    /// <summary>
    /// Sqlite specific implementation of <see cref="ProductsContext"/>
    /// </summary>
    [SqliteContext]
    [ModelConfigurator(typeof(SqliteModelConfigurator))]
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
    }
}
