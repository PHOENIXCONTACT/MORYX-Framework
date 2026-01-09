// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model;

// ReSharper disable UnusedMember.Global
// ReSharper disable VirtualMemberNeverOverridden.Global
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Moryx.ControlSystem.ProcessEngine.Model;

/// <summary>
/// The DBContext of this database model.
/// </summary>
public class ProcessContext : MoryxDbContext
{
    /// <inheritdoc />
    public ProcessContext()
    {
    }

    /// <inheritdoc />
    public ProcessContext(DbContextOptions options) : base(options)
    {
    }

    public virtual DbSet<ProcessEntity> Processes { get; set; }

    public virtual DbSet<ActivityEntity> Activities { get; set; }

    public virtual DbSet<TokenHolderEntity> TokenHolders { get; set; }

    public virtual DbSet<JobEntity> Jobs { get; set; }

    public virtual DbSet<TracingType> TracingTypes { get; set; }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseLazyLoadingProxies();
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // References to the ActivityEntity
        modelBuilder.Entity<ActivityEntity>()
            .HasOne(a => a.Process)
            .WithMany(p => p.Activities)
            .IsRequired();

        modelBuilder.Entity<ActivityEntity>()
            .HasOne(a => a.TracingType)
            .WithMany();

        modelBuilder.Entity<ActivityEntity>()
            .HasOne(d => d.Job)
            .WithMany();

        // References to the Job
        modelBuilder.Entity<JobEntity>()
            .HasMany(j => j.Processes)
            .WithOne(p => p.Job);

        modelBuilder.Entity<JobEntity>()
            .HasOne(j => j.Previous)
            .WithMany();

        // References to the Process
        modelBuilder.Entity<ProcessEntity>()
            .HasOne(p => p.Job)
            .WithMany(j => j.Processes);

        // References to the TokenHolder
        modelBuilder.Entity<TokenHolderEntity>()
            .HasOne(th => th.Process)
            .WithMany(p => p.TokenHolders)
            .IsRequired();
    }
}