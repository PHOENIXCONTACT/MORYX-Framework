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
        /// Workplan unique element id of this connector
        /// </summary>
        long Id { get; set; }

        /// <summary>
        /// Transition name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Position of this node
        /// </summary>
        WorkplanNodePosition Position { get; set; }
    }

    /// <summary>
    /// Position of a workplan node
    /// </summary>
    public class WorkplanNodePosition
    {
        /// <summary>
        /// X coordinate
        /// </summary>
        public long X { get; set; }

        /// <summary>
        /// Y coordinate
        /// </summary>
        public long Y { get; set; }
    }
}
