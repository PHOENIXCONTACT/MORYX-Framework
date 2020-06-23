// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using Moryx.Container;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel
{
    [Registration(LifeCycle.Singleton, typeof(ICommandHandler))]
    internal class ConsoleCommand : ICommandHandler
    {
        public IModuleManager ModuleManager { get; set; }

        /// <summary>
        /// Check if this 
        /// </summary>
        public bool CanHandle(string command)
        {
            return command == "man" || command == "exec" || command == "enter";
        }

        /// <summary>
        /// Handle the entered command
        /// </summary>
        public void Handle(string[] fullCommand)
        {
            if (fullCommand.Length < 2)
                return;

            var moduleName = fullCommand[1];
            var module = CommandHelper.GetByName(ModuleManager, moduleName);
            if (module == null)
                return;

            var console = module.Console;
            if (console == null)
                return;

            switch (fullCommand[0])
            {
                case "enter":
                    ModuleSpecificConsole(module);
                    break;
                default:
                    ModuleCommandExecutor(fullCommand, module);
                    break;
            }
        }

        /// <summary>
        /// Sends the commands to the module console
        /// </summary>
        private void ModuleCommandExecutor(string[] fullCommand, IServerModule module)
        {
            var console = module.Console;

            if (fullCommand[0] == "man")
                Console.Write(console.ExportDescription(DescriptionExportFormat.Console));
            else
                console.ExecuteCommand(fullCommand.Skip(2).ToArray(), Console.WriteLine);
        }

        /// <summary>
        /// Opens the module specific console 
        /// </summary>
        /// <param name="module">The module.</param>
        private void ModuleSpecificConsole(IServerModule module)
        {
            var console = module.Console;

            Console.Clear();
            Console.WriteLine("Welcome to the '{0}' module. Type 'help' to print the possible commands.", module.Name);
            Console.WriteLine();

            var command = string.Empty;
            while (command != "bye")
            {
                WriteModulePostString(module);
                command = CommandHelper.ReadCommand(ModuleManager);
                switch (command)
                {
                    case "help":
                        Console.WriteLine(console.ExportDescription(DescriptionExportFormat.Console));
                        break;
                    case "quit":
                    case "bye":
                    case "exit":
                        break;
                    default:
                        var parts = command.Split(' ');
                        Console.WriteLine();
                        console.ExecuteCommand(parts, Console.WriteLine);
                        Console.WriteLine();
                        break;
                }
            }

            Console.WriteLine();
            Console.WriteLine("Exiting module console...bye!");
            Console.WriteLine();
        }

        /// <summary>
        /// Writes the module post string. "TestModule > "
        /// </summary>
        /// <param name="module">The module.</param>
        private void WriteModulePostString(IServerModule module)
        {
            Console.Write("{0} > ", module.Name);
        }

        /// <summary>
        /// Print all valid commands
        /// </summary>
        public void ExportValidCommands(int pad)
        {
            Console.WriteLine("man <modulename>".PadRight(pad) + "Prints the modules man page.");
            Console.WriteLine("exec <modulename> cmd".PadRight(pad) + "Executes a command using the module console.");
            Console.WriteLine("enter <modulename>".PadRight(pad) + "Enter the modules console and execute commands from there.");
        }
    }
}
