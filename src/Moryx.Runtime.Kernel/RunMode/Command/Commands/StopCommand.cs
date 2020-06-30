// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Container;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel
{
    [Registration(LifeCycle.Singleton, typeof(ICommandHandler))]
    internal class StopCommand : ICommandHandler
    {
        public IModuleManager ModuleManager { get; set; }

        /// <summary>
        /// Check if this 
        /// </summary>
        public bool CanHandle(string command)
        {
            return command == "stop";
        }

        /// <summary>
        /// Handle the entered command
        /// </summary>
        public void Handle(string[] fullCommand)
        {
            if (fullCommand.Length == 1 || fullCommand[1] == "all")
                ModuleManager.StopModules();
            else
            {
                // Start of specific service
                var module = CommandHelper.GetByName(ModuleManager, fullCommand[1]);
                if (module != null)
                    ModuleManager.StopModule(module);
            }
        }

        /// <summary>
        /// Print all valid commands
        /// </summary>
        public void ExportValidCommands(int pad)
        {
            Console.WriteLine("stop [all]".PadRight(pad) + "Stops all services");
            Console.WriteLine("stop <servicename>".PadRight(pad) +
                              "Stops service named <servicename> and all depending services");
        }
    }
}
