using Microsoft.EntityFrameworkCore;
using Moryx.Model.Sqlite.Attributes;

namespace Moryx.Orders.Management.Model
{
    /// <summary>
    /// The DBContext of this database model.
    /// </summary>
    [SqliteContext]
    public class SqliteOrdersContext : OrdersContext
    {
        /// <inheritdoc />
        public SqliteOrdersContext()
        {
        }

        /// <inheritdoc />
        public SqliteOrdersContext(DbContextOptions options) : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
