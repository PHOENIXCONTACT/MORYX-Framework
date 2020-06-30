// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.StateMachines;

namespace Marvin.Tests
{
    public abstract class MyStateBase : StateBase<MyContext>
    {
        protected MyStateBase(MyContext context, StateMap stateMap) : base(context, stateMap)
        {
            
        }

        public virtual void Initial()
        {
            InvalidState();
        }

        public virtual void AtoB()
        {
            InvalidState();
        }

        public virtual void BtoC()
        {
            InvalidState();
        }

        public virtual void CtoA()
        {
            InvalidState();
        }

        [StateDefinition(typeof(AState), IsInitial = true)]
        public const int StateA = 10;

        [StateDefinition(typeof(BState))]
        public const int StateB = 20;

        [StateDefinition(typeof(CState))]
        public const int StateC = 30;
    }
}
