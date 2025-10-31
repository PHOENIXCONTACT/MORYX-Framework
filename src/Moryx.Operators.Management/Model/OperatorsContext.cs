// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Microsoft.EntityFrameworkCore;
using Moryx.Model;
// ReSharper disable VirtualMemberNeverOverridden.Global

namespace Moryx.Operators.Management.Model;

public class OperatorsContext : MoryxDbContext
{
    public OperatorsContext()
    {
    }

    public OperatorsContext(DbContextOptions options) : base(options)
    {
    }

    public virtual DbSet<OperatorEntity> OperatorEntities { get; set; }

    public virtual DbSet<ResourceLinkEntity> ResourceLinkEntities { get; set; }

    public virtual DbSet<SkillEntity> SkillEntities { get; set; }

    public virtual DbSet<SkillTypeEntity> SkillTypeEntities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OperatorEntity>().HasMany(o => o.AssignedResources);

        modelBuilder.Entity<SkillEntity>().HasOne(s => s.SkillType);
    }
}

