// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Shifts.Endpoints.Models;

/// <summary>
/// Class representing the context for creating a shift.
/// </summary>
[DataContract]
public class ShiftCreationContextModel
{
    /// <summary>
    /// Reference if of the shift type.
    /// </summary>
    [DataMember]
    public long TypeId { get; set; }

    /// <summary>
    /// The start date of the shift.
    /// </summary>
    [DataMember]
    //[DataType(DataType.Date)]
    public DateOnly Date { get; set; }
}