using System;
using Marvin.Container;
using Marvin.Runtime.Modules;

namespace Marvin.Runtime.HeartOfGold
{
    [Registration(LifeCycle.Singleton, typeof(ICommandHandler))]
    internal class StartCommand : ICommandHandler
    {
        public IModuleManager ModuleManager { get; set; }

        /// <summary>
        /// Check if this 
        /// </summary>
        public bool CanHandle(string command)
        {
            return command == "start";
        }

        /// <summary>
        /// Handle the entered command
        /// </summary>
        public void Handle(string[] fullCommand)
        {
            if (fullCommand.Length == 1 || fullCommand[1] == "all")
                ModuleManager.StartModules();
            else
            {
                // Start of specific service
                var module = CommandHelper.GetByName(ModuleManager, fullCommand[1]);
                if (module != null)
                    ModuleManager.StartModule(module);
            }
        }

        /// <summary>
        /// Print all valid commands
        /// </summary>
        public void ExportValidCommands(int pad)
        {
            Console.WriteLine("start [all]".PadRight(pad) + "Starts all services");
            Console.WriteLine("start <servicename>".PadRight(pad) +
                              "Starts service named <servicename> and its dependencies");
        }
    }
}
