// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Text;
using Moryx.Container;

namespace Moryx.Resources.Samples.Strategies;

/// <summary>
/// Strategy removing white spaces with a <see cref="StringBuilder"/>.
/// It is defined outside of the resource assembly and registered by the
/// <see cref="WhiteSpaceRemovingStrategyInitializer"/> of the <see cref="StrategyUsingResource"/>.
/// The class must be public to be found by <see cref="ContainerLoadComponentsExtension"/>.
/// </summary>
[Plugin(LifeCycle.Singleton, typeof(IWhiteSpaceRemovingStrategy), Name = "StringBuilder Remover")]
public class StringBuilderWhiteSpaceRemovingStrategy : IWhiteSpaceRemovingStrategy
{
    /// <inheritdoc />
    public string DropSpaces(string s)
    {
        var builder = new StringBuilder(s.Length);
        foreach (var c in s)
        {
            if (!char.IsWhiteSpace(c))
                builder.Append(c);
        }

        return builder.ToString();
    }
}
