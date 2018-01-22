using System.Threading.Tasks;
using Marvin.AbstractionLayer.UI;

namespace Marvin.Products.UI
{
    /// <summary>
    /// Interface for product detail views
    /// </summary>
    public interface IProductDetails : IEditModeViewModel, IDetailsViewModel
    {
        /// <summary>
        /// Method to load the product details
        /// </summary>
        Task Load(long productId);

        /// <summary>
        /// Id of the product which is currently selected to show its details
        /// </summary>
        long ProductId { get; }
    }
}
