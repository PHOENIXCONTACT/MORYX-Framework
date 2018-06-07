using Marvin.AbstractionLayer;

namespace Marvin.Products.Samples
{
    /// <summary>
    /// Instance of a single cardboard package
    /// </summary>
    public class WatchPackage : Article<WatchPackageProduct>
    {
        /// <inheritdoc />
        public override string Type => nameof(WatchPackage);

        /// <summary>
        /// Reference to the watch packed in this box
        /// </summary>
        public WatchArticle PackedWatch
        {
            get { return Single<WatchArticle>().Part; }
            set { Single<WatchArticle>().Part = value; }
        }
    }
}