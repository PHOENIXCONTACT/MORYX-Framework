// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ControlSystem.Cells;

namespace Moryx.ControlSystem.Activities;

/// <summary>
/// Defines different classifications of activities. A resource will define accepted classifications
/// in the published <see cref="ReadyToWork"/>. An activity declares its classification which is compared
/// to the accepted classification within the kernel. While a <see cref="ReadyToWork"/> can accept multiple
/// classifications (by combining flags), an activity has only one classification.
/// </summary>
[Flags]
public enum ActivityClassification
{
    /// <summary>
    /// Use in case the activity classification is unkown
    /// </summary>
    Unknown = 0x00,

    /// <summary>
    /// Default classification is production
    /// </summary>
    Production = 0x01,

    /// <summary>
    /// Activity performs a setup/reconfiguration
    /// </summary>
    Setup = 0x02,

    /// <summary>
    /// Activity performs maintenance on the the target
    /// </summary>
    Maintenance = 0x04,

    /// <summary>
    /// Activity performs a preparation, for example for a <see cref="Production"/> activity
    /// </summary>
    Preparation = 0x08,
}