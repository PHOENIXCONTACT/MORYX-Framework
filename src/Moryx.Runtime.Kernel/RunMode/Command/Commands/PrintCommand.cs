// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moryx.Container;
using Moryx.Modules;
using Moryx.Notifications;
using Moryx.Runtime.Modules;
using Moryx.Tools;

namespace Moryx.Runtime.Kernel
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

            Console.Write("Module: " + module.Name);

            var warningCount = module.Notifications.Count(n => n.Severity == Severity.Warning);
            var errorCount = module.Notifications.Count(n => n.Severity >= Severity.Error);

            Console.Write(" (");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(warningCount);
            Console.ResetColor();
            Console.Write(" | ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(errorCount);
            Console.ResetColor();
            Console.Write(")".PadRight(20));

            CommandHelper.PrintState(module.State, false, 17);
            Console.WriteLine("Version: " + (versionAttribute == null ? "N/A" : versionAttribute.Version));

            if (printOptions == null)
                return;

            Console.WriteLine();
            foreach (var printOption in printOptions)
            {
                switch (printOption)
                {
                    case "-e":
                        var predicate = new Func<IModuleNotification, bool>(n => n.Severity >= Severity.Error || n.Severity == Severity.Warning);
                        if (!module.Notifications.Any(predicate))
                            break;
                        Console.WriteLine("Notifications for " + module.Name + ":");

                        var relevantNotifications = module.Notifications.Where(predicate).OrderBy(n => n.Timestamp);
                        foreach (var notification in relevantNotifications)
                            PrintNotification(notification);
                        break;
                    case "-d":
                        Console.WriteLine("Dependencies of " + module.Name + ":");
                        foreach (var startDependency in ModuleManager.StartDependencies(module))
                        {
                            Console.WriteLine(" Module: " + startDependency.Name.PadRight(29) + "State: " +
                                              startDependency.State);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Prints a module notiication to the console
        /// </summary>
        private static void PrintNotification(IModuleNotification notification)
        {
            var dashes = Enumerable.Repeat("-", Console.WindowWidth);
            var line = dashes.Aggregate((sum, next) => sum + next);
            Console.WriteLine(line);

            Console.WriteLine("Timestamp: " + notification.Timestamp);

            Console.Write("Severity: ");

            if (notification.Severity == Severity.Warning)
                Console.ForegroundColor = ConsoleColor.Yellow;

            if (notification.Severity >= Severity.Error)
                Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(notification.Severity);
            Console.ResetColor();

            Console.WriteLine("Message: " + notification.Message);
            Console.Write("Exception: ");

            if (notification.Exception != null)
                Console.WriteLine(Environment.NewLine + ExceptionPrinter.Print(notification.Exception));
        }

        /// <summary>
        /// Print all valid commands
        /// </summary>
        public void ExportValidCommands(int pad)
        {
            Console.WriteLine("print [all]".PadRight(pad) + "Prints current state of all services");
            Console.WriteLine("print <module name>".PadRight(pad) +
                              "Prints current state of service named <module name>.");
            Console.WriteLine("  - options: [-e] [-d]");
            Console.WriteLine("[-e]".PadLeft(pad - 1).PadRight(pad) + "shows notifications for this module.");
            Console.WriteLine("[-d]".PadLeft(pad - 1).PadRight(pad) + "print all dependencies");
            Console.WriteLine();
        }
    }
}
