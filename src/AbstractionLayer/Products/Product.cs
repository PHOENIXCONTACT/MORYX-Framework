using System.Runtime.Serialization;
using Marvin.AbstractionLayer.Identity;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Base class for all products
    /// </summary>
    [DataContract]
    public abstract class Product : IProduct
    {
        /// <summary>
        /// Unique type name of this instance
        /// </summary>
        public abstract string Type { get; }

        ///
        public long Id { get; set; }

        ///
        public string Name { get; set; }

        ///
        public IProduct Parent => ParentLink?.Parent;

        ///
        public IProductPartLink ParentLink { get; set; }

        /// 
        public ProductState State { get; set; }

        /// <summary>
        /// Identity of this product
        /// </summary>
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
        public Article CreateInstance()
        {
            var article = Instantiate();
            article.Product = this;
            return article;
        }

        /// <summary>
        /// Instantiate this product
        /// </summary>
        /// <returns>New instance</returns>
        protected abstract Article Instantiate();
    }
}