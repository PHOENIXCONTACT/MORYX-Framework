using Microsoft.EntityFrameworkCore;
using Moryx.Model.Sqlite.Attributes;

namespace Moryx.ControlSystem.ProcessEngine.Model
{
    /// <summary>
    /// The Sqlite DbContext of this database model.
    /// </summary>
    [SqliteContext]
    public class SqliteProcessContext : ProcessContext
    {
        /// <inheritdoc />
        public SqliteProcessContext()
        {
        }

        /// <inheritdoc />
        public SqliteProcessContext(DbContextOptions options) : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
