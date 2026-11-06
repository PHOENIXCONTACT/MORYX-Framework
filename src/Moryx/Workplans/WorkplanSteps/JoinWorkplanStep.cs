// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Moryx.Properties;
using Moryx.Workplans.Transitions;

namespace Moryx.Workplans.WorkplanSteps;

/// <summary>
/// Workplan step to join multiple inputs
/// </summary>
[DataContract]
[Display(ResourceType = typeof(Strings), Name = "JoinWorkplanStep_Name", Description = "JoinWorkplanStep_Description")]
public class JoinWorkplanStep : WorkplanStepBase
{
    private JoinWorkplanStep()
    {
        Name = "Join";
    }

    /// <summary>
    /// Create new join step for certain number of inputs
    /// </summary>
    /// <param name="inputs">Number of inputs</param>
    public JoinWorkplanStep(ushort inputs = 2)
    {
        Inputs = new IConnector[inputs];
    }

    /// 
    protected override TransitionBase Instantiate(IWorkplanContext context)
    {
        return new JoinTransition();
    }
}
