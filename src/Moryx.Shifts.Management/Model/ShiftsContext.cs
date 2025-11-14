// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model;

// ReSharper disable UnusedMember.Global
// ReSharper disable VirtualMemberNeverOverridden.Global
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Moryx.Shifts.Management.Model
{
    /// <summary>
    /// The DBContext of this database model.
    /// </summary>
    public class ShiftsContext : MoryxDbContext
    {
        /// <inheritdoc />
        public ShiftsContext()
        {
        }

        /// <inheritdoc />
        public ShiftsContext(DbContextOptions options) : base(options)
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

            modelBuilder.Entity<ShiftEntity>().HasOne(s => s.ShiftType);

            modelBuilder.Entity<ShiftAssignementEntity>().HasOne(s => s.Shift);
        }

        public virtual DbSet<ShiftEntity> Shifts { get; set; }

        public virtual DbSet<ShiftTypeEntity> ShiftTypes { get; set; }

        public virtual DbSet<ShiftAssignementEntity> ShiftAssignements { get; set; }
    }
}

