// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moq;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Serialization;

namespace Moryx.Tests.Serialization;

/// <summary>
/// Shared helpers for the <see cref="EntryConvert"/> test fixtures
/// </summary>
internal static class EntryConvertTestHelper
{
    /// <summary>
    /// Get the single sub entry with the given identifier
    /// </summary>
    public static Entry SubEntry(this Entry entry, string identifier) =>
        entry.SubEntries.Single(e => e.Identifier == identifier);

    /// <summary>
    /// Instantiate a prototype of the collection entry, add it as new item and return it.
    /// The current value is only overwritten if <paramref name="current"/> is given.
    /// </summary>
    public static Entry AddFromPrototype(this Entry collection, string current = null, int prototype = 0)
    {
        var item = collection.Prototypes[prototype].Instantiate();
        if (current is not null)
        {
            item.Value.Current = current;
        }

        collection.SubEntries.Add(item);
        return item;
    }

    /// <summary>
    /// Create a <see cref="PossibleValuesSerialization"/> with mocked container and service provider
    /// </summary>
    public static PossibleValuesSerialization CreatePossibleValuesSerialization(ValueProviderExecutorSettings settings = null) =>
        new(new Mock<IContainer>().Object, new Mock<IServiceProvider>().Object,
            new ValueProviderExecutor(settings ?? new ValueProviderExecutorSettings()));
}
