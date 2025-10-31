// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.PostgreSQL;

namespace Moryx.Notifications.Model
{
    /// <summary>
    /// The DBContext of this database model.
    /// </summary>
    [NpgsqlDbContext(typeof(NotificationsContext))]
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
    }
}

