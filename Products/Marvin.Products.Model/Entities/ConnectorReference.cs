using System.ComponentModel.DataAnnotations.Schema;
using Marvin.Model;

namespace Marvin.Products.Model
{
    /// <summary>
    /// There are no comments for Marvin.Products.Model.ConnectorReference in the schema.
    /// </summary>
    [Table(nameof(ConnectorReference), Schema = ProductsConstants.SchemaName)]
    public class ConnectorReference : EntityBase
    {
        /// <summary>
        /// There are no comments for Index in the schema.
        /// </summary>
        public virtual int Index { get; set; }

        /// <summary>
        /// There are no comments for Role in the schema.
        /// </summary>
        public virtual ConnectorRole Role { get; set; }

        /// <summary>
        /// There are no comments for ConnectorId in the schema.
        /// </summary>
        public virtual long? ConnectorId { get; set; }

        /// <summary>
        /// There are no comments for StepId in the schema.
        /// </summary>
        public virtual long StepId { get; set; }

        #region Navigation Properties

        /// <summary>
        /// There are no comments for Connector in the schema.
        /// </summary>
        public virtual ConnectorEntity Connector { get; set; }

        /// <summary>
        /// There are no comments for Step in the schema.
        /// </summary>
        public virtual StepEntity Step { get; set; }

        #endregion
    }
}
