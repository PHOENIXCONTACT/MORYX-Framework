using System.Runtime.Serialization;
using Marvin.AbstractionLayer.Identity;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Base class for all products
    /// </summary>
    public abstract class ProductType : IProductType
    {
        /// <inheritdoc />
        public abstract string Type { get; }

        /// <inheritdoc />
        public long Id { get; set; }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public ProductTypeState State { get; set; }

        /// <inheritdoc />
        public IProductType Parent => ParentLink?.Parent;

        /// <inheritdoc />
        public IProductPartLink ParentLink { get; set; }

        /// <inheritdoc />
        public IIdentity Identity { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Identity.ToString();
        }

        /// <summary>
        /// Create article instance of this type
        /// </summary>
        /// <returns></returns>
        public ProductInstance CreateInstance()
        {
            var article = Instantiate();
            article.ProductType = this;
            return article;
        }

        /// <summary>
        /// Instantiate this product
        /// </summary>
        /// <returns>New instance</returns>
        protected abstract ProductInstance Instantiate();
    }
}