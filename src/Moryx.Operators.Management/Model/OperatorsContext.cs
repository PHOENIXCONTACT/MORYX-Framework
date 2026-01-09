// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model;

// ReSharper disable UnusedMember.Global
// ReSharper disable VirtualMemberNeverOverridden.Global
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Moryx.Operators.Management.Model;

/// <summary>
/// The DBContext of this database model.
/// </summary>
public class OperatorsContext : MoryxDbContext
{
    /// <inheritdoc />
    public OperatorsContext()
    {
    }

    /// <inheritdoc />
    public OperatorsContext(DbContextOptions options) : base(options)
    {
    }

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

        modelBuilder.Entity<OperatorEntity>()
            .HasMany(o => o.AssignedResources);

        modelBuilder.Entity<SkillEntity>()
            .HasOne(s => s.SkillType);
    }

    public virtual DbSet<OperatorEntity> Operators { get; set; }

    public virtual DbSet<ResourceLinkEntity> ResourceLinks { get; set; }

    public virtual DbSet<SkillEntity> Skills { get; set; }

    public virtual DbSet<SkillTypeEntity> SkillTypes { get; set; }
}
