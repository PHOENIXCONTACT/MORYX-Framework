// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using Moryx.Container;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel
{
    [Registration(LifeCycle.Singleton, typeof(ICommandHandler))]
    internal class ReincConfCommand : ICommandHandler
    {
        public IModuleManager ModuleManager { get; set; }

        /// <summary>
        /// Check if this 
        /// </summary>
        public bool CanHandle(string command)
        {
            return command == "reinc" | command == "conf";
        }

        /// <summary>
        /// Handle the entered command
        /// </summary>
        public void Handle(string[] fullCommand)
        {
            if (fullCommand.Length < 2)
            {
                Console.WriteLine("Service name must be specified!");
                return;
            }

            if (fullCommand[0] == "reinc")
            {
                // Reincarnate of specific service
                var module = CommandHelper.GetByName(ModuleManager, fullCommand[1]);
                if (module != null)
                    ModuleManager.ReincarnateModule(module);
            }
            if (fullCommand[0] == "conf")
            {
                // Confirm possible errors
                var module = CommandHelper.GetByName(ModuleManager, fullCommand[1]);
                foreach (var notification in module.Notifications.ToArray())
                {
                    module.AcknowledgeNotification(notification);
                }
                // Invoke init - this will get failed modules back to ready
                ModuleManager.InitializeModule(module);
            }
        }

        /// <summary>
        /// Print all valid commands
        /// </summary>
        public void ExportValidCommands(int pad)
        {
            Console.WriteLine("reinc <modulename>".PadRight(pad) +
                              "Reincarnates failed service named <servicename>");
            Console.WriteLine("conf <modulename>".PadRight(pad) +
                              "Confirm service named <servicename>. Services in state Warning return to running.");
            Console.WriteLine(string.Empty.PadRight(pad) + "Failed services to Ready state.");
        }
    }
}
