using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Marvin.Container;
using Marvin.Modules;
using Marvin.Runtime.ModuleManagement;
using Marvin.Tools;

namespace Marvin.Runtime.HeartOfGold
{
    [Registration(LifeCycle.Singleton, typeof(ICommandHandler))]
    internal class PrintCommand : ICommandHandler
    {
        public IModuleManager ModuleManager { get; set; }

        /// <summary>
        /// Check if this 
        /// </summary>
        public bool CanHandle(string command)
        {
            return command == "print" | command == "status";
        }

        /// <summary>
        /// Handle the entered command
        /// </summary>
        public void Handle(string[] fullCommand)
        {
            if (fullCommand.Length == 1 || fullCommand[1] == "all")
            {
                foreach (var module in ModuleManager.AllModules)
                {
                    PrintServerModule(module, null);
                }
            }
            else if (fullCommand.Length >= 2)
            {
                var module = CommandHelper.GetByName(ModuleManager, fullCommand[1]);
                if (module != null)
                    PrintServerModule(module, fullCommand.Skip(2));
            }
            else
            {
                Console.WriteLine("Invalid syntax for \"print\" command!");
            }
        }

        private void PrintServerModule(IServerModule module, IEnumerable<string> printOptions)
        {
            var versionAttribute = module.GetType().Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();

            Console.Write("Service: " + module.Name.PadRight(30));
            CommandHelper.PrintState(module.State.Current, false, 17);
            Console.WriteLine("Version: " + (versionAttribute == null ? "N/A" : versionAttribute.Version));

            if (printOptions == null)
                return;

            Console.WriteLine();
            foreach (var printOption in printOptions)
            {
                switch (printOption)
                {
                    case "-e":
                        var predicate = new Func<IModuleNotification, bool>(n => n.Type == NotificationType.Failure || n.Type == NotificationType.Warning);
                        if (!module.Notifications.Any(predicate))
                            break;
                        Console.WriteLine("Exception stack for " + module.Name + ":");
                        Console.WriteLine(ExceptionPrinter.Print(module.Notifications.OrderBy(n => n.Timestamp).Last(predicate).Exception));
                        break;
                    case "-d":
                        Console.WriteLine("Dependencies of " + module.Name + ":");
                        foreach (var startDependency in ModuleManager.StartDependencies(module))
                        {
                            Console.WriteLine(" Service: " + startDependency.Name.PadRight(29) + "State: " +
                                              startDependency.State.Current);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Print all valid commands
        /// </summary>
        public void ExportValidCommands(int pad)
        {
            Console.WriteLine("print [all]".PadRight(pad) + "Prints current state of all services");
            Console.WriteLine("print <servicename>".PadRight(pad) +
                              "Prints current state of service named <servicename>.");
            Console.WriteLine("  - options: [-e] [-d]");
            Console.WriteLine("[-e]".PadLeft(pad - 1).PadRight(pad) + "shows exception for this service.");
            Console.WriteLine("[-d]".PadLeft(pad - 1).PadRight(pad) + "print all dependencies");
            Console.WriteLine();
        }
    }
}
