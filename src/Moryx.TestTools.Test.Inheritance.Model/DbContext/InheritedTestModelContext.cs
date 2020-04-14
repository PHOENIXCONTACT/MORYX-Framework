// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Data.Common;
using System.Data.Entity;
using Moryx.Model;
using Moryx.TestTools.Test.Model;

namespace Moryx.TestTools.Test.Inheritance.Model
{
    public class InheritedTestModelContext : TestModelContext
    {
        public InheritedTestModelContext()
        {
        }

        public InheritedTestModelContext(string connectionString) : base(connectionString)
        {
        }

        public InheritedTestModelContext(DbConnection connection) : base(connection)
        {
        }

        public virtual DbSet<SuperCarEntity> SuperCars { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SuperCarEntity>()
                .ToTable(nameof(SuperCarEntity));
        }
    }
}
