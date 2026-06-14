// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Identity;

namespace Moryx.Material;

/// <summary>
/// Announcement that material is inbound to the system.
/// </summary>
/// <param name="RequestReference"> Optional cross-reference to an existing <see cref="MaterialRequest"/>. </param>
/// <param name="Material"> Material reference. May be omitted if a request is referenced. </param>
/// <param name="AnnouncedQuantity"> Announced quantity. </param>
/// <param name="Unit"> Optional unit of <see cref="AnnouncedQuantity"/>. </param>
/// <param name="ContainerIdentity"> Optional identity of a specific announced container. </param>
/// <param name="ExpectedArrival"> Optional expected arrival. </param>
public record MaterialAnnouncement(string? RequestReference, string? Material, decimal AnnouncedQuantity,
    string? Unit, IIdentity? ContainerIdentity, DateTime? ExpectedArrival)
{
    /// <summary>
    /// Optional unique identifier of the request. Generated if not set.
    /// </summary>
    public string? Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
