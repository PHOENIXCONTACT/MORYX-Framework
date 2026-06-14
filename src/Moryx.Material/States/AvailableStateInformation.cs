// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Moryx.Material.States;

/// <summary>
/// State information of a registered, in-use container.
/// </summary>
[DataContract]
[Display(Name = "Available", Description = "Container is registered and available for use.")]
public class AvailableStateInformation : StateInformation
{
    /// <inheritdoc />
    public override string ToString() => "Available";
}
