// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moryx.Model.PostgreSQL.Attributes;

namespace Moryx.Notifications.Model
{
    /// <summary>
    /// The DBContext of this database model.
    /// </summary>
    [NpgsqlDatabaseContext]
    public class NpgsqlNotificationsContext : NotificationsContext
    {
        /// <inheritdoc />
        public NpgsqlNotificationsContext()
        {
        }

        /// <inheritdoc />
        public NpgsqlNotificationsContext(DbContextOptions options) : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                var connectionString = configuration.GetConnectionString("Moryx.Notifications.Model");
                optionsBuilder.UseNpgsql(connectionString);
            }
        }
    }
}

