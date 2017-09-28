using Marvin.Container;
using Marvin.Modules.ModulePlugins;
using Marvin.Runtime.Diagnostics.ClientLog.Wcf;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.Diagnostics.ClientLog
{
    
    /// <summary>
    /// Start a log session witht he configurated host.
    /// </summary>
    [ExpectedConfig(typeof(ClientLogConfig))]
    [Plugin(LifeCycle.Singleton, typeof(IDiagnosticsPlugin), Name = PluginName)]
    public class ClientLog : DiagnosticsPluginBase<ClientLogConfig>
    {
        /// <summary>
        /// Const name of the plugin.
        /// </summary>
        internal const string PluginName = "ClientLog";
        /// <summary>
        /// The name of the plugin.
        /// </summary>
        public override string Name
        {
            get { return PluginName; }
        }

        /// <summary>
        /// Behavior to listen allways to config changes or not.
        /// </summary>
        protected override bool AllwaysListenToConfigChanged
        {
            get { return false; }
        }

        private IConfiguredServiceHost _host;

        /// <summary>
        /// Instance of the host factory. Injected by castle.
        /// </summary>
        public IConfiguredHostFactory HostFactory { get; set; }

        /// <summary>
        /// Additional code which will be called in [Init]
        /// </summary>
        protected override void OnInitialize()
        {
            _host = HostFactory.CreateHost<IRemoteLogging>(Config.RemoteLogHost);
        }

        /// <summary>
        /// Additional behavior for when called from [Start].
        /// </summary>
        protected override void OnStart()
        {
            _host.Start();
        }

        /// <summary>
        /// Additional code which will run when [Dispose].
        /// </summary>
        protected override void OnDispose()
        {
            _host.Dispose();
        }
    }
}
