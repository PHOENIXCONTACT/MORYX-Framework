using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Marvin.AbstractionLayer.Identity;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Base class for all articles.
    /// </summary>
    [DataContract]
    public abstract class Article : IQuickCast, IArticleParts, IPersistentObject, IIdentifiableObject
    {
        ///
        public abstract string Type { get; }

        /// <summary>
        /// Prepare article instance with the current date if it is 
        /// not overwritten by storage
        /// </summary>
        protected Article()
        {
            ProductionDate = DateTime.Now;
        }
 
        /// <summary>
        /// The ID of this article
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Indicator that this article was reworked/repaired/refurbished
        /// </summary>
        public bool Reworked { get; set; }

        /// <summary>
        /// Date of production
        /// </summary>
        public DateTime ProductionDate { get; set; }

        /// <summary>
        /// Product this article is an instance of
        /// </summary>
        public IProduct Product { get; set; }

        /// <summary>
        /// The current state of the article
        /// </summary>
        public ArticleState State { get; set; }

        /// <summary>
        /// Unique identifier
        /// </summary>
        public IIdentity Identity { get; set; }

        #region Parts

        private readonly IList<ArticlePart> _parts = new List<ArticlePart>(); 

        /// <summary>
        /// Get a single part with this name
        /// </summary>
        protected IPartWrapper<T> Single<T>([CallerMemberName] string name = null)
            where T : Article
        {
            return new SinglePart<T>(_parts, name);
        }

        /// <summary>
        /// Get a single part with this name
        /// </summary>
        protected ICollection<T> Multiple<T>([CallerMemberName] string name = null)
            where T : Article
        {
            return new MultipleParts<T>(_parts, name);
        }

        /// <summary>
        /// Set multiple parts
        /// </summary>
        protected void Multiple<T>(ICollection<T> values, [CallerMemberName] string name = null)
            where T : Article
        {
            MultipleParts<T>.Replace(_parts, name, values);
        }

        /// <summary>
        /// Id of the <see cref="IProductPartLink"/> that created this part.
        /// 0 means this article was created from a product 
        /// directly.
        /// </summary>
        long IArticleParts.PartLinkId { get; set; }

        /// <summary>
        /// Parts of this article
        /// </summary>
        ICollection<ArticlePart> IArticleParts.Parts
        {
            get { return _parts; }
        }

        #endregion
    }

    /// <summary>
    /// Generic base class for product access
    /// </summary>
    public abstract class Article<TProduct> : Article
        where TProduct : IProduct
    {
        /// <summary>
        /// Typed property for product access
        /// </summary>
        public new TProduct Product
        {
            get { return (TProduct) base.Product; } 
            set { base.Product = value; }
        }
    }
}