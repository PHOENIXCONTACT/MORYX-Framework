// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Capabilities
{
    /// <summary>
    /// Common capabilities interface
    /// </summary>
    public interface ICapabilities
    {
        /// <summary>
        /// Flag if the capabilities of this instance are implemented by sub instances
        /// </summary>
        bool IsCombined { get; }

        /// <summary>
        /// Checks whether this capability are fully provided by the other object object complies to the given one.
        /// </summary>
        /// <param name="provided">The candidate of provided capabilities</param>
        /// <returns><c>true</c> if these capabilities are provided by the given ones or <c>false</c> otherwise.</returns>
        bool ProvidedBy(ICapabilities provided);

        /// <summary>
        /// Check if this capability can provide the required capabilities
        /// </summary>
        /// <param name="required">The required capabilities we have to match</param>
        /// <returns></returns>
        bool Provides(ICapabilities required);

        /// <summary>
        /// Get all single capabilities of this implementation
        /// </summary>
        /// <returns></returns>
        IEnumerable<ICapabilities> GetAll();
    }
}
