using Marvin.AbstractionLayer.Identity;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Interface for all typed products
    /// </summary>
    public interface IProduct : IQuickCast, IPersistentObject, IIdentifiableObject
    {
        /// <summary>
        /// Display name of this product
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Parent of this product, if it was used as a part. This
        /// is <value>null</value> for "Root-Products"
        /// </summary>
        IProduct Parent { get; }

        /// <summary>
        /// Current state of this product
        /// </summary>
        ProductState State { get; set; }

        /// <summary>
        /// Create article instance of this type
        /// </summary>
        Article CreateInstance();
    }
}