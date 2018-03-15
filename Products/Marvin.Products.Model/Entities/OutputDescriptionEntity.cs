using System.ComponentModel.DataAnnotations.Schema;
using Marvin.Model;

namespace Marvin.Products.Model
{
    /// <summary>
    /// There are no comments for Marvin.Products.Model.OutputDescriptionEntity in the schema.
    /// </summary>
    [Table(nameof(OutputDescriptionEntity), Schema = ProductsConstants.SchemaName)]
    public class OutputDescriptionEntity : EntityBase
    {
        /// <summary>
        /// There are no comments for Index in the schema.
        /// </summary>
        public virtual int Index { get; set; }

        /// <summary>
        /// There are no comments for Success in the schema.
        /// </summary>
        public virtual bool Success { get; set; }

        /// <summary>
        /// There are no comments for Name in the schema.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// There are no comments for MappingValue in the schema.
        /// </summary>
        public virtual long MappingValue { get; set; }

        /// <summary>
        /// There are no comments for StepEntityId in the schema.
        /// </summary>
        public virtual long StepEntityId { get; set; }

        #region Navigation Properties

        /// <summary>
        /// There are no comments for Step in the schema.
        /// </summary>
        public virtual StepEntity Step { get; set; }

        #endregion
    }
}
