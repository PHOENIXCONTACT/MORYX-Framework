// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;

namespace Moryx.ProcessData.Adapter.OrderManagement;

[ServerModuleConsole]
internal class ModuleConsole : IServerModuleConsole
{
    public void ExecuteCommand(string[] args, Action<string> outputStream)
    {
    }
}