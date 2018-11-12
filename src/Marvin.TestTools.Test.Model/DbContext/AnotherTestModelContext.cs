﻿using System.Data.Common;
using System.Data.Entity;
using Marvin.Model;
using Marvin.Model.Npgsql;

namespace Marvin.TestTools.Test.Model
{
    /// <summary>
    /// The DBContext of this database model.
    /// </summary>
    [DbConfigurationType(typeof(NpgsqlConfiguration))]
    [DefaultSchema(AnotherTestModelConstants.Schema)]
    public class AnotherTestModelContext : MarvinDbContext
    {
        public AnotherTestModelContext()
        {
        }

        public AnotherTestModelContext(string connectionString, ContextMode mode) : base(connectionString, mode)
        {
        }

        public AnotherTestModelContext(DbConnection connection, ContextMode mode) : base(connection, mode)
        {
        }

        public virtual DbSet<AnotherEntity> Others { get; set; }
    }
}