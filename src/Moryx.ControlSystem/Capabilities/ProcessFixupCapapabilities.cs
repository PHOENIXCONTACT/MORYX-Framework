// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.ControlSystem.Capabilities
{
    /// <summary>
    /// Capabilities to fixup broken processes
    /// </summary>
    [DataContract]
    public class ProcessFixupCapabilities : CapabilitiesBase
    {
        /// <inheritdoc />
        protected override bool ProvidedBy(ICapabilities provided)
        {
            return provided is ProcessFixupCapabilities;
        }
    }
}
