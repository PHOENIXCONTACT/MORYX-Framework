using System.Data.Entity;

namespace Marvin.Model.Npgsql
{
    /// <summary>
    /// Base migration configuration for Npgsql
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NpgsqlDbMigrationConfigurationBase<T> : MarvinDbMigrationsConfiguration<T> where T : DbContext
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public NpgsqlDbMigrationConfigurationBase()
        {
            AutomaticMigrationsEnabled = true;
            CodeGenerator = new NpqsqlCSharpMigrationCodeGenerator<T>();

            SetHistoryContextFactory("Npgsql", (connection, defaultSchema) => new MarvinHistoryContext(connection, defaultSchema));
            SetSqlGenerator("Npgsql", new NpgsqlMigrationSqlGenerator());
        }
    }
}