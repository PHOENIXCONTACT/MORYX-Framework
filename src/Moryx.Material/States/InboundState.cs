// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.States;

internal sealed class InboundState(MaterialContainer context, StateMachines.StateBase.StateMap stateMap) :
    MaterialContainerState(context, stateMap)
{
    public override StateClassification Classification => StateClassification.Inbound;

    public override void Advance(StateInformation info)
    {
        switch (info)
        {
            case AvailableStateInformation:
                NextState(StateAvailable);
                break;
            case DeregisteredStateInformation:
                NextState(StateDeregistered);
                break;
            default:
                InvalidState();
                return;
        }
        Context.StateInformation = info;
    }
}
