// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Identity
{
    /// <summary>
    /// Base interface for identities (e.g. SerialNumber)
    /// </summary>
    public interface IIdentity : IEquatable<IIdentity>
    {
        /// <summary>
        /// Main and unique string identifier
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Set the identifier for this identity
        /// </summary>
        /// <param name="identifier"></param>
        /// <exception cref="InvalidOperationException">Identifiers of some identities must not be overriden</exception>
        void SetIdentifier(string identifier);
    }
}
