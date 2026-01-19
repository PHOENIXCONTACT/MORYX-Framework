// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Serialization;

/// <summary>
/// Possible value for an entry.
/// </summary>
[DataContract]
public class EntryPossible
{
    /// <summary>
    /// Invariant key of the possible value.
    /// </summary>
    [DataMember]
    public string Key { get; set; }

    /// <summary>
    /// Display name of the possible value.
    /// </summary>
    [DataMember]
    public string DisplayName { get; set; }

    /// <summary>
    /// Description of the possible value.
    /// </summary>
    [DataMember]
    public string Description { get; set; }

    /// <summary>
    /// Converts an <see cref="IEnumerable{String}"/> of possible values to an <see cref="EntryPossible"/> array.
    /// Key and DisplayName are set to the string value.
    /// </summary>
    public static EntryPossible[] FromStrings(IEnumerable<string> possible)
    {
        return possible?
            .Select(p => p != null ? new EntryPossible { Key = p, DisplayName = p, Description = null } : null)
            .ToArray();
    }
}
