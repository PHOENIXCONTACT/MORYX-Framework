using System;
using System.Collections.Generic;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Interface to provide access to named parts
    /// </summary>
    public interface IArticleParts
    {
        /// <summary>
        /// Id of the <see cref="IProductPartLink"/> that created this part.
        /// 0 means this article was created from a product 
        /// directly.
        /// </summary>
        long PartLinkId { get; set; }

        /// <summary>
        /// Parts of this article
        /// </summary>
        ICollection<ArticlePart> Parts { get; }
    }

    /// <summary>
    /// Interface for a named part
    /// </summary>
    public class ArticlePart : IEquatable<ArticlePart>
    {
        /// <summary>
        /// Create part wrapper
        /// </summary>
        /// <param name="name"></param>
        /// <param name="article"></param>
        public ArticlePart(string name, Article article)
        {
            Article = article;
            Name = name;
        }

        /// <summary>
        /// Name of this part
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Article instance of this part
        /// </summary>
        public Article Article { get; set; }

        /// <summary>
        /// Id of the <see cref="IProductPartLink"/>
        /// </summary>
        public long PartLinkId 
        {
            get { return ((IArticleParts) Article).PartLinkId; } 
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(ArticlePart other)
        {
            return other != null && other.Name == Name && other.Article == Article;
        }

        /// <summary>
        /// Compares this object with another one.
        /// </summary>
        /// <param name="obj">The object to compare with</param>
        /// <returns>True, if the values of the objects are the same.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as ArticlePart);
        }

        /// <summary>
        /// Compares two ArticlePart objects.
        /// </summary>
        /// <param name="part1">The first ArticlePart</param>
        /// <param name="part2">The second ArticlePart</param>
        /// <returns>True, if the values of the objects are the same.</returns>
        public static bool operator ==(ArticlePart part1, ArticlePart part2)
        {
            if (((object)part1) == null || ((object)part2) == null)
                return Object.Equals(part1, part2);

            return part1.Equals(part2);
        }

        /// <summary>
        /// Compares two ArticlePart objects.
        /// </summary>
        /// <param name="part1">The first ArticlePart</param>
        /// <param name="part2">The second ArticlePart</param>
        /// <returns>True, if the values of the objects differ.</returns>
        public static bool operator !=(ArticlePart part1, ArticlePart part2)
        {
            if (((object)part1) == null || ((object)part2) == null)
                return !Object.Equals(part1, part2);

            return !(part1.Equals(part2));
        }

        /// <summary>
        /// Create hash code from name and referenced article
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode() << 16 ^ Article.GetHashCode();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} - {1}", Name, Article.Type);
        }
    }
}