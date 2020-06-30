// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel
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
