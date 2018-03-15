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
            MigrationsDirectory = @"Migrations\Generated";

            SetHistoryContextFactory("Npgsql", (connection, defaultSchema) => new MarvinHistoryContext(connection, defaultSchema));

            CodeGenerator = new NpqsqlCSharpMigrationCodeGenerator<T>();
            SetSqlGenerator("Npgsql", new NpgsqlMigrationSqlGenerator());
        }
    }
}
