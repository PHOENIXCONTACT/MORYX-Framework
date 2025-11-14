// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model;

// ReSharper disable UnusedMember.Global
// ReSharper disable VirtualMemberNeverOverridden.Global
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Moryx.Notifications.Publisher.Model
{
    /// <summary>
    /// The DBContext of this database model.
    /// </summary>
    public class NotificationsContext : MoryxDbContext
    {
        /// <inheritdoc />
        public NotificationsContext()
        {
        }

        /// <inheritdoc />
        public NotificationsContext(DbContextOptions options) : base(options)
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

            // References to the NotificationEntity
            modelBuilder.Entity<NotificationEntity>()
                .HasOne(n => n.Type)
                .WithMany()
                .IsRequired();
        }

        /// <summary>
        /// There are no comments for NotificationEntity in the schema.
        /// </summary>
        public virtual DbSet<NotificationEntity> Notifications { get; set; }

        /// <summary>
        /// There are no comments for NotificationTypeEntity in the schema.
        /// </summary>
        public virtual DbSet<NotificationTypeEntity> NotificationTypes { get; set; }
    }
}
