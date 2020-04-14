// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Data.Common;
using System.Data.Entity;
using Moryx.Model;
using Moryx.Model.PostgreSQL;

namespace Moryx.TestTools.Test.Model
{
    /// <summary>
    /// The DBContext of this database model.
    /// </summary>
    [DbConfigurationType(typeof(NpgsqlConfiguration))]
    [DefaultSchema(AnotherTestModelConstants.Schema)]
    public class AnotherTestModelContext : MoryxDbContext
    {
        public AnotherTestModelContext()
        {
        }

        public AnotherTestModelContext(string connectionString) : base(connectionString)
        {
        }

        public AnotherTestModelContext(DbConnection connection) : base(connection)
        {
        }

        public virtual DbSet<AnotherEntity> Others { get; set; }
    }
}
