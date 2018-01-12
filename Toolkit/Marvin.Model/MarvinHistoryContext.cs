using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Migrations.History;

namespace Marvin.Model
{
    /// <summary>
    /// History context for Marvin
    /// Table name: __MigrationHistory
    /// Schema: public
    /// </summary>
    public class MarvinHistoryContext : HistoryContext
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="existingConnection"></param>
        /// <param name="defaultSchema"></param>
        public MarvinHistoryContext(DbConnection existingConnection, string defaultSchema)
            : base(existingConnection, defaultSchema)
        {

        }

        /// <summary>
        /// Called when the model is creating configures the history table
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Change schema from "dbo" to "public"
            modelBuilder.Entity<HistoryRow>()
                .ToTable(tableName: "__MigrationHistory",
                    schemaName: "public");
        }
    }
}
