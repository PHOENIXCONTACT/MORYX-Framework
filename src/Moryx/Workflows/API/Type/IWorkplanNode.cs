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

        position {get; set}
    }
}
