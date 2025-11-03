// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Hardware;

namespace Moryx.AbstractionLayer.Drivers.Axis
{
    /// <summary>
    /// Driver that can control axes
    /// </summary>
    public interface IAxesController : IDriver
    {
        /// <summary>
        /// Will move the axis of the laser to the given position
        /// </summary>
        /// <param name="axis">The axis which should be moved</param>
        /// <param name="targetPosition">The target position of the axis</param>
        Task<AxisMovementResponse> MoveAxis(Axes axis, double targetPosition);

        /// <summary>
        /// Will move the axis of the laser to the given position
        /// </summary>
        /// <param name="axis">The axis which should be moved</param>
        /// <param name="targetPosition">The target position of the axis</param>
        Task<AxisMovementResponse> MoveAxis(Axes axis, AxisPosition targetPosition);
    }
}
