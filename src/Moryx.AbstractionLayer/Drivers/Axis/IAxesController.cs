// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Axis
{
    /// <summary>
    /// Driver that can control axes
    /// </summary>
    public interface IAxesController : IDriver
    {
        /// <summary>
        /// Will move the axes of the system to the given position
        /// </summary>
        /// <param name="movement">Array of axes which should be moved</param>
        Task<AxisMovementResponse> MoveAxes(params AxisMovement[] movement);
    }
}
