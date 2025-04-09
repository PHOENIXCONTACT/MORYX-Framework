// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Drawing;

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
        Point Position { get; set; }
    }
}
