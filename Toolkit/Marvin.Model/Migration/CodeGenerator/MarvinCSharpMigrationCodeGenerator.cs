using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations.Design;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Utilities;
using System.Linq;
using System.Reflection;

namespace Marvin.Model
{
    /// <summary>
    /// Customized CSharp migration code generator for marvin
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class MarvinCSharpMigrationCodeGenerator<TContext> :  CSharpMigrationCodeGenerator where TContext : DbContext
    {
        private IList<MigrationOperation> _additionalMigrationOperationsBegin = new List<MigrationOperation>();
        private IList<MigrationOperation> _additionalMigrationOperationsEnd = new List<MigrationOperation>();

        /// <summary>
        /// Add an additional migration to the beginning of the migration
        /// </summary>
        /// <param name="migrationOperation"></param>
        public void AddAdditionalMigrationToTheBeginning(MigrationOperation migrationOperation)
        {
            _additionalMigrationOperationsBegin.Add(migrationOperation);
        }

        /// <summary>
        /// Add an additional migration to the beginning of the migration
        /// </summary>
        /// <param name="migrationOperation"></param>
        public void AddAdditionalMigrationToTheEnd(MigrationOperation migrationOperation)
        {
            _additionalMigrationOperationsEnd.Add(migrationOperation);
        }

        /// <inheritdoc />
        public override ScaffoldedMigration Generate(string migrationId, IEnumerable<MigrationOperation> operations, string sourceModel, string targetModel,
            string @namespace, string className)
        {
            var extendedOperations = operations.ToList();

            CustomMigrationOperations(extendedOperations, ref _additionalMigrationOperationsBegin, ref _additionalMigrationOperationsEnd);

            extendedOperations.InsertRange(0, _additionalMigrationOperationsBegin);
            extendedOperations.AddRange(_additionalMigrationOperationsEnd);

            return base.Generate(migrationId, extendedOperations, sourceModel, targetModel, @namespace, className);
        }

        /// <inheritdoc />
        protected override IEnumerable<string> GetNamespaces(IEnumerable<MigrationOperation> operations)
        {
            var namespaces = base.GetNamespaces(operations).ToList();
            namespaces.AddRange(operations.Select(migrationOperation => migrationOperation.GetType().Namespace));
            namespaces.Add("System.Data.Entity.Migrations.Infrastructure");

            return namespaces.Distinct();
        }

        /// <inheritdoc />
        protected override void Generate(SqlOperation sqlOperation, IndentedTextWriter writer)
        {
            var customSqlStatementOperation = sqlOperation as CustomSqlStatementOperation;
            if (customSqlStatementOperation != null)
            {
                Generate(customSqlStatementOperation, writer);
                return;
            }

            base.Generate(sqlOperation, writer);
        }

        /// <summary>
        /// Generator function for <see cref="CustomSqlStatementOperation"/>
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="writer"></param>
        protected virtual void Generate(CustomSqlStatementOperation operation, IndentedTextWriter writer)
        {
            writer.WriteLine(operation.CSharpCode);
            writer.WriteLine("");
        }

        /// <summary>
        /// Gets all operations
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<MigrationOperation> AdditionalOperations()
        {
            return _additionalMigrationOperationsBegin.Concat(_additionalMigrationOperationsEnd);
        }

        private void CustomMigrationOperations(ICollection<MigrationOperation> operations, ref IList<MigrationOperation> migrationOperationsBegin, ref IList<MigrationOperation> migrationOperationsEnd)
        {
            var contextType = typeof(TContext);
            var dbSetEntityTypes = contextType.GetProperties().Where(p => p.PropertyType.Name == typeof(DbSet<string>).Name).Select(p => p.PropertyType.GetGenericArguments().First());

            foreach (var dbSetEntityType in dbSetEntityTypes)
            {
                var customAttributes = dbSetEntityType.GetCustomAttributes(true);

                OnLookupForCustomMigrationOperations(operations, customAttributes, dbSetEntityType);

                foreach (var customMigrationOperationAttribute in customAttributes.Where(a => a is CustomMigrationOperationAttribute).Cast<CustomMigrationOperationAttribute>())
                {
                    switch (customMigrationOperationAttribute.ExecutionPosition)
                    {
                        case MigrationOperationExecutionPosition.Begin:
                            migrationOperationsBegin.Add(customMigrationOperationAttribute.CustomMigrationOperation());
                            break;
                        case MigrationOperationExecutionPosition.End:
                            migrationOperationsEnd.Add(customMigrationOperationAttribute.CustomMigrationOperation());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        /// <summary>
        /// Is called when the custom migration operations are looked up
        /// </summary>
        /// <param name="operations"></param>
        /// <param name="customAttributes"></param>
        /// <param name="dbSetEntityType"></param>
        protected virtual void OnLookupForCustomMigrationOperations(ICollection<MigrationOperation> operations,
            object[] customAttributes, Type dbSetEntityType)
        {
            
        }
    }
}