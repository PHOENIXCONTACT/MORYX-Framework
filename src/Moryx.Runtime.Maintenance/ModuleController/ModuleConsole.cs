// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Container;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Maintenance
{
    [ServerModuleConsole]
    internal class ModuleConsole : IServerModuleConsole
    {
        public IContainer Container { get; set; }

        public void ExecuteCommand(string[] args, Action<string> outputStream)
        {
            outputStream("Maintenance does not support any console commands!");
        }
    }
}
