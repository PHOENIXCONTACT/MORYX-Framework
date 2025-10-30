// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.PostgreSQL;

namespace Moryx.ControlSystem.ProcessEngine.Model
{
    /// <summary>
    /// The Npgsql DbContext of this database model.
    /// </summary>
    [NpgsqlDbContext(typeof(ProcessContext))]
    public class NpgsqlProcessContext : ProcessContext
    {
        /// <inheritdoc />
        public NpgsqlProcessContext()
        {
        }

        /// <inheritdoc />
        public NpgsqlProcessContext(DbContextOptions options) : base(options)
        {
        }
    }
}

