// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.Cells;

/// <summary>
/// Message send to confirm the current sequence of the session
/// </summary>
public class SequenceCompleted : Session
{
    /// <summary>
    /// Flag if the process was completed by the last activity or is still active
    /// </summary>
    public bool ProcessActive { get; }

    /// <summary>
    /// Possible cells for the processes next activities. This information may be relevant
    /// to the processes current location
    /// </summary>
    public long[] NextCells { get; }

    /// <summary>
    /// Internal constructor to forward the production context
    /// </summary>
    internal SequenceCompleted(Session currentSession, bool processActive, long[] nextCells) : base(currentSession)
    {
        ProcessActive = processActive;
        NextCells = nextCells;
    }

    /// <summary>
    /// Continue the current session by sending a new ready to work
    /// </summary>
    public ReadyToWork ContinueSession()
    {
        return new ReadyToWork(this);
    }
}