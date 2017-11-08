using System.Diagnostics;
using Marvin.Container;
using Marvin.Logging;
using Marvin.Modules;
using Marvin.Threading;

namespace Marvin.Runtime.Diagnostics.SystemMonitor
{
    /// <summary>
    /// Plugin which monitors the system. 
    /// </summary>
    [ExpectedConfig(typeof(SystemMonitorConfig))]
    [Plugin(LifeCycle.Singleton, typeof(IDiagnosticsPlugin), Name = PluginName)]
    public class SystemMonitorPlugin : DiagnosticsPluginBase<SystemMonitorConfig>, ILoggingComponent
    {
        /// <summary>
        /// Const name of the plugin.
        /// </summary>
        public const string PluginName = "SystemMonitor";

        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public override string Name
        {
            get { return PluginName; }
        }

        /// <summary>
        /// Instance of ParallelOperations. Injected by castle.
        /// </summary>
        public IParallelOperations ParallelOperations { get; set; }

        /// <summary>
        /// Behavior to listen allways to config changes or not.
        /// </summary>
        protected override bool AllwaysListenToConfigChanged
        {
            get { return false; }
        }

        private int _timerId;
        private PerformanceCounter _cpuCounter;
        private PerformanceCounter _ramCounter;

        /// <summary>
        /// Additional behavior for when called from [Start].
        /// </summary>
        protected override void OnStart()
        {          
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            _timerId = ParallelOperations.ScheduleExecution(MonitorSystem, Config.MonitorIntervalMs, Config.MonitorIntervalMs);
        }

        /// <summary>
        /// Additional code which will run when [Dispose].
        /// </summary>
        protected override void OnDispose()
        {
            ParallelOperations.StopExecution(_timerId);
            _cpuCounter.Dispose();
            _ramCounter.Dispose();
        }


        private void MonitorSystem()
        {
            var cpu = _cpuCounter.NextValue() + "%";
            var totalRam = _ramCounter.NextValue() + "MB";
            var usedRam = Process.GetCurrentProcess().PrivateMemorySize64 / (1024 * 1024);

            Logger.LogEntry(LogLevel.Debug, "System status: {0} CPU usage and {1} RAM usage with remaining RAM of {2}", cpu, usedRam, totalRam);
        }

        /// <summary>
        /// Logger. Injected by castel.
        /// </summary>
        [UseChild("SystemMonitor")]
        public IModuleLogger Logger { get; set; }
    }
}
