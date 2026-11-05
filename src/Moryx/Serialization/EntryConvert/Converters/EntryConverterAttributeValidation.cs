// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Serialization;

internal static class EntryConverterAttributeValidation
{
    /// <summary>
    /// Validate that the given type can be used as an <see cref="EntryConverter"/>
    /// </summary>
    /// <remarks>
    /// We require a <see cref="Type"/> deriving from <see cref="EntryConverter"/> with a public parameterless constructor
    /// </remarks>
    internal static void ValidateConverterType(Type converterType, string paramName = "converterType")
    {
        ArgumentNullException.ThrowIfNull(converterType, paramName);

        if (!typeof(EntryConverter).IsAssignableFrom(converterType) || converterType.IsAbstract)
        {
            throw new ArgumentException($"{converterType.Name} is not a non-abstract {nameof(EntryConverter)}.", paramName);
        }

        if (converterType.GetConstructor(Type.EmptyTypes) == null)
        {
            throw new ArgumentException($"{converterType.Name} has no public parameterless constructor.", paramName);
        }
    }
}
