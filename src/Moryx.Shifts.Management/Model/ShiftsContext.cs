// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Microsoft.EntityFrameworkCore;
using Moryx.Model;

namespace Moryx.Shifts.Management.Model
{
    public class ShiftsContext : MoryxDbContext
    {
        public ShiftsContext()
        {
        }

        public ShiftsContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<ShiftEntity> ShiftEntities { get; set; }
        public virtual DbSet<ShiftTypeEntity> ShiftTypeEntities { get; set; }
        public virtual DbSet<ShiftAssignementEntity> ShiftAssignementEntities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ShiftEntity>().HasOne(s => s.ShiftType);

            modelBuilder.Entity<ShiftAssignementEntity>().HasOne(s => s.Shift);
        }
    }
}

