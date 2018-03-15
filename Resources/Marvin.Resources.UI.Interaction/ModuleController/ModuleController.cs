using System.Windows.Media;
using C4I;
using Marvin.ClientFramework;

namespace Marvin.Resources.UI.Interaction
{
    /// <summary>
    /// Module controller of the Resources UI.
    /// </summary>
    [ClientModule(ModuleName)]
    public class ModuleController : WorkspaceModuleBase<ModuleConfig>
    {
        internal const string ModuleName = "Resources";

        private IModuleWorkspace _defaultWorkspace;

        /// <inheritdoc />
        public override Geometry Icon => ShapeFactory.GetShapeGeometry(CommonShapeType.Cells);

        ///
        protected override void OnInitialize()
        {
            // Register type factory
            Container.Register<IResourceDialogFactory>();

            // Load ResourceDetails to this container
            Container.LoadComponents<IResourceDetails>();

            // Load ResourceInteractionControllers to the local container
            Container.LoadComponents<IResourceInteractionController>();

            // Start resource controller to connect to interaction web service
            var interactionControllers = Container.ResolveAll<IResourceInteractionController>();
            foreach (var controller in interactionControllers)
            {
                controller.Start();
            }

            // Set the default workspace
            _defaultWorkspace = Container.Resolve<IModuleWorkspace>(InteractionWorkspaceViewModel.WorkspaceName);
        }

        ///
        protected override void OnActivate()
        {

        }

        ///
        protected override void OnDeactivate(bool close)
        {

        }

        ///
        protected override IModuleWorkspace OnCreateWorkspace()
        {
            return _defaultWorkspace;
        }

        ///
        protected override void OnDestroyWorkspace(IModuleWorkspace workspace)
        {

        }
    }
}