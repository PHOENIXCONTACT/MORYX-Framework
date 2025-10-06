// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Tests
{
    internal sealed class CState : MyStateBase
    {
        public CState(MyContext context, StateMap stateMap) : base(context, stateMap)
        {
        }

        public override void CtoA()
        {
            NextState(StateA);
            Context.HandleCtoA();
        }
    }
}
