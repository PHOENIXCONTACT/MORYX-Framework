using System;

namespace Marvin.Model
{
    /// <summary>
    /// Defines the default schmea for this database context
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DefaultSchemaAttribute : Attribute
    {
        /// <summary>
        /// Name of the default schema
        /// </summary>
        public string Schema { get; }

        /// <inheritdoc />
        public DefaultSchemaAttribute(string schema)
        {
            Schema = schema;
        }
    }
}
