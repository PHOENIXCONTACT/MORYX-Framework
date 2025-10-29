// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.Sqlite;

namespace Moryx.Orders.Management.Model
{
    /// <summary>
    /// The DBContext of this database model.
    /// </summary>
    [SqliteDbContext(typeof(OrdersContext))]
    public class SqliteOrdersContext : OrdersContext
    {
        /// <inheritdoc />
        public SqliteOrdersContext()
        {
        }

        /// <inheritdoc />
        public SqliteOrdersContext(DbContextOptions options) : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}

