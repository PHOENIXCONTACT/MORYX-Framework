using Marvin.AbstractionLayer;

namespace Marvin.Products.Samples
{
    /// <summary>
    /// Instance of a single cardboard package
    /// </summary>
    public class WatchPackageInstance : ProductInstance<WatchPackageType>
    {
        /// <inheritdoc />
        public override string Type => nameof(WatchPackageInstance);

        /// <summary>
        /// Reference to the watch packed in this box
        /// </summary>
        public WatchInstance PackedWatch { get; set; }
    }
}