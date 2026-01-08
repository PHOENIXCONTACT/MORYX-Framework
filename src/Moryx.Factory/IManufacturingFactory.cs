// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;

namespace Moryx.Factory
{
    /// <summary>
    /// A manufacturing factory interface
    /// </summary>
    public interface IManufacturingFactory : IResource
    {
        /// <summary>
        /// Background URL of the factory monitor
        /// </summary>
        string BackgroundUrl { get; set; }
    }
}
