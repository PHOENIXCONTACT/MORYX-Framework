// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Data.Common;
using System.Data.Entity;
using Marvin.Model;
using Marvin.Model.Npgsql;
using Marvin.Products.Model;

namespace Marvin.Products.Samples.Model
{
    /// <summary>
    /// The DBContext of this database model.
    /// </summary>
    [DbConfigurationType(typeof(NpgsqlConfiguration))]
    [DefaultSchema(ProductsConstants.Schema)]
    public class WatchProductsContext : ProductsContext
    {
        /// <inheritdoc />
        public WatchProductsContext()
        { 
        }

        /// <inheritdoc />
        public WatchProductsContext(string connectionString, ContextMode mode) : base(connectionString, mode)
        {           
        }

        /// <inheritdoc />
        public WatchProductsContext(DbConnection connection, ContextMode mode) : base(connection, mode)
        {
        }

        /// <summary>
        /// There are no comments for SmartWatchProductPropertiesEntities in the schema.
        /// </summary>
        public virtual DbSet<SmartWatchProductPropertiesEntity> SmartWatchProductPropertiesEntities { get; set; }

        /// <summary>
        /// There are no comments for AnalogWatchProductPropertiesEntities in the schema.
        /// </summary>
        public virtual DbSet<AnalogWatchProductPropertiesEntity> AnalogWatchProductPropertiesEntities { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SmartWatchProductPropertiesEntity>()
                .ToTable(nameof(SmartWatchProductPropertiesEntity), ProductsConstants.Schema);

            modelBuilder.Entity<AnalogWatchProductPropertiesEntity>()
                .ToTable(nameof(AnalogWatchProductPropertiesEntity), ProductsConstants.Schema);
        }
    }
}
