using System;
using System.Linq;
using Marvin.Container;
using Marvin.Runtime.Configuration;
using Marvin.Runtime.Modules;

namespace Marvin.Runtime.Kernel
{
    /// <summary>
    /// Command run mode. Provides serveral command functionallity. 
    /// </summary>
    public abstract class CommandRunMode : IRunMode
    {
        /// <summary>
        /// Module manager instance. Injected by the castle container
        /// </summary>
        public IModuleManager ModuleManager { get; set; }

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

        /// <summary>
        /// Get/set  the runtime arguments.
        /// </summary>
        protected RuntimeArguments Arguments { get; set; }

        /// <summary>
        /// Sets the arguments for the command run mode.
        /// </summary>
        /// <param name="args">Arguments which should be used by the run mode.</param>
        public virtual void Setup(RuntimeArguments args)
        {
            Arguments = args;
        }

        private void InitializeLocalContainer()
        {
            // Prepare local container
            _container = new CastleContainer()
                .SetInstance(ModuleManager).SetInstance(ConfigLoader)
                .SetInstance(Logger).SetInstance(Arguments);

            _container.LoadComponents<ICommandHandler>();

            CommandHandler = _container.ResolveAll<ICommandHandler>().Union(new[] { new DefaultHandler(), }).ToArray();
        }

        /// <summary>
        /// Sequence of states which should be done in the right order.
        /// </summary>
        /// <returns>0 when all methods run through without error.</returns>
        public RuntimeErrorCode Run()
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
