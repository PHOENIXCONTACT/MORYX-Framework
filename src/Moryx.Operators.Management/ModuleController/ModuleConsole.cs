﻿// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;

namespace Moryx.Operators.Management;

[ServerModuleConsole]
internal class ModuleConsole : IServerModuleConsole
{
    public void ExecuteCommand(string[] args, Action<string> outputStream)
    {
    }
}
