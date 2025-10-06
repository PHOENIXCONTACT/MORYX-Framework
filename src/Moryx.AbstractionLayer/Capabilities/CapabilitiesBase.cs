// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Capabilities
{
    /// <summary>
    /// Preferred base class for implementations of <see cref="ICapabilities"/>
    /// </summary>
    public abstract class CapabilitiesBase : ICapabilities
    {
        bool ICapabilities.IsCombined => false;

        bool ICapabilities.ProvidedBy(ICapabilities provided) =>
            provided.IsCombined ? provided.Provides(this) : ProvidedBy(provided);

        bool ICapabilities.Provides(ICapabilities required) =>
            required.ProvidedBy(this);

        /// <inheritdoc />
        public IEnumerable<ICapabilities> GetAll() =>
            [this];

        /// <summary>
        /// Check if our required capabilities are provided by the given object
        /// </summary>
        protected abstract bool ProvidedBy(ICapabilities provided);
    }
}