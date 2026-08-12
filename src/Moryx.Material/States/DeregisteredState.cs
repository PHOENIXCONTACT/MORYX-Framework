// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.States;

internal sealed class DeregisteredState(MaterialContainer context, StateMachines.StateBase.StateMap stateMap) :
    MaterialContainerState(context, stateMap)
{
    public override StateClassification Classification => StateClassification.Deregistered;

    public override void Advance(StateInformation info) => InvalidState();
}
