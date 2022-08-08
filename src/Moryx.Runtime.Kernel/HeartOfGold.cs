// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLine;
using Microsoft.Extensions.Logging;
using Moryx.Container;
using Moryx.Modules;
using Moryx.Runtime.Configuration;
using Moryx.Runtime.Container;
using Moryx.Runtime.Modules;
using Moryx.Tools;

namespace Moryx.Runtime.Kernel
{
    /// <summary>
    /// Base kernel loader for the runtime
    /// </summary>
    public class HeartOfGold
    {
        private string[] _args;

        /// <summary>
        /// Current global container
        /// </summary>
        private IContainer _container;

        /// <inheritdoc />
        public IContainer GlobalContainer => _container;

        /// <summary>
        /// Loaded environment
        /// </summary>
        private IRunMode _env;

        /// <summary>
        /// Creates an instance of the <see cref="HeartOfGold"/>
        /// </summary>
        public HeartOfGold(string[] args)
        {
            PrepareArguments(args);

            // Set working directory to location of this exe
            // ReSharper disable once AssignNullToNotNullAttribute
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            // Load all assemblies from directory before accessing any other code
            AppDomainBuilder.LoadAssemblies();

            // This is necessary to enable assembly lookup from AppDomain
            AppDomain.CurrentDomain.AssemblyResolve += AppDomainBuilder.ResolveAssembly;

            // This will give a message to the user before crashing
            AppDomain.CurrentDomain.UnhandledException += CrashHandler.HandleCrash;

            // Prepare container
            _container = CreateContainer();
        }

        /// <inheritdoc />
        public int Load()
        {
            // Load and run environment
            var loadResult = LoadEnvironment(out var env);
            if (loadResult == EnvironmentLoadResult.Success)
                _env = env;
            return (int)loadResult;
        }

        /// <inheritdoc />
        public int Execute()
        {
            if (_env == null)
                throw new InvalidOperationException("Please load environment before execution!");

            var returnCode = RunEnvironment(_env);
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

            return (int)returnCode;
        }

        /// <summary>
        /// Starts the runtime
        /// </summary>
        [Obsolete("Use Load and Execute instead")]
        public RuntimeErrorCode Run()
        {
            // Load and run environment
            var loadResult = LoadEnvironment(out var env);
            var returnCode = RuntimeErrorCode.NoError;
            switch (loadResult)
            {
                case EnvironmentLoadResult.BadVerb:
                case EnvironmentLoadResult.NoVerb:
                case EnvironmentLoadResult.Error:
                    returnCode = RuntimeErrorCode.Error;
                    break;
                case EnvironmentLoadResult.HelpRequested:
                    returnCode = RuntimeErrorCode.NoError;
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

            // Install logging
            container.ExecuteInstaller(new LoggingInstaller());

            // Register local components
            container.ExecuteInstaller(new AutoInstaller(GetType().Assembly));

            // Load additional run modes
            container.LoadComponents<IRunMode>();

            // Load kernel and core modules
            container.LoadComponents<object>(type => type.GetCustomAttribute<KernelComponentAttribute>() != null);

            // Load server modules
            container.LoadComponents<IServerModule>();

            return container;
        }

        /// <summary>
        /// Selects and loads the environment by arguments of the heart of gold
        /// </summary>
        private EnvironmentLoadResult LoadEnvironment(out IRunMode runMode)
        {
            var runModes = _container.ResolveAll<IRunMode>();
            var runModeMap = (from existing in runModes
                              let runModeAttr = existing.GetType().GetCustomAttribute<RunModeAttribute>()
                              where runModeAttr != null
                              select new EnvironmentInfo
                              {
                                  RunMode = existing,
                                  OptionType = runModeAttr.OptionType
                              }).ToArray();

            var optionTypes = runModeMap.Select(r => r.OptionType).ToArray();

            EnvironmentInfo env = null;
            var loadResult = EnvironmentLoadResult.Error;
            try
            {
                Parser.Default.ParseArguments(_args, optionTypes)
                .WithParsed(delegate (object parsed)
                {
                    // Select run mode
                    env = runModeMap.First(m => m.OptionType == parsed.GetType());
                    env.Options = (RuntimeOptions)parsed;
                    loadResult = EnvironmentLoadResult.Success;
                })
                .WithNotParsed(errors => loadResult = EvaluateParserErrors(errors));

                if (loadResult == EnvironmentLoadResult.Success)
                    SetupEnvironment(env);
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
        /// Setups the loaded environment (config dir,  platform and initializables)
        /// </summary>
        private void SetupEnvironment(EnvironmentInfo env)
        {
            // Prepare config directory
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
            initializable.ForEach(i => i.Initialize());
        }

        /// <summary>
        /// Prepares the application arguments and adds the developer console verb if no verb was selected
        /// </summary>
        private void PrepareArguments(string[] args)
        {
            // The next lines adds the developer console verb if no verb was selected
            if (args.Length == 0)
            {
                _args = new[] { DeveloperConsoleOptions.VerbName };
            }
            else if (args.Length > 0
                     && !args[0].ToLower().Contains("help")
                     && (args[0].StartsWith("-") || args[0].StartsWith("--")))
            {
                _args = new string[args.Length + 1];
                _args[0] = DeveloperConsoleOptions.VerbName;
                args.CopyTo(_args, 1);
            }
            else
            {
                _args = args;
            }
        }

        private static EnvironmentLoadResult EvaluateParserErrors(IEnumerable<Error> errors)
        {
            EnvironmentLoadResult loadResult;
            var errorArr = errors.ToArray();
            if (errorArr.Any(e => e.Tag == ErrorType.HelpRequestedError || e.Tag == ErrorType.HelpVerbRequestedError))
            {
                loadResult = EnvironmentLoadResult.HelpRequested;
                return loadResult;
            }

            if (errorArr.Any(e => e.Tag == ErrorType.BadVerbSelectedError || e.Tag == ErrorType.NoVerbSelectedError))
            {
                loadResult = EnvironmentLoadResult.BadVerb;
                return loadResult;
            }

            if (errorArr.Any(e => e.Tag == ErrorType.NoVerbSelectedError))
            {
                loadResult = EnvironmentLoadResult.NoVerb;
                return loadResult;
            }

            loadResult = EnvironmentLoadResult.Error;
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
            Success,
            Error,
            HelpRequested,
            BadVerb,
            NoVerb,
        }
    }
}
