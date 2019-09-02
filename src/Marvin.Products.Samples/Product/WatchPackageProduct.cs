using System.Collections.Generic;
using System.ComponentModel;
using Marvin.AbstractionLayer;

namespace Marvin.Products.Samples
{
    /// <summary>
    /// Product that represents the package used to ship a watch
    /// </summary>
    [DisplayName("Verpackung")]
    public class WatchPackageProduct : Product
    {
        /// <inheritdoc />
        public override string Type => nameof(WatchPackageProduct);

        /// <summary>
        /// Watches that can be shipped in this package
        /// </summary>
        [DisplayName("Mögliche Uhren")]
        public List<ProductPartLink<WatchProduct>> PossibleWatches { get; set; }

        protected override Article Instantiate()
        {
            return new WatchPackage();
        }
    }
}