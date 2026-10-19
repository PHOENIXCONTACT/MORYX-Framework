// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Identity;

/// <summary>
/// Extension methods for <see cref="IIdentity"/>.
/// </summary>
public static class IdentityExtensions
{
    extension(IIdentity identity)
    {
        /// <summary>
        /// Combines this identity with one or more <paramref name="others"/> into a <see cref="CombinedIdentity"/>.
        /// </summary>
        public IIdentity Combine(params IIdentity[] others) => CombinedIdentity.From(others.Prepend(identity));

        /// <summary>
        /// Combines this <paramref name="identity"/> with a collection of <paramref name="others"/> into an
        /// <see cref="IIdentity"/>.
        /// </summary>
        public IIdentity Combine(IEnumerable<IIdentity> others) => CombinedIdentity.From(others.Prepend(identity));

        /// <inheritdoc cref="CombinedIdentity.GetAll()"/>
        public IReadOnlyList<IIdentity> GetAll()
            => identity is CombinedIdentity combined ? combined.GetAll() : [identity];

        /// <inheritdoc cref="CombinedIdentity.Matches(IIdentity)"/>
        public bool Matches(IIdentity required)
        {
            // Something not identifiable can only match the empty requirement
            if (identity is NullIdentity)
            {
                return required is NullIdentity;
            }
            // An empty requirement is matched by anything
            if (required is NullIdentity)
            {
                return true;
            }
            // A combined identity checks whether the required are a subset
            if (identity is CombinedIdentity combined)
            {
                return combined.Matches(required);
            }
            // A singular identity can only match on equality
            return required.Equals(identity);
        }

        /// <inheritdoc cref="CombinedIdentity.MatchedBy(IIdentity)"/>
        public bool MatchedBy(IIdentity candidate)
        {
            // A not identifiable candidate can only match the empty requirement
            if (candidate is NullIdentity)
            {
                return identity is NullIdentity;
            }
            // Not identifiable is matched by anything
            if (identity is NullIdentity)
            {
                return true;
            }
            // A singular candidate can only hope for equality
            if (candidate is not CombinedIdentity combinedCandidate)
            {
                return identity.Equals(candidate);
            }
            // A combined candidate must be a superset
            return combinedCandidate.Matches(identity);
        }
    }
}
