// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading;
using System.Threading.Tasks;

namespace Moryx.Tests.AsyncTestMachine
{
    internal sealed class BAsyncState : MyAsyncStateBase
    {
        public BAsyncState(MyAsyncContext context, StateMap stateMap) : base(context, stateMap)
        {
        }

        public override Task OnExitAsync(CancellationToken cancellationToken)
        {
            Context.BExited = true;
            return Task.CompletedTask;
        }

        public override async Task BtoCAsync()
        {
            await NextStateAsync(StateC);
            await Context.HandleBtoC();
        }
    }
}
