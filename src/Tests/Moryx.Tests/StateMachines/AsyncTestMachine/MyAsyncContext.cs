// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading.Tasks;
using Moryx.StateMachines;

namespace Moryx.Tests.AsyncTestMachine
{
    public class MyAsyncContext : IStateContext
    {
        public MyAsyncStateBase State { get; private set; }

        public bool AEntered { get; set; }

        public bool BExited { get; set; }

        public bool AtoBTriggered { get; set; }

        public bool BtoCTriggered { get; set; }

        public bool CtoATriggered { get; set; }

        internal Task HandleAtoB()
        {
            AtoBTriggered = true;
            return Task.CompletedTask;
        }

        internal Task HandleBtoC()
        {
            BtoCTriggered = true;
            return Task.CompletedTask;
        }

        internal Task HandleCtoA()
        {
            CtoATriggered = true;
            return Task.CompletedTask;
        }

        public void SetState(StateBase state)
        {
            State = (MyAsyncStateBase)state;
        }
    }
}
