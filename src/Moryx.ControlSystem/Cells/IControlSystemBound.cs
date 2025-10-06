// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.Cells
{
    /// <summary>
    /// Interface for ControlSystem bound resources. Resources can provide their current sessions when the 
    /// control system was attached or detached
    /// </summary>
    public interface IControlSystemBound
    {
        /// <summary>
        /// Called if the control system was attached to production cells. 
        /// Can return currently active sessions within the cell
        /// </summary>
        IEnumerable<Session> ControlSystemAttached();

        /// <summary>
        /// Called if the control system was detached from production cells. 
        /// Can return currently active sessions within the cell
        /// </summary>
        IEnumerable<Session> ControlSystemDetached();
    }
}
