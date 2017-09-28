using System;
using System.IO;
using System.Reflection;
using Marvin.Container;
using Marvin.Model;
using Marvin.Modules;
using Marvin.Modules.Server;
using Marvin.Tools;
using Marvin.Runtime.Configuration;
using Marvin.Runtime.Container;
using Marvin.Runtime.Requirements;

namespace Marvin.Runtime.HeartOfGold
{
    /// <summary>
    /// Static programm class
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// MEF container
        /// </summary>
        private static IContainer _container;
        /// <summary>
        /// Arguments passed to this instance
        /// </summary>
        private static RuntimeArguments _arguments;

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        public static int Main(string[] args)
        {
            // Look for help command
            _arguments = RuntimeArguments.BuildArgumentDict(args);
            if (_arguments.HasArgument("h", "help"))
            {
                HelpPrinter.Print();
                return 0;
            }

            // Set working directory to location of this exe
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

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
            var result = env == null ? 2 : RunEnvironment(env);

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
        private static IContainer CreateContainer()
        {
            var container = new GlobalContainer();

            // Register runtimes
            container.ExecuteInstaller(new AutoInstaller(typeof(Program).Assembly));

            // Load additional runtimes
            container.LoadComponents<IRunmode>();

            // Load kernel and core modules
            container.LoadComponents<object>(type => type.GetCustomAttribute<KernelComponentAttribute>() != null);
            container.LoadComponents<IServerModule>(module => module.GetCustomAttribute<ServerModuleAttribute>() != null);

            // Load models
            container.LoadComponents<IUnitOfWorkFactory>();
            container.LoadComponents<IModelConfigurator>();

            // Load requirements check plugins
            container.LoadComponents<IRequirementsCheck>();

            // Load user modules
            container.LoadComponents<IServerModule>();

            return container;
        }

        /// <summary>
        /// Method to wrap all boot exceptions
        /// </summary>
        /// <param name="args">Arguments</param>
        private static IRunmode LoadEnvironment(RuntimeArguments args)
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
                var config = configManager.GetConfiguration<ProductConfig>();
                RuntimePlatform.SetPlatform(config.ProductName, new Version(config.Version));

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
        private static int RunEnvironment(IRunmode environment)
        {
            try
            {
                return environment.Run();
            }
            catch (Exception ex)
            {
                CrashHandler.HandleCrash(null, new UnhandledExceptionEventArgs(ex, true));
                return 2;
            }
        }
    }
}
