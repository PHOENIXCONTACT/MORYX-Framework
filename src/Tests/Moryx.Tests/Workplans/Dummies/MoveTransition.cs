// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Workplans;
using Moryx.Workplans.Transitions;

namespace Moryx.Tests.Workplans;

internal class MoveTransition : TransitionBase
{
    protected override void InputTokenAdded(object sender, IToken token)
    {
        Executing(() => MoveToken((IPlace)sender, Outputs[0], token));
    }

    public void PlaceUntaken(IPlace output, IToken token)
    {
        PlaceToken(output, token);
    }
}
