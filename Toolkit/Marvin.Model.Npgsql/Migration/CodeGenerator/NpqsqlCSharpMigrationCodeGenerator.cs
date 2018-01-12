using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Reflection;

namespace Marvin.Model.Npgsql
{
    /// <inheritdoc />
    public class NpqsqlCSharpMigrationCodeGenerator<TContext> : MarvinCSharpMigrationCodeGenerator<TContext> where TContext : DbContext
    {
        /// <inheritdoc />
        protected override void OnLookupForCustomMigrationOperations(ICollection<MigrationOperation> operations, object[] customAttributes, Type dbSetEntityType)
        {
            base.OnLookupForCustomMigrationOperations(operations, customAttributes, dbSetEntityType);

            LookForSchemaGeneration(operations, customAttributes);
        }

        private void LookForSchemaGeneration(ICollection<MigrationOperation> operations, object[] customAttributes)
        {
            var contextType = typeof(TContext);
            var tableAttribute = customAttributes.FirstOrDefault() as TableAttribute;
            
            var defaultSchemaAttr = contextType.GetCustomAttribute<DefaultSchemaAttribute>(true);

            var schema = tableAttribute?.Schema;

            if (string.IsNullOrEmpty(schema))
            {
                schema = defaultSchemaAttr?.Schema;
            }

            if (!string.IsNullOrEmpty(schema))
            {
                var existsSchemaCreationOperation = operations.Concat(AdditionalOperations()).Where(o => o is AddSchemaOperation).Cast<AddSchemaOperation>().FirstOrDefault(o => o.SchemaName == schema) != null;

                var createSchema = !existsSchemaCreationOperation ||
                                   operations.Concat(AdditionalOperations()).Where(o => o is MoveTableOperation).Cast<MoveTableOperation>().FirstOrDefault(o => o.NewSchema == schema) != null;

                if (createSchema)
                {
                    AddAdditionalMigrationToTheBeginning(new AddSchemaOperation(schema));
                }
            }
        }
    }
}
