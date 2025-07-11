// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Runtime.Modules;

namespace Moryx.ProcessData.Adapter.OrderManagement
{
    [ServerModuleConsole]
    internal class ModuleConsole : IServerModuleConsole
    {
        public void ExecuteCommand(string[] args, Action<string> outputStream)
        {
        }
    }
}
