// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using Moryx.Model.PostgreSQL;

namespace Moryx.Shifts.Management.Model
{
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

