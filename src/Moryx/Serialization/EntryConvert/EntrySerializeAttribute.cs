// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Serialization;

[AttributeUsage(AttributeTargets.All)]
public class EntrySerializeAttribute : Attribute
{
    /// <summary>
    /// Configured <see cref="EntrySerializeMode"/> for the entry to serialize
    /// </summary>
    public EntrySerializeMode Mode { get; }

    /// <summary>
    /// Type of the <see cref="EntryEncoderBase"/> that takes over the conversion of this member,
    /// or of all members of this type. Must derive from <see cref="EntryEncoderBase"/> and provide
    /// a public parameterless constructor.
    /// </summary>
    public Type EncoderType
    {
        get => field;
        set
        {
            EntryEncoderAttributeValidation.ValidateEncoderType(value, nameof(EncoderType));
            field = value;
        }
    }

    /// <summary>
    /// Defines whether <see cref="EncoderType"/> converts this member or the items of this collection member.
    /// The default is <see cref="EntryEncoderTarget.Member"/>.
    /// </summary>
    public EntryEncoderTarget EncoderTarget { get; set; }

    /// <summary>
    /// Creates a new instance of the <see cref="EntrySerializeAttribute"/>. The default mode is <see cref="EntrySerializeMode.Always"/>
    /// </summary>
    public EntrySerializeAttribute() : this(EntrySerializeMode.Always)
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="EntrySerializeAttribute"/>.
    /// </summary>
    public EntrySerializeAttribute(EntrySerializeMode mode)
    {
        Mode = mode;
    }
}

/// <summary>
/// Mode to for the <see cref="EntrySerializeAttribute"/>
/// </summary>
public enum EntrySerializeMode
{
    Always,
    Never
}
