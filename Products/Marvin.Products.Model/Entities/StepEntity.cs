using Marvin.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marvin.Products.Model
{
    /// <summary>
    /// There are no comments for Marvin.Products.Model.StepEntity in the schema.
    /// </summary>
    [Table(nameof(StepEntity), Schema = ProductsConstants.SchemaName)]
    public class StepEntity : ModificationTrackedEntityBase
    {
        /// <summary>
        /// There are no comments for StepId in the schema.
        /// </summary>
        public virtual long StepId { get; set; }

        /// <summary>
        /// There are no comments for Name in the schema.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// There are no comments for Assembly in the schema.
        /// </summary>
        public virtual string Assembly { get; set; }

        /// <summary>
        /// There are no comments for NameSpace in the schema.
        /// </summary>
        public virtual string NameSpace { get; set; }

        /// <summary>
        /// There are no comments for Classname in the schema.
        /// </summary>
        public virtual string Classname { get; set; }

        /// <summary>
        /// There are no comments for Parameters in the schema.
        /// </summary>
        public virtual string Parameters { get; set; }

        /// <summary>
        /// There are no comments for WorkplanId in the schema.
        /// </summary>
        public virtual long WorkplanId { get; set; }

        /// <summary>
        /// There are no comments for SubWorkplanId in the schema.
        /// </summary>
        public virtual long? SubWorkplanId { get; set; }

        #region Navigation Properties

        /// <summary>
        /// There are no comments for Workplan in the schema.
        /// </summary>
        public virtual WorkplanEntity Workplan { get; set; }

        /// <summary>
        /// There are no comments for SubWorkplan in the schema.
        /// </summary>
        public virtual WorkplanEntity SubWorkplan { get; set; }

        /// <summary>
        /// There are no comments for Connectors in the schema.
        /// </summary>
        public virtual ICollection<ConnectorReference> Connectors { get; set; }

        /// <summary>
        /// There are no comments for OutputDescriptions in the schema.
        /// </summary>
        public virtual ICollection<OutputDescriptionEntity> OutputDescriptions { get; set; }

        #endregion
    }
}
