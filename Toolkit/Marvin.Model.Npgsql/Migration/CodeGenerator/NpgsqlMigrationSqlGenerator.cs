using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Sql;
using System.Linq;
using Npgsql;

namespace Marvin.Model.Npgsql
{
    /// <inheritdoc />
    public class NpgsqlMigrationSqlGenerator : global::Npgsql.NpgsqlMigrationSqlGenerator
    {
        /// <inheritdoc />
        public override IEnumerable<MigrationStatement> Generate(IEnumerable<MigrationOperation> migrationOperations, string providerManifestToken)
        {
            var extendedOperations = migrationOperations.ToList();

            foreach (var migrationOperation in extendedOperations.Where(o => o is CustomSqlStatementOperation).Cast<CustomSqlStatementOperation>().ToList())
            {
                ReplaceOperation(extendedOperations, migrationOperation, new SqlOperation(migrationOperation.SqlStatement));
            }

            return base.Generate(extendedOperations, providerManifestToken);
        }

        private void ReplaceOperation(IList<MigrationOperation> migrations, MigrationOperation sourceMigrationOperation, MigrationOperation newMigrationOperation)
        {
            var idx = migrations.IndexOf(sourceMigrationOperation);
            migrations.RemoveAt(idx);
            migrations.Insert(idx, newMigrationOperation);
        }
    }
}
