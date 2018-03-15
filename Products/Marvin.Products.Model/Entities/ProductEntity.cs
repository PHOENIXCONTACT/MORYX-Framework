using Marvin.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marvin.Products.Model
{
    /// <summary>
    /// Product entity
    /// </summary>
    [Table(nameof(ProductEntity), Schema = ProductsConstants.SchemaName)]
    public class ProductEntity : ModificationTrackedEntityBase
    {
        /// <summary>
        /// Material number
        /// </summary>
        public virtual string MaterialNumber { get; set; }
    
        /// <summary>
        /// Revision
        /// </summary>
        public virtual short Revision { get; set; }
    
        /// <summary>
        /// Type name
        /// </summary>
        public virtual string TypeName { get; set; }

        /// <summary>
        /// Current version id
        /// </summary>
        public virtual long CurrentVersionId { get; set; }

        #region Navigation Properties
    
        /// <summary>
        /// Parts
        /// </summary>
        public virtual ICollection<PartLink> Parts { get; set; }

        /// <summary>
        /// Parents
        /// </summary>
        public virtual ICollection<PartLink> Parents { get; set; }
    
        /// <summary>
        /// Recipes
        /// </summary>
        public virtual ICollection<ProductRecipeEntity> Recipes { get; set; }

        /// <summary>
        /// Old versions
        /// </summary>
        public virtual ICollection<ProductProperties> OldVersions { get; set; }

        /// <summary>
        /// Current version
        /// </summary>
        public virtual ProductProperties CurrentVersion { get; protected internal set; }
    
        /// <summary>
        /// Documents
        /// </summary>
        public virtual ICollection<ProductDocument> Documents { get; set; }

        #endregion

        /// <summary>
        /// Creates a link to the current version of this product's properties.
        /// </summary>
        /// <param name="properties"></param>
        public void SetCurrentVersion(ProductProperties properties)
        {
            if (CurrentVersion == properties)
                return;

            if (CurrentVersion != null)
                OldVersions.Add(CurrentVersion);

            CurrentVersion = properties;
        }
    }

}
