using Marvin.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marvin.Products.Model
{
    /// <summary>
    /// There are no comments for Marvin.Products.Model.WorkplanEntity in the schema.
    /// </summary>
    [Table(nameof(WorkplanEntity), Schema = ProductsConstants.SchemaName)]
    public class WorkplanEntity : ModificationTrackedEntityBase
    {
        /// <summary>
        /// There are no comments for Name in the schema.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// There are no comments for Version in the schema.
        /// </summary>
        public virtual int Version { get; set; }

        /// <summary>
        /// There are no comments for State in the schema.
        /// </summary>
        public virtual int State { get; set; }

        /// <summary>
        /// There are no comments for MaxElementId in the schema.
        /// </summary>
        public virtual int MaxElementId { get; set; }

        #region Navigation Properties

        /// <summary>
        /// There are no comments for Recipes in the schema.
        /// </summary>
        public virtual ICollection<ProductRecipeEntity> Recipes { get; set; }

        /// <summary>
        /// There are no comments for SourceReferences in the schema.
        /// </summary>
        public virtual ICollection<WorkplanReference> SourceReferences { get; set; }

        /// <summary>
        /// There are no comments for TargetReferences in the schema.
        /// </summary>
        public virtual ICollection<WorkplanReference> TargetReferences { get; set; }

        /// <summary>
        /// There are no comments for Connectors in the schema.
        /// </summary>
        public virtual ICollection<ConnectorEntity> Connectors { get; set; }

        /// <summary>
        /// There are no comments for Steps in the schema.
        /// </summary>
        public virtual ICollection<StepEntity> Steps { get; set; }

        /// <summary>
        /// There are no comments for Parents in the schema.
        /// </summary>
        public virtual ICollection<StepEntity> Parents { get; set; }

        #endregion
    }
}
