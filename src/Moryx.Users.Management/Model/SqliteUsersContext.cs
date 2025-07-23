using Microsoft.EntityFrameworkCore;
using Moryx.Model.Sqlite.Attributes;

namespace Moryx.Users.Management.Model
{
    /// <inheritdoc />
    [SqliteContext]
    public class SqliteUsersContext : UsersContext
    {
        /// <inheritdoc />
        public SqliteUsersContext()
        {
        }

        /// <inheritdoc />
        public SqliteUsersContext(DbContextOptions options) : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
