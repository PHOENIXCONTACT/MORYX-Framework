namespace Marvin.AbstractionLayer.Identity
{
    /// <summary>
    /// Base interface for objects with an <see cref="IIdentity"/>
    /// </summary>
    public interface IIdentifiableObject
    {
        /// <summary>
        /// Identity of this product
        /// </summary>
        IIdentity Identity { get; }
    }
}
