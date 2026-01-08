// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading;
using System.Threading.Tasks;
using Moryx.StateMachines;

namespace Moryx.Tests.AsyncTestMachine
{
    public class MyAsyncContext : IAsyncStateContext
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

        public Task SetStateAsync(StateBase state, CancellationToken cancellationToken)
        {
            State = (MyAsyncStateBase)state;
            return Task.CompletedTask;
        }
    }
}
