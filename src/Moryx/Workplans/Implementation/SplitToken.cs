// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans;

/// <summary>
/// Token representing a split execution
/// </summary>
internal class SplitToken : IToken
{
    ///
    public string Name => $"{Original.Name}-Partial";

    public IToken Original { get; }

    public SplitToken(IToken original)
    {
        Original = original;
    }
}