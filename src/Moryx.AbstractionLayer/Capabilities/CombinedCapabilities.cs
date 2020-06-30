// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Linq;

namespace Marvin.AbstractionLayer.Capabilities
{
    /// <summary>
    /// Defines the capabilities of combined devices like PcWorX devices which contain some kind of logical sub devices.
    /// </summary>
    public class CombinedCapabilities : ICapabilities
    {
        /// <summary>
        /// The capabilities this combination consists of.
        /// </summary>
        private readonly ICapabilities[] _capabilities;

        /// <summary>
        /// Create combined capabilites instance from a dictionary of capabilities
        /// </summary>
        public CombinedCapabilities(IEnumerable<ICapabilities> capabilities)
        {
            _capabilities = capabilities as ICapabilities[] ?? capabilities.ToArray();
        }

        ///
        public bool IsCombined { get; } = true;

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
