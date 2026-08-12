// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.StateMachines;

namespace Moryx.Material.States;

internal abstract class MaterialContainerState(MaterialContainer context, StateBase.StateMap stateMap) :
    SyncStateBase<MaterialContainer>(context, stateMap)
{
    public abstract StateClassification Classification { get; }

    public abstract void Advance(StateInformation info);

    [StateDefinition(typeof(UninitializedState), IsInitial = true)]
    protected const int StateUninitialized = 10;

    [StateDefinition(typeof(RequestedState))]
    protected const int StateRequested = 20;

    [StateDefinition(typeof(InboundState))]
    protected const int StateInbound = 30;

    [StateDefinition(typeof(AvailableState))]
    protected const int StateAvailable = 40;

    [StateDefinition(typeof(OutboundState))]
    protected const int StateOutbound = 50;

    [StateDefinition(typeof(DeregisteredState))]
    protected const int StateDeregistered = 60;
}
