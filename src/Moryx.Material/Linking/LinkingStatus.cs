// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;

namespace Moryx.Material.Linking;

/// <summary>
/// Outcome of a link or unlink request on a <see cref="MaterialContainerLink"/>.
/// </summary>
public enum LinkingStatus
{
    /// <summary>
    /// The request was validated and applied.
    /// </summary>
    [Display(Name = "Applied", Description = "The request was validated and applied.")]
    Applied,

    /// <summary>
    /// The request was rejected during validation; the link is unchanged.
    /// </summary>
    [Display(Name = "Rejected", Description = "The request was rejected during validation; the link is unchanged.")]
    Rejected,

    /// <summary>
    /// No integration is attached yet; the request is stored and processed once one attaches.
    /// </summary>
    [Display(Name = "Pending", Description = "No integration is attached yet; the request is stored and processed once one attaches.")]
    Pending,

    /// <summary>
    /// The request did not change anything, e.g. unlinking a link that is not linked.
    /// </summary>
    [Display(Name = "Unchanged", Description = "The request did not change anything, e.g. unlinking a link that is not linked.")]
    Unchanged
}
