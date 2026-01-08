// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.FactoryMonitor.Endpoints.Models;

/// <summary>
/// Simple order model to display in the UI
/// </summary>
[DataContract]
public class OrderChangedModel //can't inherit OrderReferenceModel as before, because during SerializeObject data gets lost
{
    [DataMember]
    public string Order { get; set; }

    [DataMember]
    public string Operation { get; set; }

    [DataMember]
    public InternalOperationClassification State { get; set; }
}