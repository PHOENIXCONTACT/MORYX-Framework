// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Samples.Strategies;

/// <summary>
/// Strategy to remove white spaces from a text
/// </summary>
public interface IWhiteSpaceRemovingStrategy
{
    /// <summary>
    /// Removes all white spaces from the given text
    /// </summary>
    string DropSpaces(string s);
}
