// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model;
// ReSharper disable VirtualMemberNeverOverridden.Global

namespace Moryx.TestTools.Test.Model;

public class TestModelContext : MoryxDbContext
{
    public TestModelContext()
    {
    }

    public TestModelContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseLazyLoadingProxies();
    }

    public virtual DbSet<CarEntity> Cars { get; set; }

    public virtual DbSet<WheelEntity> Wheels { get; set; }

    public virtual DbSet<SportCarEntity> SportCars { get; set; }

    public virtual DbSet<JsonEntity> Jsons { get; set; }

    public virtual DbSet<HugePocoEntity> HugePocos { get; set; }

    public virtual DbSet<HouseEntity> Houses { get; set; }
}
