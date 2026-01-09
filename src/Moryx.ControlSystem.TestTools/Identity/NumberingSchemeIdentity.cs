// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Identity;

namespace Moryx.ControlSystem.TestTools.Identity;

/// <summary>
/// <see cref="NumberIdentity"/> for numbers defined in <see cref="NumberingScheme"/>
/// </summary>
[DataContract]
public class NumberingSchemeIdentity : NumberIdentity
{
    /// <summary>
    /// Creates a new instance of the <see cref="NumberingSchemeIdentity"/>,
    /// Used for serialization and deserialization
    /// </summary>
    public NumberingSchemeIdentity() : base(0)
    {
    }

    /// <summary>
    /// Create number identity for a certain type
    /// </summary>
    /// <param name="numberType"></param>
    public NumberingSchemeIdentity(int numberType) : base((int)numberType)
    {
    }

    /// <summary>
    /// Create number identity of type and know identifier
    /// </summary>
    /// <param name="numberType"></param>
    /// <param name="identifier"></param>
    public NumberingSchemeIdentity(int numberType, string identifier) : base((int)numberType, identifier)
    {
    }

    /// <summary>
    /// Scheme of this identity
    /// </summary>
    public int Scheme
    {
        get => (int)NumberType;
        set => NumberType = (int)value;
    }
}