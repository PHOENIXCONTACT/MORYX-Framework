// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.Sqlite;
// ReSharper disable VirtualMemberNeverOverridden.Global

namespace Moryx.Notifications.Model
{
    /// <summary>
    /// The DBContext of this database model.
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

