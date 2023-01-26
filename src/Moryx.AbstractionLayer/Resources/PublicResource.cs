// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.AbstractionLayer.Resources
{
    /// <summary>
    /// Base class for all public resources
    /// </summary>
    public abstract class PublicResource : Resource, IPublicResource
    {
        /// <summary>
        /// Current capabilities of this resource
        /// </summary>
        private ICapabilities _capabilities = NullCapabilities.Instance;

        /// <inheritdoc />
        public ICapabilities Capabilities
        {
            get
            {
                return _capabilities;
            }
            protected set
            {
                _capabilities = value;
                CapabilitiesChanged?.Invoke(this, _capabilities);
            }
        }

        /// <summary>
        /// <seealso cref="IResource"/>
        /// </summary>
        public event EventHandler<ICapabilities> CapabilitiesChanged;
    }
}
