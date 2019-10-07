using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLine;
using Marvin.Container;
using Marvin.Model;
using Marvin.Modules;
using Marvin.Runtime.Configuration;
using Marvin.Runtime.Container;
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
        /// Creates an instance of the <see cref="HeartOfGold"/>
        /// </summary>
        public HeartOfGold(string[] args)
        {
            _args = args;

            if (_args.Length == 0)
                _args = new[] { DeveloperConsoleOptions.VerbName };
        }

        /// <summary>
        /// Starts the runtime
        /// </summary>
        public RuntimeErrorCode Run()
        {
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
            var loadResult = LoadEnvironment(out var env);
            var returnCode = RuntimeErrorCode.NoError;
            switch (loadResult)
            {
                case EnvironmentLoadResult.Error:
                    returnCode = RuntimeErrorCode.Error;
                    break;
                case EnvironmentLoadResult.HelpRequested:
                    returnCode = RuntimeErrorCode.NoError;
                    break;
                case EnvironmentLoadResult.BadVerb:
                    returnCode = RuntimeErrorCode.Error;
                    break;
                case EnvironmentLoadResult.Success:
                    returnCode = RunEnvironment(env);
                    break;
            }

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

            return returnCode;
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
        private EnvironmentLoadResult LoadEnvironment(out IRunMode runMode)
        {
            EnvironmentInfo env = null;
            var loadResult = EnvironmentLoadResult.Error;

            try
            {
                var runModes = _container.ResolveAll<IRunMode>();
                var runModeMap = (from existing in runModes
                    let runModeAttr = existing.GetType().GetCustomAttribute<RunModeAttribute>()
                    select new EnvironmentInfo
                    {
                        RunMode = existing,
                        OptionType = runModeAttr.OptionType
                    }).ToArray();

                var optionTypes = runModeMap.Select(r => r.OptionType).ToArray();
                Parser.Default.ParseArguments(_args, optionTypes)
                    .WithParsed(delegate(object parsed)
                    {
                        // Select run mode
                        env = runModeMap.First(m => m.OptionType == parsed.GetType());
                        env.Options = (RuntimeOptions) parsed;
                        loadResult = EnvironmentLoadResult.Success;
                    })
                    .WithNotParsed(delegate(IEnumerable<Error> errors)
                    {
                        var errorArr = errors.ToArray();
                        if (errorArr.Any(e => e.Tag == ErrorType.HelpRequestedError || e.Tag == ErrorType.HelpVerbRequestedError))
                        {
                            loadResult = EnvironmentLoadResult.HelpRequested;
                            return;
                        }

                        if (errorArr.Any(e => e.Tag == ErrorType.BadVerbSelectedError || e.Tag == ErrorType.NoVerbSelectedError))
                        {
                            loadResult = EnvironmentLoadResult.BadVerb;
                            return;
                            //throw new ArgumentException("Failed to load environment. Name unknown!");
                        }

                        loadResult = EnvironmentLoadResult.Error;
                    });


                if (env != null && loadResult == EnvironmentLoadResult.Success)
                {
                    // Prepare config
                    var configDir = env.Options.ConfigDir;
                    if (!Directory.Exists(configDir))
                        Directory.CreateDirectory(configDir);
                    var configManager = _container.Resolve<IRuntimeConfigManager>();
                    configManager.ConfigDirectory = configDir;

                    // Setup environment
                    env.RunMode.Setup(env.Options);

                    // Prepare platform
                    RuntimePlatform.SetPlatform();

                    // Init all components that require a start up
                    var initializable = _container.ResolveAll<IInitializable>();
                    foreach (var init in initializable)
                    {
                        init.Initialize();
                    }
                }
            }
            catch (Exception ex)
            {
                loadResult = EnvironmentLoadResult.Error;
                CrashHandler.HandleCrash(null, new UnhandledExceptionEventArgs(ex, true));
            }
            finally
            {
                runMode = env?.RunMode;
            }

            return loadResult;
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

        private class EnvironmentInfo
        {
            public IRunMode RunMode { get; set; }

            public Type OptionType { get; set; }

            public RuntimeOptions Options { get; set; }
        }

        private enum EnvironmentLoadResult
        {
            Error,
            Success,
            HelpRequested,
            BadVerb
        }
    }
}