// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Material.Endpoints.Model;

public class StateClassificationDescriptorModel
{
    public StateClassificationModel State { get; set; }

    public string StateDisplayName { get; set; }

    public string StateDescription { get; set; }
}