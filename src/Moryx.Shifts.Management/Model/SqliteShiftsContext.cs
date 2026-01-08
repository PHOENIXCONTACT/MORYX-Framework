// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.Sqlite;

namespace Moryx.Shifts.Management.Model;

/// <summary>
/// Sqlite specific implementation of <see cref="ShiftsContext"/>
/// </summary>
[SqliteDbContext(typeof(ShiftsContext))]
public class SqliteShiftsContext : ShiftsContext
{
    public SqliteShiftsContext()
    {
    }

    public SqliteShiftsContext(DbContextOptions options) : base(options)
    {
    }
}