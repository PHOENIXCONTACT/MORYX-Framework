// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Cells;

namespace Moryx.ControlSystem.Setups
{
    /// <summary>
    /// Setup target using the current resource management
    /// </summary>
    public class CurrentResourceTarget : ISetupTarget
    {
        private readonly IResourceManagement _resourceManagement;

        /// <summary>
        /// Instantiate resource target
        /// </summary>
        public CurrentResourceTarget(IResourceManagement resourceManagement)
        {
            _resourceManagement = resourceManagement;
        }

        /// <summary>
        /// Fetch all cells by capabilities
        /// </summary>
        public IReadOnlyList<ICell> Cells(ICapabilities capabilities)
        {
            return _resourceManagement.GetResources<ICell>(capabilities).ToList();
        }
    }
}