// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;

namespace Moryx.ProcessData.Adapter.NotificationPublisher;

[ServerModuleConsole]
internal sealed class ModuleConsole : IServerModuleConsole
{
    public static void ExecuteCommand(string[] args, Action<string> outputStream)
    {
    }
}
