// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Linking;

/// <summary>
/// Result of a link or unlink request on a <see cref="MaterialContainerLink"/>.
/// </summary>
public class LinkingResult
{
    /// <summary>
    /// Outcome of the request.
    /// </summary>
    public LinkingStatus Status { get; }

    /// <summary>
    /// Validation context populated during the request. Empty for <see cref="LinkingStatus.Pending"/>
    /// and <see cref="LinkingStatus.Unchanged"/>.
    /// </summary>
    public ValidationContext Context { get; }

    private LinkingResult(LinkingStatus status, ValidationContext context)
    {
        Status = status;
        Context = context;
    }

    /// <inheritdoc cref="LinkingStatus.Applied"/>
    public static LinkingResult Applied(ValidationContext validationContext)
        => new(LinkingStatus.Applied, validationContext);

    /// <inheritdoc cref="LinkingStatus.Rejected"/>
    public static LinkingResult Rejected(ValidationContext validationContext)
        => new(LinkingStatus.Rejected, validationContext);

    /// <inheritdoc cref="LinkingStatus.Pending"/>
    public static LinkingResult Pending() => new(LinkingStatus.Pending, new());

    /// <inheritdoc cref="LinkingStatus.Unchanged"/>
    public static LinkingResult Unchanged() => new(LinkingStatus.Applied, new());
}
