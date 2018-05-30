using System;
using System.IO;
using System.Reflection;
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
            _arguments = RuntimeArguments.BuildArgumentDict(_args);

            // Look for help command
            if (_arguments.HasArgument("h", "help"))
            {
                HelpPrinter.Print();
                return RuntimeErrorCode.NoError;
            }

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
            var env = LoadEnvironment(_arguments);
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
            container.LoadComponents<IRunmode>();

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
        /// <param name="args">Arguments</param>
        private IRunmode LoadEnvironment(RuntimeArguments args)
        {
            try
            {
                // Determine matching environment
                string name;
                if (args.HasValue("r"))
                    name = args["r"];
                else if (args.HasArgument("dbUpdate"))
                    name = UpdateRunmode.RunModeName;
                else if (args.HasArgument("d") || Environment.UserInteractive)
                    name = DeveloperConsole.RunmodeName;
                else
                    name = ServiceRunMode.RunModeName;

                // Prepare config
                var configDir = args["c"] ?? "Config";
                if (!Directory.Exists(configDir))
                    Directory.CreateDirectory(configDir);
                var configManager = _container.Resolve<IRuntimeConfigManager>();
                configManager.ConfigDirectory = configDir;

                // Prepare platform
                RuntimePlatform.SetPlatform();

                // Resolve environment
                var environment = _container.Resolve<IRunmode>(name);
                if (environment == null)
                    throw new ArgumentException("Failed to load environment. Name unknown!");
                environment.Setup(args);

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
        private static RuntimeErrorCode RunEnvironment(IRunmode environment)
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