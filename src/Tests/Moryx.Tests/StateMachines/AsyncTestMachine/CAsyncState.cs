// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading.Tasks;

namespace Moryx.Tests.AsyncTestMachine;

internal sealed class CAsyncState : MyAsyncStateBase
{
    public CAsyncState(MyAsyncContext context, StateMap stateMap) : base(context, stateMap)
    {
    }

    public override async Task CtoAAsync()
    {
        await NextStateAsync(StateA);
        await Context.HandleCtoA();
    }
}