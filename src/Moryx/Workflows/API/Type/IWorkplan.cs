// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans
{
    /// <summary>
    /// Type interface for workplans. Instances of a workplan are denoted by <see cref="IWorkplanInstance"/>
    /// </summary>
    public interface IWorkplan
    {
        /// <summary>
        /// Unique id of this workplan
        /// </summary>
        long Id { get; }

        /// <summary>
        /// Name of the workplan
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Version of the workplan
        /// </summary>
        int Version { get; }

        /// <summary>
        /// This workplans state
        /// </summary>
        WorkplanState State { get; }

        /// <summary>
        /// Connectors of this plan
        /// </summary>
        IEnumerable<IConnector> Connectors { get; }

        /// <summary>
        /// All steps of this workplan
        /// </summary>
        IEnumerable<IWorkplanStep> Steps { get; }
    }
}
