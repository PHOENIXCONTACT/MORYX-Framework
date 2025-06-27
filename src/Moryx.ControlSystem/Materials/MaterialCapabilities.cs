// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.ControlSystem.Materials
{
    /// <summary>
    /// Base capabilities for all material containers
    /// </summary>
    public class MaterialCapabilities : CapabilitiesBase
    {
        /// <inheritdoc />
        protected override bool ProvidedBy(ICapabilities provided)
        {
            return provided is MaterialCapabilities;
        }
    }
}
