// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG

namespace Moryx.ControlSystem.Processes.Endpoints;
internal class ProcessHolderGroupChangedEventArg : EventArgs
{
    public ProcessHolderGroupModel Group { get; }

    public ProcessHolderGroupChangedEventArg(ProcessHolderGroupModel group)
    {
        Group = group;
    }
}
