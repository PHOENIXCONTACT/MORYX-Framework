using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Marvin.Logging;
using Marvin.Runtime.Modules;

namespace Marvin.Runtime.Kernel
{
    internal class ModuleStarter : ModuleManagerComponent, IModuleStarter
    {
        private readonly IModuleDependencyManager _dependencyManager;
        private readonly IModuleLogger _logger;
        private readonly ModuleManagerConfig _config;

        public ModuleStarter(IModuleDependencyManager dependencyManager, IModuleLogger logger, ModuleManagerConfig config)
        {
            _dependencyManager = dependencyManager;
            _logger = logger;
            _config = config;
        }

        /// <inheritdoc />
        public void Start(IServerModule plugin)
        {
            plugin.Initialize();
            StartModule(plugin);   
        }

        /// <inheritdoc />
        public void StartAll()
        {
            foreach (var module in AllModules())
            {
                module.Initialize();
            }

            // Find root server modules and convert all others to waiting services
            var depTree = _dependencyManager.GetDependencyEvalutaion().FullTree;
            foreach (var root in depTree.RootModules.Where(ShouldBeStarted))
            {
                ConvertBranch(root);
            }
            foreach (var module in depTree.RootModules.Where(ShouldBeStarted).Select(branch => branch.RepresentedModule))
            {
                StartModule(module);
            }
        }

        private void StartModule(IServerModule module)
        {
            // Now we check for any not running dependencies and start them
            var awaitingDependecies = _dependencyManager.GetDependencyBranch(module).Dependencies
                                     .Where(item => !item.RepresentedModule.State.HasFlag(ServerModuleState.Running))
                                     .Select(item => item.RepresentedModule).ToArray();
            if (awaitingDependecies.Any())
            {
                EnqueServiceAndStartDependencies(awaitingDependecies, module);
            }
            else
            {
                ThreadPool.QueueUserWorkItem(ExecuteModuleStart, module);
            }  
        }

        private void ExecuteModuleStart(object moduleObj)
        {
            var module = (IServerModule)moduleObj;
            // Should be caught by ServerModuleBase but better be safe than sorry 
            try
            {
                module.Start();
            }
            catch(Exception ex)
            {
                _logger.LogException(LogLevel.Error, ex, "Failed to start plugin {0}", module.Name);
            }
            // Forward result
            ModuleChangedState(module, module.State);
        }

        private void ModuleChangedState(IServerModule module, ServerModuleState newState)
        {
            // Check if it switched to running
            if (!newState.HasFlag(ServerModuleState.Running))
                return;

            // Now we start every service waiting on this service to return
            lock (WaitingModules)
            {
                if (!WaitingModules.ContainsKey(module))
                    return;

                // To increase boot speed we fork plugin start if more than one dependends was found
                foreach (var waitingModule in WaitingModules[module].ToArray())
                {
                    WaitingModules[module].Remove(waitingModule);
                    StartModule(waitingModule);
                }
                // We remove this service for now after we started every dependend
                WaitingModules.Remove(module);
            }
        }

        private void ConvertBranch(IModuleDependency branch)
        {
            foreach (var dependend in branch.Dependends.Where(ShouldBeStarted))
            {
                AddWaitingService(branch.RepresentedModule, dependend.RepresentedModule);
                ConvertBranch(dependend);
            }
        }

        private void EnqueServiceAndStartDependencies(IEnumerable<IServerModule> dependencies, IServerModule waitingService)
        {
            foreach (var dependency in dependencies)
            {
                AddWaitingService(dependency, waitingService);
                Start(dependency);
            }
        }

        private bool ShouldBeStarted(IModuleDependency plugin)
        {
            var conf = _config.GetOrCreate(plugin.RepresentedModule.Name);
            var result = conf.StartBehaviour == ModuleStartBehaviour.Auto || plugin.Dependends.Any(ShouldBeStarted);
            return result;
        }
    }
}
