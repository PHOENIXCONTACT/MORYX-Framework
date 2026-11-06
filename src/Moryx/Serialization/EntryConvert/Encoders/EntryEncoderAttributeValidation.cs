// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Serialization;

internal static class EntryEncoderAttributeValidation
{
    /// <summary>
    /// Validate that the given type can be used as an <see cref="EntryEncoderBase"/>
    /// </summary>
    /// <remarks>
    /// We require a <see cref="Type"/> deriving from <see cref="EntryEncoderBase"/> with a public parameterless constructor
    /// </remarks>
    internal static void ValidateEncoderType(Type encoderType, string paramName = "encoderType")
    {
        ArgumentNullException.ThrowIfNull(encoderType, paramName);

        if (!typeof(EntryEncoderBase).IsAssignableFrom(encoderType) || encoderType.IsAbstract)
        {
            throw new ArgumentException($"{encoderType.Name} is not a non-abstract {nameof(EntryEncoderBase)}.", paramName);
        }

        if (encoderType.GetConstructor(Type.EmptyTypes) == null)
        {
            throw new ArgumentException($"{encoderType.Name} has no public parameterless constructor.", paramName);
        }
    }
}
