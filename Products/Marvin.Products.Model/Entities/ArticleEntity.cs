using System;
using Marvin.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marvin.Products.Model
{
    /// <summary>
    /// There are no comments for Marvin.Products.Model.ArticleEntity in the schema.
    /// </summary>
    [Table(nameof(ArticleEntity), Schema = ProductsConstants.SchemaName)]
    public class ArticleEntity : EntityBase
    {
        /// <summary>
        /// There are no comments for State in the schema.
        /// </summary>
        public virtual long State { get; set; }
    
        /// <summary>
        /// There are no comments for ProductionDate in the schema.
        /// </summary>
        public virtual DateTime ProductionDate { get; set; }
    
        /// <summary>
        /// There are no comments for Identifier in the schema.
        /// </summary>
        public virtual string Identifier { get; set; }
    
        /// <summary>
        /// There are no comments for NumberType in the schema.
        /// </summary>
        public virtual int NumberType { get; set; }
    
        /// <summary>
        /// There are no comments for ProductId in the schema.
        /// </summary>
        public virtual long ProductId { get; set; }
    
        /// <summary>
        /// There are no comments for ParentId in the schema.
        /// </summary>
        public virtual long? ParentId { get; set; }
    
        /// <summary>
        /// There are no comments for ExtensionData in the schema.
        /// </summary>
        public virtual string ExtensionData { get; set; }
    
        /// <summary>
        /// There are no comments for PartLinkId in the schema.
        /// </summary>
        public virtual long? PartLinkId { get; set; }

        #region Navigation Properties
    
        /// <summary>
        /// There are no comments for Product in the schema.
        /// </summary>
        public virtual ProductEntity Product { get; set; }
    
        /// <summary>
        /// There are no comments for Parts in the schema.
        /// </summary>
        public virtual ICollection<ArticleEntity> Parts { get; set; }
    
        /// <summary>
        /// There are no comments for Parent in the schema.
        /// </summary>
        public virtual ArticleEntity Parent { get; set; }
    
        /// <summary>
        /// There are no comments for PartLink in the schema.
        /// </summary>
        public virtual PartLink PartLink { get; set; }

        #endregion
    }
}
