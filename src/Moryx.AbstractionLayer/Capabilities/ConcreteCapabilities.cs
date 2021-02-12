// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Moryx.AbstractionLayer.Capabilities
{
    /// <summary>
    /// Abstract base class for concrete capabilities
    /// </summary>
    [DataContract]
    [Obsolete("This is removed in the next major version. Use CapabilitiesBase instead")]
    public abstract class ConcreteCapabilities : ICapabilities
    {
        ///
        bool ICapabilities.IsCombined => false;

        ///
        bool ICapabilities.ProvidedBy(ICapabilities provided)
        {
            return provided.IsCombined ? provided.Provides(this) : ProvidedBy(provided);
        }

        ///
        bool ICapabilities.Provides(ICapabilities required)
        {
            return required.ProvidedBy(this);
        }

        ///
        public IEnumerable<ICapabilities> GetAll()
        {
            return new ICapabilities[] { this };
        }

        /// <summary>
        /// Check if our required capabilities are provided by the given object
        /// </summary>
        /// <param name="provided"></param>
        /// <returns></returns>
        protected abstract bool ProvidedBy(ICapabilities provided);
    }
}
