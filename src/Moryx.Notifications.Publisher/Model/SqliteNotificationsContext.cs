// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.Sqlite;

namespace Moryx.Notifications.Publisher.Model
{
    /// <summary>
    /// Sqlite specific implementation of <see cref="NotificationsContext"/>
    /// </summary>
    [SqliteDbContext(typeof(NotificationsContext))]
    public class SqliteNotificationsContext : NotificationsContext
    {
        /// <inheritdoc />
        public SqliteNotificationsContext()
        {
        }

        /// <inheritdoc />
        public SqliteNotificationsContext(DbContextOptions options) : base(options)
        {
        }
    }
}

