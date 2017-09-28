using System;
using System.Collections.Generic;
using Marvin.Modules.Server;

namespace Marvin.Runtime.Kernel.ModuleManagement
{
    internal interface IModuleManagerComponent
    {
        /// <summary>
        /// Global dictionary of modules awaiting the start of other plugins
        /// </summary>
        IDictionary<IServerModule, ICollection<IServerModule>> WaitingModules { get; set; }

        /// <summary>
        /// Delegate to get a copy of the module list
        /// </summary>
        Func<IEnumerable<IServerModule>> AllModules { get; set; }
    }
}
