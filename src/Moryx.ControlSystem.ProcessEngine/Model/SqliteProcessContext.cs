// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.Sqlite;

namespace Moryx.ControlSystem.ProcessEngine.Model
{
    /// <summary>
    /// Sqlite specific implementation of <see cref="ProcessContext"/>
    /// </summary>
    [SqliteDbContext(typeof(ProcessContext))]
    public class SqliteProcessContext : ProcessContext
    {
        /// <inheritdoc />
        public SqliteProcessContext()
        {
        }

        /// <inheritdoc />
        public SqliteProcessContext(DbContextOptions options) : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}

