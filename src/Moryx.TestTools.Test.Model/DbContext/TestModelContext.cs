// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Data.Common;
using System.Data.Entity;
using Marvin.Model;
using Marvin.Model.PostgreSQL;

namespace Marvin.TestTools.Test.Model
{
    /// <summary>
    /// The DBContext of this database model.
    /// </summary>
    [DbConfigurationType(typeof(NpgsqlConfiguration))]
    [DefaultSchema(TestModelConstants.Schema)]
    public class TestModelContext : MarvinDbContext
    {
        public TestModelContext()
        {
        }

        public TestModelContext(string connectionString, ContextMode mode) : base(connectionString, mode)
        {
        }

        public TestModelContext(DbConnection connection, ContextMode mode) : base(connection, mode)
        {
        }

        public virtual DbSet<CarEntity> Cars { get; set; }

        public virtual DbSet<WheelEntity> Wheels { get; set; }

        public virtual DbSet<SportCarEntity> SportCars { get; set; }

        public virtual DbSet<JsonEntity> Jsons { get; set; }
        
        public virtual DbSet<HugePocoEntity> HugePocos { get; set; }

        public virtual DbSet<HouseEntity> Houses { get; set; }
    }
}
