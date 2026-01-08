// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Serialization;

namespace Moryx.VisualInstructions.Endpoints;

/// <summary>
/// Model class for an visual instruction
/// </summary>
[DataContract]
public class InstructionModel
{
    /// <summary>
    /// Runtime unique identifier of this instruction
    /// </summary>
    [DataMember]
    public long Id { get; set; }

    /// <summary>
    /// Name of the sender
    /// </summary>
    [DataMember]
    public string Sender { get; set; }

    /// <summary>
    /// Type of the instruction, Display or Execute
    /// </summary>
    [DataMember]
    public InstructionType Type { get; set; }

    /// <summary>
    /// Items of the instruction
    /// </summary>
    [DataMember]
    public InstructionItemModel[] Items { get; set; }

    /// <summary>
    /// Optional inputs by the user
    /// </summary>
    [DataMember]
    public Entry Inputs { get; set; }

    /// <summary>
    /// Possible results with key and display values
    /// </summary>
    [DataMember]
    public InstructionResultModel[] Results { get; set; } = [];
}