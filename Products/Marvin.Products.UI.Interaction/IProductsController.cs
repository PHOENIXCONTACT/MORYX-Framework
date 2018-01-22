using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marvin.AbstractionLayer.UI;
using Marvin.Container;
using Marvin.Modules.ModulePlugins;
using Marvin.Products.UI.Interaction.InteractionSvc;
using Marvin.Tools.Wcf;

namespace Marvin.Products.UI.Interaction
{
    /// <summary>
    /// Model interface for product interaction
    /// </summary>
    internal interface IProductsController : IInteractionController, IModulePlugin
    {
        /// <summary>
        /// Represents the product structure tree
        /// </summary>
        ProductStructureEntry[] Structure { get; }

        /// <summary>
        /// Customization of this application
        /// </summary>
        ProductCustomization Customization { get; }

        /// <summary>
        /// Will updated the product structure tree
        /// </summary>
        void UpdateStructure();

        /// <summary>
        /// Will return a full product
        /// </summary>
        Task<ProductModel> GetDetails(long id);

        /// <summary>
        /// Will save the product
        /// </summary>
        Task<ProductModel> Save(ProductModel product);

        /// <summary>
        /// Update importer parameters
        /// </summary>
        /// <param name="importer">Name of the importer</param>
        /// <param name="currentParameters">List of parameters</param>
        /// <returns>List of importer parameters</returns>
        Task<ImportParameter[]> UpdateParameters(string importer, ImportParameter[] currentParameters);
        /// <summary>
        /// Import product
        /// </summary>
        /// <param name="importerName">Name of the importer</param>
        /// <param name="parameters">List of parameters</param>
        /// <returns>Imported product</returns>
       Task<ProductModel> ImportProduct(string importerName, ImportParameter[] parameters);

        /// <summary>
        /// Create product revision
        /// </summary>
        /// <param name="productId">Product Id</param>
        /// <param name="revisionNo">New revision number</param>
        /// <param name="comment">Comment for new revision</param>
        /// <returns>Created product revision</returns>
        Task<ProductModel> CreateRevision(long productId, short revisionNo, string comment);

        /// <summary>
        /// Gets product reviosns
        /// </summary>
        /// <param name="identifier">Product indentifier</param>
        /// <returns>Array of ProductRevisionEntry</returns>
        Task<ProductRevisionEntry[]> GetProductRevisions(string identifier);

        /// <summary>
        /// Event which will be raised if the product structure will be updated
        /// </summary>
        event EventHandler StructureUpdated;
    }

    [Component(LifeCycle.Singleton, typeof(IProductsController))]
    internal class ProductsController : HttpServiceConnectorBase<ProductInteractionClient, IProductInteraction>, IProductsController
    {
        protected override string MinServerVersion => "1.1.1.0";

        protected override string ClientVersion => "1.1.1.0";

        public ProductStructureEntry[] Structure { get; private set; }

        public ProductCustomization Customization { get; private set; }

        protected override async void ClientCallback(ConnectionState state, ProductInteractionClient client)
        {
            base.ClientCallback(state, client);
            if (state != ConnectionState.Success)
                return;

            await LoadStructure();
            Customization = await WcfClient.GetCustomizationAsync();
        }

        private async Task LoadStructure()
        {
            Structure = await WcfClient.GetProductStructureAsync();
            RaiseStructureUpdated();
        }

        public void UpdateStructure()
        {
            Task.Run(LoadStructure);
        }

        public Task<ProductModel> GetDetails(long id)
        {
            return WcfClient.GetProductDetailsAsync(id);
        }

        public Task<ProductModel> Save(ProductModel product)
        {
            return WcfClient.SaveProductAsync(product);
        }

        public Task<ImportParameter[]> UpdateParameters(string importer, ImportParameter[] currentParameters)
        {
            return WcfClient.UpdateParametersAsync(importer, currentParameters);
        }

        public Task<ProductModel> ImportProduct(string importerName, ImportParameter[] parameters)
        {
            return WcfClient.ImportProductAsync(importerName, parameters);
        }

        public event EventHandler StructureUpdated;
        private void RaiseStructureUpdated()
        {
            StructureUpdated?.Invoke(this, EventArgs.Empty);
        }

        public Task<ProductModel> CreateRevision(long productId, short revisionNo, string comment)
        {
            return WcfClient.CreateRevisionAsync(productId, revisionNo, comment);
        }

        public Task<ProductRevisionEntry[]> GetProductRevisions(string identifier)
        {
            return WcfClient.GetProductRevisionsAsync(identifier);
        }
    }
}