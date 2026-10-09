// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;

namespace Moryx.Samples.Strategies;

/// <summary>
/// Strategy removing white spaces with LINQ
/// </summary>
[Plugin(LifeCycle.Singleton, typeof(IWhiteSpaceRemovingStrategy), Name = StrategyName)]
public class LinqWhiteSpaceRemovingStrategy : IWhiteSpaceRemovingStrategy
{
    /// <summary>
    /// Name of the strategy used for selection
    /// </summary>
    public const string StrategyName = "Linq Remover";

    /// <inheritdoc />
    public string DropSpaces(string s) => string.Concat(s.Where(c => !char.IsWhiteSpace(c)));
}
