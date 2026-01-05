// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Maintenance.Model.Entities;
using Moryx.Model;

namespace Moryx.Maintenance.Model;

public class MaintenanceContext : MoryxDbContext
{
    public MaintenanceContext()
    {
    }

    public MaintenanceContext(DbContextOptions options) : base(options)
    {
    }

    public virtual DbSet<MaintenanceOrderEntity> MaintenanceOrders { get; set; }

    public virtual DbSet<AcknowledgementEntity> Acknowledgements { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MaintenanceOrderEntity>()
            .HasMany(a => a.Acknowledgements)
            .WithOne();

        modelBuilder.Entity<MaintenanceOrderEntity>()
            .OwnsMany(x => x.Instructions, pb =>
            {
                pb.WithOwner().HasForeignKey("MaintenanceOrderId");
                pb.Property<long>("Id");
                pb.HasKey("Id");
            });
        modelBuilder.Entity<MaintenanceOrderEntity>()
            // Hide deleted maintenance order
            .HasQueryFilter(b => !b.Deleted.HasValue);
    }

}
