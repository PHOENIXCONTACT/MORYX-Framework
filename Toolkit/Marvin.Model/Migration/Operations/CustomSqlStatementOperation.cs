using System.Data.Entity.Migrations.Model;

namespace Marvin.Model
{
    /// <summary>
    /// A custom migration operation to set execute sql statements
    /// </summary>
    public abstract class CustomSqlStatementOperation : SqlOperation
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected CustomSqlStatementOperation() : base("FROM FOO SELECT *")
        {
        }

        /// <summary>
        /// Returns the sql statement
        /// </summary>
        public abstract string SqlStatement { get; }

        /// <inheritdoc />
        public override MigrationOperation Inverse => null;

        /// <inheritdoc />
        public override bool IsDestructiveChange => false;

        /// <summary>
        /// Called that is written by the csharp code generator
        /// </summary>
        public virtual string CSharpCode => $"((IDbMigration)this).AddOperation(new { GetType().Name }());";
    }
}
