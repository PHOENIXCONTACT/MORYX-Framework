// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Cells;

namespace Moryx.ControlSystem.Setups
{
    /// <summary>
    /// Interface to decouple setup evaluation from the current state of the <see cref="IResourceManagement"/>
    /// </summary>
    public interface ISetupTarget
    {
        /// <summary>
        /// Return all cells that provide given, required capabilities
        /// </summary>
        IReadOnlyList<ICell> Cells(ICapabilities capabilities);
    }
}