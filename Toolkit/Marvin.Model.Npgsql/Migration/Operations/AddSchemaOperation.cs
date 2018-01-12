using System.Data.Entity.Migrations.Model;

namespace Marvin.Model.Npgsql
{
    /// <summary>
    /// Adds a new schema
    /// </summary>
    public class AddSchemaOperation : CustomSqlStatementOperation
    {
        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        public AddSchemaOperation(string schema)
        {
            SchemaName = schema;
        }

        /// <summary>
        /// Name of the target schema
        /// </summary>
        public string SchemaName { get; set; }

        /// <inheritdoc />
        public override string CSharpCode => $"((IDbMigration)this).AddOperation(new {GetType().Name}(\"{SchemaName}\"));";

        /// <inheritdoc />
        public override string SqlStatement => $"CREATE SCHEMA IF NOT EXISTS {SchemaName};";

        /// <inheritdoc />
        public override MigrationOperation Inverse => new RemoveSchemaOperation(SchemaName);
    }
}
