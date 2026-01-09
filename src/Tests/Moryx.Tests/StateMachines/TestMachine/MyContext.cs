// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.StateMachines;

namespace Moryx.Tests;

public class MyContext : IStateContext
{
    public MyStateBase State { get; private set; }

    public bool AEntered { get; set; }

    public bool BExited { get; set; }

    public bool AtoBTriggered { get; set; }

    public bool BtoCTriggered { get; set; }

    public bool CtoATriggered { get; set; }

    internal void HandleAtoB()
    {
        AtoBTriggered = true;
    }

    internal void HandleBtoC()
    {
        BtoCTriggered = true;
    }

    internal void HandleCtoA()
    {
        CtoATriggered = true;
    }

    void IStateContext.SetState(StateBase state)
    {
        State = (MyStateBase)state;
    }
}