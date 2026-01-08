// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.Sqlite;

namespace Moryx.Orders.Management.Model;

/// <summary>
/// Sqlite specific implementation of <see cref="OrdersContext"/>
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
}