using Marvin.AbstractionLayer;
using Marvin.Modules.ModulePlugins;

namespace Marvin.Products.Management.Importers
{
    /// <summary>
    /// Interface for plugins that can import products from file
    /// </summary>
    public interface IProductImporter : IConfiguredModulePlugin<ProductImporterConfig>
    {
        /// <summary>
        /// Name of the importer
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get the parameters of this importer
        /// </summary>
        IImportParameters Parameters { get; }

        /// <summary>
        /// Update parameters based on partial input
        /// </summary>
        IImportParameters Update(IImportParameters currentParameters);

        /// <summary>
        /// Import products using given parameters
        /// </summary>
        IProduct[] Import(IImportParameters parameters);
    }
}