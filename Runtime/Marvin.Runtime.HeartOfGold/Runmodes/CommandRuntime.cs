using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Marvin.Container;
using Marvin.Logging;
using Marvin.Modules.Server;
using Marvin.Runtime.Configuration;
using Marvin.Runtime.HeartOfGold.Commands;
using Marvin.Runtime.ModuleManagement;

namespace Marvin.Runtime.HeartOfGold
{
    public abstract class CommandRuntime : IRuntimeEnvironment
    {
        // Injected by the castle container
        public IModuleManager ModuleManager { get; set; }
        public IRuntimeConfigManager ConfigLoader { get; set; }
        public ILoggerManagement Logger { get; set; }

        private IContainer _localContainer;
        protected ICommandHandler[] CommandHandler;

        protected RuntimeArguments Arguments { get; set; }

        public void Setup(RuntimeArguments args)
        {
            Arguments = args;
        }

        private void InitializeLocalContainer()
        {
            // Prepare local container
            _localContainer = new CastleContainer(new AutoInstaller(typeof(DeveloperConsole).Assembly));
            _localContainer.SetInstances(ModuleManager, ConfigLoader, Logger, Arguments);
            CommandHandler = _localContainer.ResolveAll<ICommandHandler>().Union(new[] { new DefaultHandler(), }).ToArray();
        }

        public int RunEnvironment()
        {
            InitializeLocalContainer();

            Boot();

            RunTextEnvironment();

            ShutDown();

            return 0;
        }

        /// <summary>
        /// Boot application (Starts all modules)
        /// </summary>
        protected virtual void Boot()
        {
            // Welcome message and start
            ModuleManager.StartModules();
        }

        /// <summary>
        /// Read commands and pass to execute Command method
        /// </summary>
        protected abstract void RunTextEnvironment();

        /// <summary>
        /// Shutdown application (Stop all modules
        /// </summary>
        protected virtual void ShutDown()
        {
            ModuleManager.StopModules();
        }

        protected void ExecuteCommand(string command)
        {
            var parts = command.Split(' ');
            var match = CommandHandler.First(handler => handler.CanHandle(parts[0]));
            try
            {
                match.Handle(parts);
            }
            catch (Exception ex)
            {
                PrintText(string.Format("Command invocation failed: {0}", ex.Message));
            }
        }

        protected abstract void PrintText(params string[] lines);
    }
}
