using System;
using System.Collections.Generic;
using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Processes;

namespace Moryx.ControlSystem.ProcessEngine.Jobs
{
    /// <summary>
    /// Public representation of a job
    /// </summary>
    internal interface IJobData
    {
        /// <summary>
        /// This Job's ID
        /// </summary>
        long Id { get; }

        /// <summary>
        /// The number of items to complete
        /// </summary>
        int Amount { get; }

        /// <summary>
        /// Current recipe of this job
        /// </summary>
        IWorkplanRecipe Recipe { get; }

        /// <summary>
        /// Start of the job
        /// </summary>
        DateTime Started { get; }

        /// <summary>
        /// End of the job
        /// </summary>
        DateTime Completed { get; }

        /// <summary>
        /// Job wrapped by this data instance
        /// </summary>
        EngineJob Job { get; }

        /// <summary>
        /// Public representation of the job
        /// </summary>
        IJobState State { get; }

        /// <summary>
        /// Current job classification
        /// </summary>
        JobClassification Classification { get; }

        /// <summary>
        /// The current recipe provider of this job
        /// </summary>
        IRecipeProvider RecipeProvider { get; }

        /// <summary>
        /// List of processes references to this job
        /// </summary>
        IReadOnlyList<ProcessData> RunningProcesses { get; }

        /// <summary>
        /// All processes of the job
        /// </summary>
        IReadOnlyList<ProcessData> AllProcesses { get; }

        /// <summary>
        /// <c>true</c> if the job can be completed
        /// </summary>
        bool CanComplete { get; }

        /// <summary>
        /// <c>true</c> if the job can be aborted
        /// </summary>
        bool CanAbort { get; }

        /// <summary>
        /// <c>true</c> if the job is in a stable state
        /// </summary>
        bool IsStable { get; }

        /// <summary>
        /// Called if the job was prepared by the job management
        /// </summary>
        void Ready();

        /// <summary>
        /// Activates the job. Moves the job to a state where processes will be dispatched to
        /// the process controller
        /// </summary>
        void Start();

        /// <summary>
        /// Deactivates the job. Moves the job to a state where no processes will be dispatched anymore.
        /// </summary>
        void Stop();

        /// <summary>
        /// Aborts the job. No new processes will be dispatched.
        /// Running processes will be aborted with an unmount activity.
        /// </summary>
        void Abort();

        /// <summary>
        /// Job was reloaded from database and should try to restore processes.
        /// In case the current state is not supposed to be shutdown this can be considered as a chance to switch to cleanup.
        /// </summary>
        void Load();

        /// <summary>
        /// Interrupts the job. No new processes will be dispatched.
        /// Current processes will also be interrupted.
        /// </summary>
        void Interrupt();

        /// <summary>
        /// Completes the job. No new processes will be dispatched. Running processes
        /// will be finished
        /// </summary>
        void Complete();

        /// <summary>
        /// Add a process to the job
        /// </summary>
        void AddProcess(ProcessData processData);

        /// <summary>
        /// Add processes to the job
        /// </summary>
        void AddProcesses(IReadOnlyList<ProcessData> processes);

        /// <summary>
        /// Will handle process changes from the process controller components
        /// All changes will be given to the state machine to do further handling
        /// </summary>
        void ProcessChanged(ProcessData processData, ProcessState trigger);

        /// <summary>
        /// The process engine has predicted that a process will fail
        /// </summary>
        void FailurePredicted(ProcessData processData);

        /// <summary>
        /// Event raised when the jobs progress changes
        /// </summary>
        event EventHandler ProgressChanged;

        /// <summary>
        /// Event raised when the job state changed
        /// </summary>
        event EventHandler<JobStateEventArgs> StateChanged;
    }
}