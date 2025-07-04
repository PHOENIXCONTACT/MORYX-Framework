﻿using Microsoft.EntityFrameworkCore;
using Moryx.Model;

namespace Moryx.Notifications.Model
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
        public virtual DbSet<NotificationEntity> NotificationEntities { get; set; }

        /// <summary>
        /// There are no comments for NotificationTypeEntity in the schema.
        /// </summary>
        public virtual DbSet<NotificationTypeEntity> NotificationTypeEntities { get; set; }
    }
}
