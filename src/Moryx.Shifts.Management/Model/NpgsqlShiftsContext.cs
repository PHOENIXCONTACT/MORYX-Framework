// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.PostgreSQL;

namespace Moryx.Shifts.Management.Model
{
    /// <summary>
    /// Npgsql specific implementation of <see cref="ShiftsContext"/>
    /// </summary>
    [NpgsqlDbContext(typeof(ShiftsContext))]
    public class NpgsqlShiftsContext : ShiftsContext
    {
        public NpgsqlShiftsContext()
        {
        }

        public NpgsqlShiftsContext(DbContextOptions options) : base(options)
        {
        }
    }
}

