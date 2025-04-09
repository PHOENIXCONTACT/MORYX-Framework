// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Capabilities
{
    /// <summary>
    /// Defines the capabilities of combined devices like PcWorX devices which contain some kind of logical sub devices.
    /// </summary>
    public class CombinedCapabilities : ICapabilities
    {
        /// <summary>
        /// The capabilities this combination consists of.
        /// </summary>
        private readonly IReadOnlyList<ICapabilities> _capabilities;

        /// <summary>
        /// Create combined capabilities instance from collection of capabilities
        /// </summary>
        public CombinedCapabilities(IReadOnlyList<ICapabilities> capabilities)
        {
            _capabilities = capabilities;
        }

        ///
        public bool IsCombined => true;

        ///
        bool ICapabilities.ProvidedBy(ICapabilities provided)
        {
            return _capabilities.All(capabilities => capabilities.ProvidedBy(provided));
        }

        bool ICapabilities.Provides(ICapabilities required)
        {
            return required.IsCombined
                ? required.GetAll().All(rc => rc.ProvidedBy(this))
                : _capabilities.Any(required.ProvidedBy);
        }

        ///
        IEnumerable<ICapabilities> ICapabilities.GetAll()
        {
            return _capabilities;
        }
    }
}
