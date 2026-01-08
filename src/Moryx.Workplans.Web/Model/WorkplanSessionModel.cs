// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Workplans.Endpoint;

public class WorkplanSessionModel
{
    /// <summary>
    /// Token of the session
    /// </summary>
    [DataMember]
    public string SessionToken { get; set; }

    /// <summary>
    /// Id of the workplan
    /// </summary>
    [DataMember]
    public long WorkplanId { get; set; }

    /// <summary>
    /// Name of the workplan
    /// </summary>
    [DataMember]
    public string Name { get; set; }

    /// <summary>
    /// Version of the workplan
    /// </summary>
    [DataMember]
    public int Version { get; set; }

    /// <summary>
    /// Current state of the workplan
    /// </summary>
    [DataMember]
    public WorkplanState State { get; set; }

    /// <summary>
    /// All steps within the workplan
    /// </summary>
    [DataMember]
    public WorkplanNodeModel[] Nodes { get; set; }
}