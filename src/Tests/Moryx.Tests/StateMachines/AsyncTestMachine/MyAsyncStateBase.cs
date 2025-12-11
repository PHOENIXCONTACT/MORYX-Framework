// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading.Tasks;
using Moryx.StateMachines;

namespace Moryx.Tests.AsyncTestMachine
{
    public abstract class MyAsyncStateBase : AsyncStateBase<MyAsyncContext>
    {
        protected MyAsyncStateBase(MyAsyncContext context, StateMap stateMap) : base(context, stateMap)
        {

        }

        public virtual Task InitialAsync()
        {
            return InvalidStateAsync();
        }

        public virtual Task AtoBAsync()
        {
            return InvalidStateAsync();
        }

        public virtual Task BtoCAsync()
        {
            return InvalidStateAsync();

        }

        public virtual Task CtoAAsync()
        {
            return InvalidStateAsync();
        }

        [StateDefinition(typeof(AAsyncState), IsInitial = true)]
        public const int StateA = 10;

        [StateDefinition(typeof(BAsyncState))]
        public const int StateB = 20;

        [StateDefinition(typeof(CAsyncState))]
        public const int StateC = 30;
    }
}
