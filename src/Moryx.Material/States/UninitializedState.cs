// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.States;

internal class UninitializedState(MaterialContainer context, StateMachines.StateBase.StateMap stateMap) :
    MaterialContainerState(context, stateMap)
{
    public override StateClassification Classification => StateClassification.Uninitialized;

    /// <summary>
    /// Recover the state of material containers after a restart of the resource management
    /// </summary>
    public override void OnEnter() => Advance(Context.StateInformation);

    public override void Advance(StateInformation? info)
    {
        switch (Context.StateInformation)
        {
            case RequestedStateInformation:
                NextState(StateRequested);
                return;
            case InboundStateInformation:
                NextState(StateInbound);
                return;
            case AvailableStateInformation:
                NextState(StateAvailable);
                return;
            case OutboundStateInformation:
                NextState(StateOutbound);
                return;
            case DeregisteredStateInformation:
                NextState(StateDeregistered);
                return;
            default:
                return;
        }
    }
}

