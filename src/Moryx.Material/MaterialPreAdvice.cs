// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.States;

namespace Moryx.Material;

/// <summary>
/// Announcement that the content of a registered container is ready to leave the system.
/// </summary>
public class MaterialPreAdvice
{
    /// <summary>
    /// Id of the container being announced for departure.
    /// </summary>
    public long ContainerId { get; set; }

    /// <summary>
    /// Reason for the departure.
    /// </summary>
    public PreAdviceDepartureReason DepartureReason { get; set; }
}
