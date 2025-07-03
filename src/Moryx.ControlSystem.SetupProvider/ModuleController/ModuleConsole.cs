using System;
using Moryx.Runtime.Modules;

namespace Moryx.ControlSystem.SetupProvider
{
    [ServerModuleConsole]
    internal class ModuleConsole : IServerModuleConsole
    {

        public void ExecuteCommand(string[] args, Action<string> outputStream)
        {
        }
    }
}