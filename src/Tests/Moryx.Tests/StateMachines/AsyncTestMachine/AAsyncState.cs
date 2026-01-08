// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading;
using System.Threading.Tasks;

namespace Moryx.Tests.AsyncTestMachine
{
    internal sealed class AAsyncState : MyAsyncStateBase
    {
        public AAsyncState(MyAsyncContext context, StateMap stateMap) : base(context, stateMap)
        {
        }

        public override Task OnEnterAsync(CancellationToken cancellationToken)
        {
            Context.AEntered = true;
            return Task.CompletedTask;
        }

        public override Task InitialAsync()
        {
            return Task.CompletedTask;
        }

        public override async Task AtoBAsync()
        {
            await NextStateAsync(StateB);
            await Context.HandleAtoB();
        }
    }
}
