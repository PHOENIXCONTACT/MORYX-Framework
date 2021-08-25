// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ComponentModel;
using Moryx.Runtime.Modules;
using Moryx.Serialization;
using IContainer = Moryx.Container.IContainer;

namespace Moryx.TestModule
{
    [ServerModuleConsole]
    internal class ModuleConsole : IServerModuleConsole
    {
        public void ExecuteCommand(string[] args, Action<string> outputStream)
        {
            outputStream("The TestModule does not provide any commands!");
        }
    }
}
