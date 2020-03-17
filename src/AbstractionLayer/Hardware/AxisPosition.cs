// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.AbstractionLayer.Hardware
{
    /// <summary>
    /// Different positions an axes can be set to
    /// </summary>
    public enum AxisPosition
    {
        /// <summary>
        /// Move axis to its next position, what ever that may be
        /// </summary>
        Next,
        /// <summary>
        /// Move a horizontal axes to the left
        /// </summary>
        Left,
        /// <summary>
        /// Move a horizontal axes to the right
        /// </summary>
        Right,
        /// <summary>
        /// Move a vertical axes up
        /// </summary>
        Up, 
        /// <summary>
        /// Move a vertical axes down
        /// </summary>
        Down,
        /// <summary>
        /// Move depth axes to the front
        /// </summary>
        Front,
        /// <summary>
        /// Move depth axes to the back
        /// </summary>
        Back,
        /// <summary>
        /// Open a door
        /// </summary>
        Open,
        /// <summary>
        /// Close a door 
        /// </summary>
        Closed,
        /// <summary>
        /// Rotate an axis in its default direction
        /// </summary>
        Rotate,
        /// <summary>
        /// Rotate an axis clockwise
        /// </summary>
        RotateClockwise,
        /// <summary>
        /// Rotate an axis counter-clockwise
        /// </summary>
        RotateCounterClockwise,
        /// <summary>
        /// Move axes to reference position
        /// </summary>
        Reference,
    }
}
