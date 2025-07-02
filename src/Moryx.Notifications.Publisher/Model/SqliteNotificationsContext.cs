using Microsoft.EntityFrameworkCore;
using Moryx.Model.Sqlite.Attributes;

namespace Moryx.Notifications.Model
{
    /// <summary>
    /// The DBContext of this database model.
    /// </summary>
    [SqliteContext]
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

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
