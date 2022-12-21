// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans
{
    /// <summary>
    /// Common interface for <see cref="IWorkplanStep"/> and <see cref="IConnector"/>
    /// </summary>
    public interface IWorkplanNode
    {
        /// <summary>
        /// Workplan unique element id of this node
        /// </summary>
        long Id { get; set; }

        /// <summary>
        /// Node name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Position of this <see cref="IWorkplanNode"/> in any visualization of the <see cref="IWorkplan"/>
        /// </summary>
        NodePosition Position { get; set; }
    }

    /// <summary>
    /// Unitless position of a workplan node in a two-dimensional space.
    /// </summary>
    public struct NodePosition
    {
        /// <summary>
        /// Unitless X-Coordinate of the node
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Unitless Y-Coordinate of the node
        /// </summary>
        public double Y { get; set; }
    }
}
