// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Cells;

namespace Moryx.ControlSystem.Processes
{
    /// <summary>
    /// Common interface to interact with resources holding a process
    /// </summary>
    public interface IProcessHolderPosition : IResource
    {
        /// <summary>
        /// Process on this position
        /// </summary>
        IProcess Process { get; }

        /// <summary>
        /// Identifier of this position. It is not necessarily a unique
        /// identifier and depends on the concrete type and context
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Current session on this position
        /// </summary>
        Session Session { get; }

        /// <summary>
        /// Retrieve the current mount information
        /// </summary>
        MountInformation MountInformation { get; }

        /// <summary>
        /// Assign process and session to this position
        /// </summary>
        /// <param name="mountInformation"></param>
        void Mount(MountInformation mountInformation);

        /// <summary>
        /// Clear mount information AFTER they were moved somewhere else
        /// </summary>
        void Unmount();

        /// <summary>
        /// Reset position and return to initial state. All mount information is lost
        /// </summary>
        void Reset();

        /// <summary>
        /// Event raised when the reference <see cref="Process"/> was changed
        /// </summary>
        event EventHandler<IProcess> ProcessChanged;

        /// <summary>
        /// Event raised when the position was reset
        /// </summary>
        event EventHandler ResetExecuted;
    }
}
