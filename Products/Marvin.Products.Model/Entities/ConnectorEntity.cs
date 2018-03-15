using Marvin.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marvin.Products.Model
{
    /// <summary>
    /// There are no comments for Marvin.Products.Model.ConnectorEntity in the schema.
    /// </summary>
    [Table(nameof(ConnectorEntity), Schema = ProductsConstants.SchemaName)]
    public class ConnectorEntity : EntityBase
    {
        /// <summary>
        /// There are no comments for ConnectorId in the schema.
        /// </summary>
        public virtual long ConnectorId { get; set; }

        /// <summary>
        /// There are no comments for Name in the schema.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// There are no comments for Classification in the schema.
        /// </summary>
        public virtual int Classification { get; set; }

        /// <summary>
        /// There are no comments for WorkplanId in the schema.
        /// </summary>
        public virtual long WorkplanId { get; set; }

        #region Navigation Properties

        /// <summary>
        /// There are no comments for Workplan in the schema.
        /// </summary>
        public virtual WorkplanEntity Workplan { get; set; }

        /// <summary>
        /// There are no comments for Usages in the schema.
        /// </summary>
        public virtual ICollection<ConnectorReference> Usages { get; set; }

        #endregion
    }

}
