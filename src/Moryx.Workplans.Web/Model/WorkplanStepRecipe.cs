// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Serialization;
using Moryx.Tools;

namespace Moryx.Workplans.Endpoint;

/// <summary>
/// Exchange type for workplan step creation 
/// </summary>
[DataContract]
public sealed class WorkplanStepRecipe
{
    /// <summary>
    /// Server side data type
    /// </summary>
    [DataMember]
    public string Type { get; set; }

    /// <summary>
    /// Name of this step
    /// </summary>
    [DataMember]
    public string Name { get; set; }

    /// <summary>
    /// Long description of this step
    /// </summary>
    [DataMember]
    public string Description { get; set; }

    /// <summary>
    /// Top 
    /// </summary>
    [DataMember]
    public int PositionTop { get; set; }

    /// <summary>
    /// Left position
    /// </summary>
    [DataMember]
    public int PositionLeft { get; set; }

    /// <summary>
    /// Classification of the step
    /// </summary>
    [DataMember]
    public WorkplanNodeClassification Classification { get; set; }

    /// <summary>
    /// All initializers required to create an instance of this step
    /// </summary>
    [DataMember]
    public MethodEntry Constructor { get; set; }

    /// <summary>
    /// Optional reference to a subworkplan. This is only valid for <see cref="Classification"/> of <see cref="WorkplanNodeClassification.Subworkplan"/>.
    /// </summary>
    [DataMember]
    public long SubworkplanId { get; set; }

    public static WorkplanStepRecipe FromStepType(Type step)
    {
        var consturctors = step.GetConstructors().ToList();
        MethodEntry constructor;
        if (consturctors.Count == 1 && step.GetConstructor([]) != null)
            constructor = null;
        else
            constructor = EntryConvert.EncodeConstructors(step).First();

        var recipe = new WorkplanStepRecipe
        {
            Type = step.FullName,
            Name = step.GetDisplayName(),
            Classification = ModelConverter.ToClassification(step),
            Description = step.GetDescription(),
            Constructor = constructor
        };

        return recipe;
    }
}