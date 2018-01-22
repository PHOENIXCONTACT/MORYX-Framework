namespace Marvin.Products.Model
{
    ///
    public partial class ProductEntity
    {
        /// <summary>
        /// Creates a link to the current version of this product's properties.
        /// </summary>
        /// <param name="properties"></param>
        public void SetCurrentVersion(ProductProperties properties)
        {
            if (CurrentVersion == properties)
                return;

            if(CurrentVersion != null)
                OldVersions.Add(CurrentVersion);

            CurrentVersion = properties;
        }
    }
}
