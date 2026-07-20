// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Material.States;

namespace Moryx.Material.Management.Components;

[Component(LifeCycle.Singleton, typeof(IFulfillmentMatcher))]
internal class FulfillmentMatcher : IFulfillmentMatcher
{
    public IContainerPool Pool { get; set; }

    public void Start() { }
    public void Stop() { }

    public IMaterialContainer? TryMatch(MaterialAnnouncement announcement)
    {
        // Match by request reference first, then by container identity, then by material.
        var candidates = Pool.GetAll(c => c.State is RequestedStateInformation);

        if (string.IsNullOrEmpty(announcement.RequestReference))
        {
            var match = candidates.FirstOrDefault(c =>
                c.State is StateClassification.Requested); //&& c.RequestId == announcement.RequestReference);
            if (match != null)
            {
                return match;
            }
        }

        if (announcement.ContainerIdentity != null)
        {
            var match = candidates.FirstOrDefault(c =>
                c.Identity != null &&
                string.Equals(c.Identity.Identifier, announcement.ContainerIdentity.Identifier, StringComparison.Ordinal));
            if (match != null)
            {
                return match;
            }
        }

        if (!string.IsNullOrEmpty(announcement.Material))
        {
            return candidates.FirstOrDefault(c =>
                string.Equals(c.Material, announcement.Material, StringComparison.Ordinal) &&
                c.Quantity >= announcement.AnnouncedQuantity);
        }

        return null;
    }

    public IMaterialContainer? TryMatchRegistration(IMaterialContainer container)
    {
        if (container == null)
        {
            throw new ArgumentNullException(nameof(container));
        }

        var candidates = Pool.GetAll(c => c.State is StateClassification.Requested or StateClassification.Inbound && !ReferenceEquals(c, container));

        // Match by identity first
        if (container.Identity != null)
        {
            var byIdentity = candidates.FirstOrDefault(c =>
                c.Identity != null &&
                string.Equals(c.Identity.Identifier, container.Identity.Identifier, StringComparison.Ordinal));
            if (byIdentity != null)
            {
                return byIdentity;
            }
        }

        // Fallback: match by material + sufficient pending quantity
        if (!string.IsNullOrEmpty(container.Material))
        {
            return candidates.FirstOrDefault(c =>
                string.Equals(c.Material, container.Material, StringComparison.Ordinal) &&
                c.Quantity >= container.Quantity);
        }

        return null;
    }
}
