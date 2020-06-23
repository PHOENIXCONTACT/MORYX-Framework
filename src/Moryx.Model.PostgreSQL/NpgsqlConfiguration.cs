// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Data.Entity;
using Npgsql;

namespace Moryx.Model.PostgreSQL
{
    /// <summary>
    /// Registers Npgsql as EntityFramework provider.
    /// This class has to be referenced by Moryx database models only.
    /// </summary>
    public class NpgsqlConfiguration : DbConfiguration
    {
        /// <summary>
        /// This constructor is called directly from EntityFramework.
        /// </summary>
        public NpgsqlConfiguration()
        {
            SetProviderServices("Npgsql", NpgsqlServices.Instance);
            SetProviderFactory("Npgsql", NpgsqlFactory.Instance);
            SetDefaultConnectionFactory(new NpgsqlConnectionFactory());
        }
    }
}
