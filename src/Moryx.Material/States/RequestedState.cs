// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.States;

internal class RequestedState(MaterialContainer context, StateMachines.StateBase.StateMap stateMap) :
    MaterialContainerState(context, stateMap)
{
    public override StateClassification Classification => StateClassification.Requested;

    public override void Advance(StateInformation info)
    {
        switch (Context.StateInformation)
        {
            case InboundStateInformation:
                NextState(StateInbound);
                return;
            case AvailableStateInformation:
                NextState(StateAvailable);
                return;
            case DeregisteredStateInformation:
                NextState(StateDeregistered);
                return;
            default:
                InvalidState();
                return;
        }
    }
}
