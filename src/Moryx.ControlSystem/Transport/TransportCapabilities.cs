// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.ControlSystem.Transport
{
    /// <summary>
    /// Capabilities provided by resources that can route articles or article groups wihtin the machine
    /// </summary>
    public class TransportCapabilities : CapabilitiesBase
    {
        /// <summary>
        /// This property serves two main purpose. For the resource it contains the resources that are connected to the
        /// transport system. For the module it contains the two resources included in a certain route to determine the
        /// correct router for a message.
        /// </summary>
        public long[] ConnectedResources { get; set; }

        /// <inheritdoc />
        protected override bool ProvidedBy(ICapabilities provided)
        {
            var capabilities = provided as TransportCapabilities;
            if (capabilities == null)
                return false;

            if (ConnectedResources == null || ConnectedResources.Length == 0)
                return true;

            // If resources are specified we must check whether we are connected to all of them
            if (ConnectedResources.All(requiredRes => capabilities.ConnectedResources.Contains(requiredRes)))
                return true;

            return false;
        }
    }
}
