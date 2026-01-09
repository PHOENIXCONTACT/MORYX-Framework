// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.FactoryMonitor.Endpoints.Models;

/// <summary>
/// Reference model used in the ActivityChangedModel
/// </summary>
public class OrderReferenceModel
{
    [DataMember]
    public string Order { get; set; }

    [DataMember]
    public string Operation { get; set; }
}