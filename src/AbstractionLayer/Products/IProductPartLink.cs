using System.Collections.Generic;
using System.Linq;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Base interface for the link
    /// </summary>
    public interface IProductPartLink : IPersistentObject
    {
        /// <summary>
        /// Generic access to the product of this part link
        /// </summary>
        IProduct Product { get; set; }

        /// <summary>
        /// Create single article instance for this part
        /// </summary>
        Article Instantiate();
    }

    /// <summary>
    /// API for part wrapper
    /// </summary>
    /// <typeparam name="TProduct"></typeparam>
    public interface IProductPartLink<TProduct> : IProductPartLink 
        where TProduct : IProduct
    {
        /// <summary>
        /// Typed product of this part
        /// </summary>
        new TProduct Product { get; set; }
    }

    /// <summary>
    /// Extension to instantiate article collection from product parts collection
    /// </summary>
    public static class PartLinkExtension
    {
        /// <summary>
        /// Instantiate article collection
        /// </summary>
        public static ICollection<TArticle> Instantiate<TArticle>(this IEnumerable<IProductPartLink> parts)
            where TArticle : Article
        {
            return parts.Select(p => (TArticle)p.Instantiate()).ToList();
        }
    }
}