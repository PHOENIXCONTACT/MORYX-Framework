// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Moryx.Material.States;

/// <summary>
/// Terminal state information of a container indicating a clean transition before deregistration from the system.
/// </summary>
[DataContract]
[Display(Name = "Deregistered", Description = "Container was deregistered from the system after a clean transition.")]
public class DeregisteredStateInformation : StateInformation
{
    /// <inheritdoc />
    public override string ToString() => "Deregistered";
}
