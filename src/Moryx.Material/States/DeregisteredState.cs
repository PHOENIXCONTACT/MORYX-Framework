// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Material.States;

/// <summary>
/// Terminal state of a container which has been deregistered.
/// </summary>
[DataContract]
public class DeregisteredState : MaterialContainerStateBase
{
    /// <inheritdoc />
    public override MaterialContainerStateClassification Classification => MaterialContainerStateClassification.Deregistered;
}