// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Material.Management.Components;

/// <summary>
/// Matches inbound announcements / registrations against pending requests/announcements
/// and drives the appropriate state transitions.
/// </summary>
internal interface IFulfillmentMatcher : IPlugin
{
    /// <summary>
    /// Tries to match the given announcement against an existing request and returns
    /// the matched container (if any).
    /// </summary>
    IMaterialContainer? TryMatch(MaterialAnnouncement announcement);

    /// <summary>
    /// Tries to match an incoming registration of <paramref name="container"/> against
    /// an existing request or announcement. Returns the existing virtual container that
    /// should be merged into / updated to represent the registration, or <c>null</c> if
    /// no match exists.
    /// </summary>
    IMaterialContainer? TryMatchRegistration(IMaterialContainer container);
}