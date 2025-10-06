// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel
{
    internal abstract class ModuleManagerComponent
    {
        /// <summary>
        /// Delegate to get a copy of the module list
        /// </summary>
        public IReadOnlyList<IServerModule> AvailableModules { get; set; }

        /// <summary>
        /// Global dictionary of modules awaiting the start of other modules
        /// </summary>
        public IDictionary<IServerModule, ICollection<IServerModule>> WaitingModules { get; set; }

        protected void AddWaitingService(IServerModule dependency, IServerModule dependent)
        {
            lock (WaitingModules)
            {
                if (WaitingModules.ContainsKey(dependency))
                {
                    if (!WaitingModules[dependency].Contains(dependent))
                        WaitingModules[dependency].Add(dependent);
                }
                else
                {
                    WaitingModules[dependency] = new List<IServerModule> { dependent };
                }
            }
        }
    }
}
