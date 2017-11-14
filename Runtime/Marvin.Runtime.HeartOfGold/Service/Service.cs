using System.ServiceProcess;
using Marvin.Runtime.Configuration;
using Marvin.Runtime.Modules;

namespace Marvin.Runtime.HeartOfGold
{
    /// <summary>
    /// MARVIN service
    /// </summary>
    internal class MarvinService : ServiceBase
    {
        private readonly IModuleManager _moduleManager;
        private readonly IRuntimeConfigManager _configLoader;

        /// <summary>
        /// Constructor for the service
        /// </summary>
        public MarvinService(IModuleManager moduleManager, IRuntimeConfigManager configLoader)
        {
            InitializeComponent();
            _moduleManager = moduleManager;
            _configLoader = configLoader;
        }

        public void Run()
        {
            // Set flags
            CanShutdown = true;
            CanPauseAndContinue = true;

            Run(new ServiceBase[] { this });
        }

        #region ServiceBase

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("Marvin core service starting...");
            Start();
        }

        protected override void OnContinue()
        {
            Start();
        }

        protected override void OnPause()
        {
            Stop(true);
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry("Marvin core service stopping...");
            Stop(false);
            EventLog.WriteEntry("Marvin core service stopped!");
        }

        protected override void OnShutdown()
        {
            EventLog.WriteEntry("Stopping due to shutdown...");
            Stop(false);
            EventLog.WriteEntry("Marvin service stopped!");
        }

        #endregion

        #region Methods

        private void Start()
        {
            _moduleManager.StartModules();
        }

        private void Stop(bool paused)
        {
            _moduleManager.StopModules();
            if(paused)
                _configLoader.ClearCache();
            else
                _configLoader.SaveAll();
        }

        #endregion

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            ServiceName = Platform.Current.ProductName;
        }

        #endregion
    }
}
