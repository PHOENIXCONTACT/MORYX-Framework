// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;

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
        SequenceCompleted CompleteSequence(IProcess process, bool processActive, params long[] nextCells);
    }
}
