// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Moryx.Identity.AccessManagement.Data
{
    /// <summary>
    /// Entity Framework database context using <see cref="MoryxUser"/>s, <see cref="MoryxRole"/>s and a string key.
    /// </summary>
    public class MoryxIdentitiesDbContext : IdentityDbContext<MoryxUser, MoryxRole, string>
    {
        /// <inheritdoc/>
        public MoryxIdentitiesDbContext(DbContextOptions<MoryxIdentitiesDbContext> options)
            : base(options)
        {
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            modelBuilder.Entity<Permission>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<MoryxRole>()
                .HasMany(e => e.Permissions)
                .WithMany(e => e.Roles)
                .UsingEntity<PermissionRole>(
                    pr => pr
                        .HasOne(pr => pr.Permission)
                        .WithMany()
                        .HasForeignKey(pr => pr.PermissionId),
                    pr => pr
                        .HasOne(pr => pr.Role)
                        .WithMany()
                        .HasForeignKey(pr => pr.RoleId))
                .HasKey(pr => new { pr.RoleId, pr.PermissionId });
        }

        /// <summary>
        /// Set of permissions managed in the AccessManagement.
        /// </summary>
        public DbSet<Permission> Permissions { get; set; }

        /// <summary>
        /// Set of roles that a permission is assigned to.
        /// </summary>
        public DbSet<PermissionRole> PermissionRoles { get; set; }

        /// <summary>
        /// Set of refresh tokens that are currently in use.
        /// </summary>
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<PasswordReset> PasswordResets { get; set; }
    }
}

