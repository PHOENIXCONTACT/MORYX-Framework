// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ServiceProcess;
using Marvin.Runtime.Configuration;
using Marvin.Runtime.Modules;

namespace Marvin.Runtime.WinService
{
    /// <summary>
    /// Windows service implementation as runtime run mode
    /// </summary>
    internal class HeartOfGoldService : ServiceBase
    {
        private readonly IModuleManager _moduleManager;
        private readonly IRuntimeConfigManager _configLoader;

        /// <summary>
        /// Constructor for the service
        /// </summary>
        public HeartOfGoldService(IModuleManager moduleManager, IRuntimeConfigManager configLoader)
        {
            ServiceName = Platform.Current.ProductName;

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
            EventLog.WriteEntry($"{ServiceName} service starting...");
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
            EventLog.WriteEntry($"{ServiceName} service stopping...");
            Stop(false);
            EventLog.WriteEntry($"{ServiceName} service stopped!");
        }

        protected override void OnShutdown()
        {
            EventLog.WriteEntry("Stopping due to shutdown...");
            Stop(false);
            EventLog.WriteEntry($"{ServiceName} service stopped!");
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
    }
}
