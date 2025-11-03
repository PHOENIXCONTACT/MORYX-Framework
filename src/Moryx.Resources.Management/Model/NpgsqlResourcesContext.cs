// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.PostgreSQL;

namespace Moryx.Resources.Management.Model
{
    /// <summary>
    /// Npgsql specific implementation of <see cref="ResourcesContext"/>
    /// </summary>
    [NpgsqlDbContext(typeof(ResourcesContext))]
    public class NpgsqlResourcesContext : ResourcesContext
    {
        /// <inheritdoc />
        public NpgsqlResourcesContext()
        {
        }

        /// <inheritdoc />
        public NpgsqlResourcesContext(DbContextOptions options) : base(options)
        {
        }
    }
}
