// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Tools;

namespace Moryx.Serialization;

/// <summary>
/// Extension methods for <see cref="Type"/> to create <see cref="EntryPossible"/> instances
/// </summary>
public static class TypeEntryExtensions
{
    /// <summary>
    /// Creates an <see cref="EntryPossible"/> from the given <paramref name="type"/>
    /// </summary>
    public static EntryPossible ToEntryPossible(this Type type) => new()
    {
        Key = type.FullName,
        DisplayName = type.GetDisplayName() ?? type.Name,
        Description = type.GetDescription(),
    };
}
