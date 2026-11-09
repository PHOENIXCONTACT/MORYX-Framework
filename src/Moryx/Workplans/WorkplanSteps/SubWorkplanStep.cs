// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Moryx.Properties;
using Moryx.Workplans.Transitions;

namespace Moryx.Workplans.WorkplanSteps;

/// <summary>
/// Step that creates a <see cref="SubworkplanTransition"/> with the given workplan
/// </summary>
[DataContract]
[Display(ResourceType = typeof(Strings), Name = nameof(Strings.SubworkplanStep_DisplayName), Description = nameof(Strings.SubworkplanStep_Description))]
public class SubworkplanStep : SubWorkplanStepBase
{
    private SubworkplanStep()
    {
        // Empty constructor for JSON
    }

    /// <summary>
    /// Create step from workplan
    /// </summary>
    public SubworkplanStep(IWorkplan workplan) : base(workplan)
    {
    }

    private IIndexResolver _indexResolver;
    /// <summary>
    /// Instantiate transition from this step
    /// </summary>
    protected override TransitionBase Instantiate(IWorkplanContext context)
    {
        var engine = WorkplanInstance.CreateEngine(Workplan, context);
        var indexResolver = _indexResolver ??= TransitionBase.CreateIndexResolver(OutputDescriptions);
        var transition = new SubworkplanTransition(engine, indexResolver);
        return transition;
    }
}
