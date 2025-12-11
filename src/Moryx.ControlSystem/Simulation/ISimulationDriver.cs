// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Drivers;
using Moryx.ControlSystem.Cells;

namespace Moryx.ControlSystem.Simulation
{
    /// <summary>
    /// Interface for driver based simulation
    /// </summary>
    public interface ISimulationDriver : IDriver
    {
        /// <summary>
        /// Currently simulated state
        /// </summary>
        SimulationState SimulatedState { get; }

        /// <summary>
        /// Cells that reference this simulation driver
        /// and expect process events from it
        /// </summary>
        IEnumerable<ICell> Usages { get; }

        /// <summary>
        /// Send a message to the cell through the driver to simulate that the
        /// physical cell is ready to execute the next step.
        /// </summary>
        /// <param name="activity">The activity objects gives access to all relevant information</param>
        void Ready(Activity activity);

        /// <summary>
        /// Send a result to the cell about the activity that just finished
        /// </summary>
        /// <param name="result">Simulated execution result</param>
        void Result(SimulationResult result);

        /// <summary>
        /// Event raised when the value of <see cref="SimulatedState"/> has changed
        /// </summary>
        public event EventHandler<SimulationState> SimulatedStateChanged;
    }

    /// <summary>
    /// Simulation result object
    /// </summary>
    public class SimulationResult
    {
        /// <summary>
        /// Numeric result of the activity, mappable to the result enum
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// Activity the result is for
        /// </summary>
        public Activity Activity { get; set; }
    }
}
