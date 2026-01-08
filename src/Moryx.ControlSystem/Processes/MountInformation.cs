// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Processes;
using Moryx.ControlSystem.Cells;

namespace Moryx.ControlSystem.Processes;

/// <summary>
/// Combined process and session information on a <see cref="IProcessHolderPosition"/>
/// </summary>
public struct MountInformation
{
    /// <summary>
    /// Gets the process to transfer.
    /// </summary>
    public Process Process { get; }

    /// <summary>
    /// Gets the session to transfer.
    /// </summary>
    public Session Session { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MountInformation"/> class.
    /// </summary>
    /// <param name="process">The process.</param>
    /// <param name="session">The session.</param>
    public MountInformation(Process process, Session session)
    {
        Process = process;
        Session = session;
    }
}