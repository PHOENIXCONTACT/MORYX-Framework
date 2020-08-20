// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Threading;
using Moryx.Communication;
using Moryx.Configuration;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel
{
    /// <summary>
    /// Provides a RunMode which is used for system tests.
    /// </summary>
    [RunMode(typeof(SystemTestOptions))]
    public class SystemTest : CommandRunMode<SystemTestOptions>
    {
        /// <summary>
        /// Const name of the RunMode.
        /// </summary>
        public const string RunModeName = "SystemTest";

        /// <summary>
        /// Configuration manager instance. Injected by castle.
        /// </summary>
        public IConfigManager ConfigManager { get; set; }

        private Timer _shutdownTimer;

        /// <inheritdoc />
        public override void Setup(RuntimeOptions args)
        {
            base.Setup(args);

            // Override configs port value
            var portConfig = ConfigManager.GetConfiguration<PortConfig>(false);
            portConfig.HttpPort += Options.PortIncrement;
            portConfig.NetTcpPort += Options.PortIncrement;
        }

        /// <summary>
        /// Boot application (Starts all modules)
        /// </summary>
        protected override void Boot()
        {
            // Prepare shutdown timer
            var timeout = Options.ShutdownTimeout;
            _shutdownTimer = new Timer(ShutDownTimer, null, TimeSpan.FromSeconds(timeout), Timeout.InfiniteTimeSpan);

            // Register to service manager event
            ModuleManager.ModuleStateChanged += OnModuleStateChanged;
            base.Boot();
        }

        /// <summary>
        /// Read commands and pass to execute Command method
        /// </summary>
        protected override void RunTextEnvironment()
        {
            var command = string.Empty;
            while (command != "exit")
            {
                command = Console.ReadLine();

                switch (command)
                {
                    case "exit":
                        break;
                    default:
                        ExecuteCommand(command);
                        command = string.Empty;
                        break;
                }
            }
        }

        /// <summary>
        /// Write the text which should be printed to the connected client.
        /// </summary>
        /// <param name="lines">Text which should be printed.</param>
        protected override void PrintText(params string[] lines)
        {
        }

        /// <summary>
        /// Print the state change of a module in the console.
        /// </summary>
        /// <param name="sender">Sender object, here IServerModule.</param>
        /// <param name="eventArgs">EventArgs with the new state.</param>
        protected void OnModuleStateChanged(object sender, ModuleStateChangedEventArgs eventArgs)
        {
            lock (this)
            {
                var module = (IServerModule)sender;
                Console.WriteLine("{0} changed state to {1}", module.Name, eventArgs.NewState);
            }
        }

        private void ShutDownTimer(object state)
        {
            try
            {
                ShutDown();
            }
            finally
            {
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Shut down application, stop modules, release and dispose the necessary objects.
        /// </summary>
        protected override void ShutDown()
        {
            base.ShutDown();

            ModuleManager.ModuleStateChanged -= OnModuleStateChanged;

            _shutdownTimer.Dispose();

            Thread.Sleep(100);
        }
    }
}
