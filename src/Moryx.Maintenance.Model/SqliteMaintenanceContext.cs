// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.Sqlite;

namespace Moryx.Maintenance.Model;

/// <inheritdoc />
[SqliteDbContext(typeof(MaintenanceContext))]
public class SqliteMaintenanceContext : MaintenanceContext
{
    /// <inheritdoc />
    public SqliteMaintenanceContext()
    {
    }

    public SqliteMaintenanceContext(DbContextOptions options) : base(options)
    {
    }

    /// <inheritdoc />
    public SqliteMaintenanceContext(DbContextOptions<SqliteMaintenanceContext> options) : base(options)
    {
    }
}
