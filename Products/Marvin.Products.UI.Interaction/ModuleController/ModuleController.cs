using Marvin.ClientFramework;
using Marvin.ClientFramework.Base;
using Marvin.Modules.Client;

namespace Marvin.Products.UI.Interaction
{
    /// <summary>
    /// Module controller handling the lifecycle of the module
    /// </summary>
    [ClientModule(ModuleName)]
    public class ModuleController : WorkspaceModuleBase<ModuleConfig>
    {
        internal const string ModuleName = "Products";

        private IProductsController _controller;

        /// <summary>
        /// Initializes the module
        /// </summary>
        protected override void OnInitialize()
        {
            // Load ResourceDetails to this container
            Container.LoadComponents<IProductDetails>();

            _controller = Container.Resolve<IProductsController>();
            _controller.Start();
        }

        /// <summary>
        /// Will be called when the module will be selected
        /// </summary>
        protected override void OnActivate()
        {

        }

        /// <summary>
        /// Will be called when the module will be deactivated
        /// </summary>
        protected override void OnDeactivate(bool close)
        {
            if (close)
                _controller.Dispose();
        }

        /// <summary>
        /// Will be called by selecting the module
        /// </summary>
        protected override IModuleWorkspace OnCreateWorkspace()
        {
            return Container.Resolve<IModuleWorkspace>(ProductsWorkspaceViewModel.WorkspaceName);
        }

        /// <summary>
        /// Will destroy the given workspace
        /// </summary>
        protected override void OnDestroyWorkspace(IModuleWorkspace workspace)
        {

        }
    }
}