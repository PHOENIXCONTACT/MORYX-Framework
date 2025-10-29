// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model;

namespace Moryx.ControlSystem.ProcessEngine.Model
{
    /// <summary>
    /// The DBContext of this database model.
    /// </summary>
    public abstract class ProcessContext : MoryxDbContext
    {
        /// <inheritdoc />
        protected ProcessContext()
        {
        }

        /// <inheritdoc />
        protected ProcessContext(DbContextOptions options) : base(options)
        {
        }

        /// <summary>
        /// There are no comments for <see cref="ProcessEntities"/> in the schema.
        /// </summary>
        public virtual DbSet<ProcessEntity> ProcessEntities { get; set; }

        /// <summary>
        /// There are no comments for <see cref="ActivityEntities"/> in the schema.
        /// </summary>
        public virtual DbSet<ActivityEntity> ActivityEntities { get; set; }

        /// <summary>
        /// There are no comments for <see cref="TokenHolderEntities"/> in the schema.
        /// </summary>
        public virtual DbSet<TokenHolderEntity> TokenHolderEntities { get; set; }

        /// <summary>
        /// There are no comments for <see cref="JobEntities"/> in the schema.
        /// </summary>
        public virtual DbSet<JobEntity> JobEntities { get; set; }

        /// <summary>
        /// There are no comments for <see cref="TracingTypes"/> in the schema.
        /// </summary>
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
}

