// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.Sqlite;

namespace Moryx.Shifts.Management.Model
{
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
}

