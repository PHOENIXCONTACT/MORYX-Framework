using System;
using System.Data.Entity.Migrations.Model;

namespace Marvin.Model
{
    /// <summary>
    /// Describes when the migration operation should be executed
    /// </summary>
    public enum MigrationOperationExecutionPosition
    {
        /// <summary>
        /// Execute at the beginning of the migration
        /// </summary>
        Begin,

        /// <summary>
        /// Execute at the end of the migration
        /// </summary>
        End
    }

    /// <summary>
    /// Base class for custom annotations
    /// </summary>
    public abstract class CustomMigrationOperationAttribute : Attribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected CustomMigrationOperationAttribute(MigrationOperationExecutionPosition executionPosition)
        {
            ExecutionPosition = executionPosition;
        }

        /// <summary>
        /// Defines when the operation should be executed while migration
        /// </summary>
        public MigrationOperationExecutionPosition ExecutionPosition { get; }

        /// <summary>
        /// Returns the custom <see cref="MigrationOperation"/>
        /// </summary>
        public abstract MigrationOperation CustomMigrationOperation();
    }
}
