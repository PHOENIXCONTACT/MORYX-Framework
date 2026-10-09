// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Text.RegularExpressions;
using Moryx.Container;

namespace Moryx.Samples.Strategies;

/// <summary>
/// Strategy removing white spaces with a regular expression
/// </summary>
[Plugin(LifeCycle.Singleton, typeof(IWhiteSpaceRemovingStrategy), Name = StrategyName)]
public class RegexWhiteSpaceRemovingStrategy : IWhiteSpaceRemovingStrategy
{
    /// <summary>
    /// Name of the strategy used for selection
    /// </summary>
    public const string StrategyName = "Regex Remover";

    /// <inheritdoc />
    public string DropSpaces(string s) => Regex.Replace(s, @"\s+", "");
}
