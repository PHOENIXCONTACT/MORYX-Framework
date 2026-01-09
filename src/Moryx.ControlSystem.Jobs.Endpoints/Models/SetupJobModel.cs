// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.ControlSystem.Jobs.Endpoints;

[DataContract]
public class SetupJobModel
{
    [DataMember]
    public SetupJobClassification Classification { get; set; }

    [DataMember]
    public long TargetRecipeId { get; set; }

    [DataMember]
    public SetupStepModel[] Steps { get; set; }
}

[DataContract]
public class SetupStepModel
{
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public StepState State { get; set; }
}

public enum StepState
{
    Initial,
    Running,
    Completed
}

public enum SetupJobClassification
{
    None,
    Prepare,
    Cleanup,
    Manual,
}