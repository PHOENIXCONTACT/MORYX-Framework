using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLine;
using Marvin.Container;
using Marvin.Model;
using Marvin.Modules;
using Marvin.Runtime.Configuration;
using Marvin.Runtime.Container;
using Marvin.Runtime.Kernel.Update;
using Marvin.Runtime.Modules;
using Marvin.Tools;

namespace Marvin.Runtime.Kernel
{
    /// <summary>
    /// Base kernel loader for the runtime
    /// </summary>
    public class HeartOfGold
    {
        private readonly string[] _args;

        /// <summary>
        /// Current global container
        /// </summary>
        private IContainer _container;

        /// <summary>
        /// Arguments passed to this instance
        /// </summary>
        private RuntimeArguments _arguments;

        /// <summary>
        /// Creates an instance of the <see cref="HeartOfGold"/>
        /// </summary>
        public HeartOfGold(string[] args)
        {
            _args = args;
        }

        /// <summary>
        /// Starts the runtime
        /// </summary>
        public RuntimeErrorCode Run()
        {
            var errors = RuntimeArguments.Build(_args, out _arguments);

            // Check for general errors
            if (_arguments == null)
                return RuntimeErrorCode.Error;

            // Check if help was executed
            if (errors.Any(e => e.Tag == ErrorType.HelpRequestedError))
                return RuntimeErrorCode.NoError;

            // Check for other general errors
            if (errors.Any())
                return RuntimeErrorCode.Error;

            // Set working directory to location of this exe
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            // Load all assemblies from directory before accessing any other code
            AppDomainBuilder.LoadAssemblies();

            // This is necessary to enable assembly lookup from AppDomain
            AppDomain.CurrentDomain.AssemblyResolve += AppDomainBuilder.ResolveAssembly;

            // This will give a message to the user before crashing
            AppDomain.CurrentDomain.UnhandledException += CrashHandler.HandleCrash;

            // Prepare container
            _container = CreateContainer();

            // Load and run environment
            var env = LoadEnvironment();
            var result = env == null ? RuntimeErrorCode.Error : RunEnvironment(env);

            // Clean up and exit
            try
            {
                _container.Destroy();
            }
            catch (Exception)
            {
                // Ignore it.
            }

            AppDomain.CurrentDomain.AssemblyResolve -= AppDomainBuilder.ResolveAssembly;
            AppDomain.CurrentDomain.UnhandledException -= CrashHandler.HandleCrash;

            return result;
        }

        /// <summary>
        /// Create the container and load all relevant directories
        /// </summary>
        /// <returns></returns>
        private IContainer CreateContainer()
        {
            var container = new GlobalContainer();

            // Register runtimes
            container.ExecuteInstaller(new AutoInstaller(GetType().Assembly));

            // Load additional runtimes
            container.LoadComponents<IRunMode>();

            // Load kernel and core modules
            container.LoadComponents<object>(type => type.GetCustomAttribute<KernelComponentAttribute>() != null);
            container.LoadComponents<IServerModule>(module => module.GetCustomAttribute<ServerModuleAttribute>() != null);

            // Load models
            container.LoadComponents<IUnitOfWorkFactory>();

            // Load user modules
            container.LoadComponents<IServerModule>();

            return container;
        }

        /// <summary>
        /// Method to wrap all boot exceptions
        /// </summary>
        private IRunMode LoadEnvironment()
        {
            try
            {
                // Determine matching environment
                string name;
                if (!string.IsNullOrWhiteSpace(_arguments.RunMode))
                    name = _arguments.RunMode;
                else if (_arguments.DbUpdate)
                    name = UpdateRunMode.RunModeName;
                else if (_arguments.DeveloperConsole || Environment.UserInteractive)
                    name = DeveloperConsole.RunmodeName;
                else
                    name = ServiceRunMode.RunModeName;

                // Prepare config
                var configDir = _arguments.ConfigDir;
                if (!Directory.Exists(configDir))
                    Directory.CreateDirectory(configDir);
                var configManager = _container.Resolve<IRuntimeConfigManager>();
                configManager.ConfigDirectory = configDir;

                // Prepare platform
                RuntimePlatform.SetPlatform();

                // Resolve environment
                var environment = _container.Resolve<IRunMode>(name);
                if (environment == null)
                    throw new ArgumentException("Failed to load environment. Name unknown!");
                environment.Setup(_arguments);

                // Init all components that require a start up
                var initializable = _container.ResolveAll<IInitializable>();
                foreach (var init in initializable)
                {
                    init.Initialize();
                }

                return environment;
            }
            catch (Exception ex)
            {
                CrashHandler.HandleCrash(null, new UnhandledExceptionEventArgs(ex, true));
                return null;
            }
        }

        /// <summary>
        /// Method to wrap all execution exceptions
        /// </summary>
        /// <param name="environment">Environment to execute</param>
        private static RuntimeErrorCode RunEnvironment(IRunMode environment)
        {
            try
            {
                return environment.Run();
            }
            catch (Exception ex)
            {
                CrashHandler.HandleCrash(null, new UnhandledExceptionEventArgs(ex, true));
                return RuntimeErrorCode.Error;
            }
        }
    }
}