using System;
using System.Collections.Generic;
using Marvin.Runtime.Modules;

namespace Marvin.Runtime.Kernel
{
    internal abstract class ModuleManagerComponent : IModuleManagerComponent
    {
        /// <summary>
        /// Global dictionary of modules awaiting the start of other modules
        /// </summary>
        public IDictionary<IServerModule, ICollection<IServerModule>> WaitingModules { get; set; }

        /// <summary>
        /// Delegate to get a copy of the module list
        /// </summary>
        public Func<IEnumerable<IServerModule>> AllModules { get; set; }

        protected void AddWaitingService(IServerModule dependency, IServerModule dependend)
        {
            lock (WaitingModules)
            {
                if (WaitingModules.ContainsKey(dependency))
                {
                    if (!WaitingModules[dependency].Contains(dependend))
                        WaitingModules[dependency].Add(dependend);
                }
                else
                {
                    WaitingModules[dependency] = new List<IServerModule> { dependend };
                }
            }
        }
    }
}