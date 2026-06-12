// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Material.States;

/// <summary>
/// State of a registered, in-use container.
/// </summary>
[DataContract]
public class AvailableState : MaterialContainerStateBase
{
    /// <inheritdoc />
    public override MaterialContainerStateClassification Classification => MaterialContainerStateClassification.Available;
}