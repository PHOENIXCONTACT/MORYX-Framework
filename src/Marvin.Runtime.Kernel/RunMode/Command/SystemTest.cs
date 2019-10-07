using System;
using System.Collections.Generic;
using System.Threading;
using CommandLine;
using CommandLine.Text;
using Marvin.Configuration;
using Marvin.Runtime.Modules;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.Kernel
{
    /// <summary>
    /// Option class for the <see cref="SystemTest"/>
    /// </summary>
    [Verb("systemTest", HelpText = "Starts the runtime in system test mode. Less console output and more options.")]
    public class SystemTestOptions : RuntimeOptions
    {
        /// <summary>
        /// Increments all ports for the SystemTest
        /// </summary>
        [Option('p', "portIncrement", Required = false, Default = 0, HelpText = "Increments all ports for the SystemTest.")]
        public int PortIncrement { get; set; }

        /// <summary>
        /// Timeout to wait for a module shutdown
        /// </summary>
        [Option('t', "shutdown", Required = false, Default = 300, HelpText = "Timeout to wait for a module shutdown.")]
        public int ShutdownTimeout { get; set; }

        /// <summary>
        /// Examples for the help output
        /// </summary>
        [Usage]
        public static IEnumerable<Example> Examples =>
            new List<Example> {
                new Example("Custom config directory", new SystemTestOptions { ConfigDir = @"C:\YourApp\Config"}),
                new Example("Add port increment for all http and net.tcp ports", new SystemTestOptions { PortIncrement = 4711}),
                new Example("Override shutdown time out", new SystemTestOptions { ShutdownTimeout = 800}),
            };
    }

    /// <summary>
    /// Provides a RunMode which is used for system tests.
    /// </summary>
    [RunMode(nameof(SystemTest), typeof(SystemTestOptions))]
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
        private SystemTestOptions _options;

        /// <inheritdoc />
        public override void Setup(RuntimeOptions args)
        {
            base.Setup(args);
            _options = (SystemTestOptions) args;

            // Override configs port value
            var wcfConfig = ConfigManager.GetConfiguration<WcfConfig>(false);
            wcfConfig.HttpPort += _options.PortIncrement;
            wcfConfig.NetTcpPort += _options.PortIncrement;
        }

        /// <summary>
        /// Boot application (Starts all modules)
        /// </summary>
        protected override void Boot()
        {
            // Prepare shutdown timer
            var timeout = _options.ShutdownTimeout;
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
