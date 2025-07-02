namespace Moryx.ControlSystem.ProcessEngine.Processes
{
    /// <summary>
    /// Definition of process states
    /// Keep in mind that states can only increase, therefor they must follow a numerical order
    /// </summary>
    public enum ProcessState
    {
        /// <summary>
        /// Initial state of the process
        /// </summary>
        Initial = 0,

        /// <summary>
        /// Process was restored from database and is not ready for execution
        /// </summary>
        Restored = 10,

        /// <summary>
        /// Cleanup after previous uncontrolled shutdown
        /// </summary>
        CleaningUp = 50,

        /// <summary>
        /// The process is not yet started.
        /// </summary>
        Ready = 1000,

        /// <summary>
        /// Process is ready but was restored from database
        /// </summary>
        RestoredReady = 1100,

        /// <summary>
        /// Process is ready for cleanup
        /// </summary>
        CleaningUpReady = 1200,

        /// <summary>
        /// State of the process once the engine was started after process (re)creation
        /// </summary>
        EngineStarted = 1800,

        /// <summary>
        /// The process is started.
        /// </summary>
        Running = 2000,

        /// <summary>
        /// The process and its parts shall be removed from the machine before the enter
        /// the <see cref="Interrupted"/> state.
        /// </summary>
        Aborting = 2500,

        /// <summary>
        /// Removing a broken process from the machine
        /// </summary>
        RemoveBroken = 2550,

        /// <summary>
        /// Process is stopping
        /// </summary>
        Stopping = 2800,

        /// <summary>
        /// Process was started before but is currently interrupted
        /// </summary>
        Interrupted = 3000,

        /// <summary>
        /// The process was an not started cached instance, that was discarded
        /// </summary>
        Discarded = 3500,

        /// <summary>
        /// Flag if process execution created a positive result
        /// </summary>
        Success = 4000,

        /// <summary>
        /// Flag if process execution created a negative result
        /// </summary>
        Failure = 4500
    }
}