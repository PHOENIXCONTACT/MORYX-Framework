using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Model;

namespace Marvin.Model
{
    /// <summary>
    /// Base configuration for code first migration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MarvinDbMigrationsConfiguration<T> : DbMigrationsConfiguration<T> where T : DbContext
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MarvinDbMigrationsConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            MigrationsDirectory = @"Migrations\Generated";

            CodeGenerator = new MarvinCSharpMigrationCodeGenerator<T>();
        }

        /// <summary>
        /// Property to the Marvin based code generator
        /// </summary>
        protected MarvinCSharpMigrationCodeGenerator<T> MarvinCodeGenerator => CodeGenerator as MarvinCSharpMigrationCodeGenerator<T>;

        /// <summary>
        /// Add an additional migration to the beginning of the migration
        /// </summary>
        /// <param name="migrationOperation"></param>
        public void AddAdditionalMigrationToTheBeginning(MigrationOperation migrationOperation)
        {
            MarvinCodeGenerator?.AddAdditionalMigrationToTheBeginning(migrationOperation);
        }

        /// <summary>
        /// Add an additional migration to the end of the migration
        /// </summary>
        /// <param name="migrationOperation"></param>
        public void AddAdditionalMigrationToTheEnd(MigrationOperation migrationOperation)
        {
            MarvinCodeGenerator?.AddAdditionalMigrationToTheEnd(migrationOperation);
        }
    }
}
