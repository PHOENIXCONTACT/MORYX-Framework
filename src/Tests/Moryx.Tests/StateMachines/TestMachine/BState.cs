// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Tests
{
    internal sealed class BState : MyStateBase
    {
        public BState(MyContext context, StateMap stateMap) : base(context, stateMap)
        {
        }

        public override void OnExit()
        {
            Context.BExited = true;
        }

        public override void BtoC()
        {
            NextState(StateC);
            Context.HandleBtoC();
        }
    }
}
