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
    /// Type of the <see cref="EntryConverter"/> that takes over the conversion of this member,
    /// or of all members of this type. Must derive from <see cref="EntryConverter"/> and provide
    /// a public parameterless constructor.
    /// </summary>
    public Type ConverterType
    {
        get => field;
        set
        {
            EntryConverterAttributeValidation.ValidateConverterType(value, nameof(ConverterType));
            field = value;
        }
    }

    /// <summary>
    /// Defines whether <see cref="ConverterType"/> converts this member or the items of this collection member.
    /// The default is <see cref="EntryConverterTarget.Member"/>.
    /// </summary>
    public EntryConverterTarget ConverterTarget { get; set; }

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
