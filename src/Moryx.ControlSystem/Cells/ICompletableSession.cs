// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Processes;

namespace Moryx.ControlSystem.Cells
{
    /// <summary>
    /// Indicator if a session can complete the current sequence
    /// </summary>
    public interface ICompletableSession
    {
        /// <summary>
        /// Complete the current sequence
        /// </summary>
        SequenceCompleted CompleteSequence(Process process, bool processActive, params long[] nextCells);
    }
}
