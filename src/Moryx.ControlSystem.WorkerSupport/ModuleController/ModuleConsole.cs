using System;
using Moryx.Runtime.Modules;

namespace Moryx.ControlSystem.WorkerSupport
{
    [ServerModuleConsole]
    internal class ModuleConsole : IServerModuleConsole
    {
        public void ExecuteCommand(string[] args, Action<string> outputStream)
        {
        }
    }
}