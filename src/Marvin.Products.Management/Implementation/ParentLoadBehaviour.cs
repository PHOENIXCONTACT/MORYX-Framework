namespace Marvin.Products.Management
{

    /// <summary>
    /// Behaviour for <see cref="IProductTypeStrategy"/> how parent references shall be treated
    /// </summary>
    public enum ParentLoadBehaviour
    {
        /// <summary>
        /// Ignore parent reference
        /// </summary>
        Ignore = 1,
        /// <summary>
        /// Load parent without parts
        /// </summary>
        Flat = 1 << 8,
        /// <summary>
        /// Load full parent with all its parts
        /// </summary>
        Full = 1 << 24
    }
}