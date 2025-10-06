// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model;

namespace Moryx.Orders.Management.Model
{
    /// <summary>
    /// The DBContext of this database model.
    /// </summary>
    public class OrdersContext : MoryxDbContext
    {
        /// <inheritdoc />
        public OrdersContext()
        {
        }

        /// <inheritdoc />
        public OrdersContext(DbContextOptions options) : base(options)
        {
        }

        /// <summary>
        /// There are no comments for <see cref="OrderEntities"/> in the schema.
        /// </summary>
        public virtual DbSet<OrderEntity> OrderEntities { get; set; }

        /// <summary>
        /// There are no comments for <see cref="OperationEntities"/> in the schema.
        /// </summary>
        public virtual DbSet<OperationEntity> OperationEntities { get; set; }

        /// <summary>
        /// There are no comments for <see cref="OperationJobReferenceEntities"/> in the schema.
        /// </summary>
        public virtual DbSet<OperationJobReferenceEntity> OperationJobReferenceEntities { get; set; }

        /// <summary>
        /// There are no comments for <see cref="OperationReportEntities"/> in the schema.
        /// </summary>
        public virtual DbSet<OperationReportEntity> OperationReportEntities { get; set; }

        /// <summary>
        /// There are no comments for <see cref="OperationRecipeReferenceEntities"/> in the schema.
        /// </summary>
        public virtual DbSet<OperationRecipeReferenceEntity> OperationRecipeReferenceEntities { get; set; }

        /// <summary>
        /// There are no comments for <see cref="ProductPartEntities"/> in the schema.
        /// </summary>
        public virtual DbSet<ProductPartEntity> ProductPartEntities { get; set; }

        /// <summary>
        /// There are no comments for <see cref="OperationAdviceEntities"/> in the schema.
        /// </summary>
        public virtual DbSet<OperationAdviceEntity> OperationAdviceEntities { get; set; }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}

