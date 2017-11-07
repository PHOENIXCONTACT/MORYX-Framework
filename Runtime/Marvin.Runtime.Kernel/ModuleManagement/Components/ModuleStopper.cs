using System;
using System.Linq;
using Marvin.Logging;

namespace Marvin.Runtime.Kernel.ModuleManagement
{
    internal class ModuleStopper : ModuleManagerComponent, IModuleStopper
    {
        private readonly IModuleDependencyManager _dependencyManager;
        private readonly IModuleLogger _logger;
        public ModuleStopper(IModuleDependencyManager dependencyManager, IModuleLogger logger)
        {
            _dependencyManager = dependencyManager;
            _logger = logger;
        }

        /// <summary>
        /// Stop this plugin and all required dependencies
        /// </summary>
        /// <param name="module"></param>
        public void Stop(IServerModule module)
        {
            // First we have to find all running modules that depend on this service
            var dependingServices = _dependencyManager.GetDependencyBranch(module).Dependends.Select(item => item.RepresentedModule);
            // Now we will stop all of them recursivly
            foreach (var dependingService in dependingServices.Where(dependend => dependend.State.Current.HasFlag(ServerModuleState.Running)
                                                                               || dependend.State.Current == ServerModuleState.Starting))
            {
                // We will enque the service to make sure it is restarted later on
                AddWaitingService(module, dependingService);
                Stop(dependingService);
            }

            ExecuteModuleStop(module);
        }


        private void ExecuteModuleStop(object moduleObj)
        {
            var module = (IServerModule)moduleObj;
            // Since stop is synchron we don't need an event
            try
            {
                module.Stop();
            }
            catch
            {
                Console.WriteLine("Failed to stop service <{0}>", module.Name);
            }
        }

        /// <summary>
        /// Stop all services
        /// </summary>
        public void StopAll()
        {
            // Detemine all leaves of the dependency tree
            foreach (var plugin in AllModules())
            {
                Stop(plugin);
            }
        }
    }
}
