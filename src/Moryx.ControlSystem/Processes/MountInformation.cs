// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.ControlSystem.Cells;

namespace Moryx.ControlSystem.Processes
{
    /// <summary>
    /// Combined process and session information on a <see cref="IProcessHolderPosition"/>
    /// </summary>
    public struct MountInformation
    {
        /// <summary>
        /// Gets the process to transfer.
        /// </summary>
        public IProcess Process { get; }

        /// <summary>
        /// Gets the session to transfer.
        /// </summary>
        public Session Session { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MountInformation"/> class.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="session">The session.</param>
        public MountInformation(IProcess process, Session session)
        {
            Process = process;
            Session = session;
        }
    }
}
