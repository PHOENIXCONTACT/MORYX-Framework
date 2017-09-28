using Marvin.Runtime.Base;
using Marvin.Runtime.Container;
using Marvin.Runtime.ModuleManagement;
using Marvin.Runtime.ServerModules;


namespace Marvin.Runtime.Diagnostics
{
    /// <summary>
    /// Diagnostic Module to handle the diagnostic plugins.
    /// </summary>
    [ServerModule(ModuleName)]
    public class ModuleController : ServerModuleFacadeControllerBase<ModuleConfig>, IPlatformModule
    {
        /// <summary>
        /// Name of this module
        /// </summary>
        private const string ModuleName = "Diagnostics";

        /// <summary>
        /// The diagnostic controller helper to manage the start and stop of configured plugins.
        /// </summary>
        private DiagnosticControllerHelper _diagnosticControllerHelper;

        /// <summary>
        /// Name of this module
        /// </summary>
        public override string Name => ModuleName;

        private IModuleManager _moduleManager;

        #region Dependency Injection

        /// <summary>
        /// Set the module manager.
        /// </summary>
        /// <param name="moduleManager">The module manager which should be set.</param>
        public void SetModuleManager(IModuleManager moduleManager)
        {
            _moduleManager = moduleManager;
        }
        #endregion

        #region State transition

        /// <summary>
        /// Code executed on start up and after service was stopped and should be started again
        /// </summary>
        protected override void OnInitialize()
        {
            Container.SetInstance(LoggerManagement).SetInstance(_moduleManager);
            Container.LoadComponents<IDiagnosticsPlugin>();           
        }

        /// <summary>
        /// Code executed after OnInitialize
        /// </summary>
        protected override void OnStart()
        {
            _diagnosticControllerHelper = Container.Resolve<DiagnosticControllerHelper>();
            _diagnosticControllerHelper.OnStart();
        }

        /// <summary>
        /// Code executed when service is stopped
        /// </summary>
        protected override void OnStop()
        {
            _diagnosticControllerHelper.OnStop();
        }

        #endregion
    }
}