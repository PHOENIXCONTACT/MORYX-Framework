using System;
using System.Linq;
using Marvin.Container;
using Marvin.Runtime.Configuration;

namespace Marvin.Runtime.Kernel
{
    /// <summary>
    /// Command run mode. Provides several command functionality.
    /// </summary>
    public abstract class CommandRunMode<TOptions> : RunModeBase<TOptions> where TOptions : RuntimeOptions
    {
        /// <summary>
        /// Config manger instance. Injected by the castle container
        /// </summary>
        public IRuntimeConfigManager ConfigLoader { get; set; }

        /// <summary>
        /// Logger management instance. Injected by the castle container
        /// </summary>
        public IServerLoggerManagement Logger { get; set; }

        /// <summary>
        /// Local container of the command run mode
        /// </summary>
        private IContainer _container;

        /// <summary>
        /// The command handler instance.
        /// </summary>
        protected ICommandHandler[] CommandHandler;

        private void InitializeLocalContainer()
        {
            // Prepare local container
            _container = new CastleContainer()
                .SetInstance(ModuleManager).SetInstance(ConfigLoader)
                .SetInstance(Logger);

            _container.LoadComponents<ICommandHandler>();

            CommandHandler = _container.ResolveAll<ICommandHandler>().Union(new[] { new DefaultHandler(), }).ToArray();
        }

        /// <summary>
        /// Sequence of states which should be done in the right order.
        /// </summary>
        /// <returns>0 when all methods run through without error.</returns>
        public override RuntimeErrorCode Run()
        {
            InitializeLocalContainer();

            Boot();

            RunTextEnvironment();

            ShutDown();

            return RuntimeErrorCode.NoError;
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
        /// Shutdown application (Stop all modules)
        /// </summary>
        protected virtual void ShutDown()
        {
            ModuleManager.StopModules();
        }

        /// <summary>
        /// Execute an enterd command.
        /// </summary>
        /// <param name="command">The command which should be executed.</param>
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
                PrintText($"Command invocation failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Abstract: Prints the text from the params on the screen.
        /// </summary>
        /// <param name="lines">The text which should be printed on the screen.</param>
        protected abstract void PrintText(params string[] lines);
    }
}
