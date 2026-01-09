// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans.Transitions;

/// <summary>
/// Transition to split execution flow
/// </summary>
internal sealed class SplitTransition : TransitionBase
{
    protected override void InputTokenAdded(object sender, IToken token)
    {
        Executing(delegate
        {
            ((IPlace)sender).Remove(token);
            // Split input into output
            foreach (var output in Outputs)
            {
                output.Add(new SplitToken(token));
            }
        });
    }
}