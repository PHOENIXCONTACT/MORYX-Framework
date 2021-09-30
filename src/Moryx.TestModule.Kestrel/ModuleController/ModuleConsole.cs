// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Runtime.Modules;

namespace Moryx.TestModule.Kestrel
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
