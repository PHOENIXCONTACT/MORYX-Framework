// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model;

// ReSharper disable UnusedMember.Global
// ReSharper disable VirtualMemberNeverOverridden.Global
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

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

        public virtual DbSet<OrderEntity> Orders { get; set; }

        public virtual DbSet<OperationEntity> Operations { get; set; }

        public virtual DbSet<OperationJobReferenceEntity> OperationJobReferences { get; set; }

        public virtual DbSet<OperationReportEntity> OperationReports { get; set; }

        public virtual DbSet<OperationRecipeReferenceEntity> OperationRecipeReferences { get; set; }

        public virtual DbSet<ProductPartEntity> ProductParts { get; set; }

        public virtual DbSet<OperationAdviceEntity> OperationAdvices { get; set; }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}

