using Devart.Data.PostgreSql;
using Marvin.Container;
using Marvin.Modules.ModulePlugins;

namespace Marvin.Runtime.Diagnostics.DbMonitor
{
    /// <summary>
    /// Monitors all db accesses.
    /// </summary>
    [ExpectedConfig(typeof(DbMonitorConfig))]
    [Plugin(LifeCycle.Singleton, typeof(IDiagnosticsPlugin), Name = PluginName)]
    public class DbMonitorPlugin : DiagnosticsPluginBase<DbMonitorConfig>
    {
        private PgSqlMonitor _monitor;
        internal const string PluginName = "DbMonitor";

        /// <summary>
        /// The name of the plugin.
        /// </summary>
        public override string Name => PluginName;

        /// <summary>
        /// Creates a new instance of the <see cref="PgSqlMonitor"/>.
        /// </summary>
        protected override void OnStart()
        {
            // AKTIVATE dbMonitor FROM DevArt dotConnect for Postgres Lib. 
            // Über Command-Prompt dbMonitor.exe starten
            _monitor = new PgSqlMonitor { IsActive = true, Host = Config.HostName, Port = Config.Port };
        }

        /// <summary>
        /// Is called when [Dispose]
        /// </summary>
        protected override void OnDispose()
        {
            _monitor.IsActive = false;
            _monitor = null;
        }

        /// <summary>
        /// Behavior to listen allways to config changes or not.
        /// </summary>
        protected override bool AllwaysListenToConfigChanged => true;
    }
}