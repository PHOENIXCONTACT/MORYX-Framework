using System;
using Marvin.AbstractionLayer.Identity;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Identity for products consisting of article number and revision
    /// </summary>
    public class ProductIdentity : IIdentity
    {
        /// <summary>
        /// Const value representing the latest revision of a product. This is intended for later changes of this constant.
        /// </summary>
        public const short LatestRevision = short.MinValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductIdentity"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="revision">The revision.</param>
        public ProductIdentity(string identifier, short revision)
        {
            Identifier = identifier;
            Revision = revision;
        }

        /// <summary>
        /// Create a product identity that represents the latest revision of a given identifier
        /// </summary>
        public static ProductIdentity AsLatestRevision(string identifier) => new ProductIdentity(identifier, LatestRevision);

        /// <summary>
        /// Main and unique string identifier
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Set the identifier for this identity
        /// </summary>
        /// <param name="identifier"></param>
        /// <exception cref="InvalidOperationException">Identifiers of some identities must not be overriden</exception>
        public void SetIdentifier(string identifier)
        {
            throw new InvalidOperationException("Product identifiers must not be overriden!");
        }

        /// <summary>
        /// Revision of this instance
        /// </summary>
        public short Revision { get; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(IIdentity other)
        {
            var casted = other as ProductIdentity;
            return casted != null && casted.Identifier == Identifier && casted.Revision == Revision;
        }

        /// <summary>
        /// Compares this object with another one.
        /// </summary>
        /// <param name="obj">The object to compare with</param>
        /// <returns>True, if the values of the objects are the same.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as IIdentity);
        }

        /// <summary>
        /// Compares two ProductIdentity objects.
        /// </summary>
        /// <param name="id1">The first ProductIdentity</param>
        /// <param name="id2">The second ProductIdentity</param>
        /// <returns>True, if the values of the objects are the same.</returns>
        public static bool operator ==(ProductIdentity id1, ProductIdentity id2)
        {
            if (((object)id1) == null || ((object)id2) == null)
                return Object.Equals(id1, id2);

            return id1.Equals(id2);
        }

        /// <summary>
        /// Compares two ProductIdentity objects.
        /// </summary>
        /// <param name="id1">The first ProductIdentity</param>
        /// <param name="id2">The second ProductIdentity</param>
        /// <returns>True, if the values of the objects differ.</returns>
        public static bool operator !=(ProductIdentity id1, ProductIdentity id2)
        {
            if (((object)id1) == null || ((object)id2) == null)
                return !Object.Equals(id1, id2);

            return !(id1.Equals(id2));
        }

        /// <summary>
        /// Create hash of identifier and revision
        /// </summary>
        public override int GetHashCode()
        {
            return Identifier.GetHashCode() ^ Revision << 24;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{Identifier}-{Revision:D2}";
        }

    }
}