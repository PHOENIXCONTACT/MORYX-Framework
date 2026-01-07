// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.PostgreSQL;

namespace Moryx.Maintenance.Model;

/// <inheritdoc />
[NpgsqlDbContext(typeof(MaintenanceContext))]
public class NpgsqlMaintenanceContext : MaintenanceContext
{
    /// <inheritdoc />
    public NpgsqlMaintenanceContext()
    {
    }

    /// <inheritdoc />
    public NpgsqlMaintenanceContext(DbContextOptions options) : base(options)
    {
    }

    /// <inheritdoc />
    public NpgsqlMaintenanceContext(DbContextOptions<NpgsqlMaintenanceContext> options) : base(options)
    {
    }

}
