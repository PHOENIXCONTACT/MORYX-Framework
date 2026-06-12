// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Moryx.Serialization;

namespace Moryx.Material.Linking;

/// <summary>
/// Built-in <see cref="ILinkingRequirement"/> requesting explicit operator acknowledgement.
/// </summary>
[DataContract]
public class OperatorAcknowledgementRequirement : ILinkingRequirement
{
    /// <inheritdoc />
    public RequirementMode Mode => RequirementMode.Manual;

    /// <summary>
    /// Pseudonym entered by the operator to acknowledge the action.
    /// </summary>
    [DataMember]
    [EntrySerialize]
    [Display(Name = "Operator", Description = "Register operator pseudonym to confirm action.")]
    public string? OperatorPseudonym { get; set; }

    /// <inheritdoc />
    public bool IsFulfilled
    {
        get => !string.IsNullOrEmpty(OperatorPseudonym);
        set { /* derived from OperatorPseudonym */ }
    }
}