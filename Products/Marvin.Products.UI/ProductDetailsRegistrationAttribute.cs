using Marvin.AbstractionLayer.UI;

namespace Marvin.Products.UI
{
    /// <summary>
    /// Registration attribute to register <see cref="IProductDetails"/> for a product group
    /// </summary>
    public class ProductDetailsRegistrationAttribute : DetailsRegistrationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductDetailsRegistrationAttribute"/> class.
        /// </summary>
        public ProductDetailsRegistrationAttribute(string typeName) 
            : base(typeName, typeof(IProductDetails))
        {

        }
    }
}