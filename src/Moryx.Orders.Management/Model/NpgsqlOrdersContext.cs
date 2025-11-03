// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.PostgreSQL;

namespace Moryx.Orders.Management.Model
{
    /// <summary>
    /// Npgsql specific implementation of <see cref="OrdersContext"/>
    /// </summary>
    [NpgsqlDbContext(typeof(OrdersContext))]
    public class NpgsqlOrdersContext : OrdersContext
    {
        /// <inheritdoc />
        public NpgsqlOrdersContext()
        {
        }

        /// <inheritdoc />
        public NpgsqlOrdersContext(DbContextOptions options) : base(options)
        {
        }
    }
}

