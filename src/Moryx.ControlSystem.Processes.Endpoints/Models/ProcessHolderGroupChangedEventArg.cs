// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.Processes.Endpoints;
internal class ProcessHolderGroupChangedEventArg : EventArgs
{
    public ProcessHolderGroupModel Group { get; }

    public ProcessHolderGroupChangedEventArg(ProcessHolderGroupModel group)
    {
        Group = group;
    }
}
