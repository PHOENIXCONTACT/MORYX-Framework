using Marvin.Container;
using Marvin.Products.Management.Importers;

namespace Marvin.Products.Management
{
    /// <summary>
    /// Factory to create configured importers
    /// </summary>
    [PluginFactory(typeof(IConfigBasedComponentSelector))]
    internal interface IProductImporterFactory
    {
        /// <summary>
        /// Create new importer
        /// </summary>
        IProductImporter Create(ProductImporterConfig config);

        /// <summary>
        /// Destroy an importer
        /// </summary>
        void Destroy(IProductImporter instance);
    }
}