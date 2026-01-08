// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.ControlSystem.ProcessEngine.Model;
using Moryx.Model.Repositories;
using Moryx.Threading;

namespace Moryx.ControlSystem.ProcessEngine.Processes
{
    /// <summary>
    /// The main class of the ProcessController module.
    /// </summary>
    [Component(LifeCycle.Singleton, typeof(IProcessController), typeof(IActivityPoolListener))]
    internal sealed class ProcessController : IProcessController, IActivityPoolListener, IDisposable
    {
        #region Dependencies

        /// <summary>
        /// Pool of open and running activities
        /// !Injected!
        /// </summary>
        public IActivityDataPool ActivityPool { get; set; }

        /// <summary>
        /// Parallel operations for async process storage
        /// </summary>
        public IParallelOperations ParallelOperations { get; set; }

        /// <summary>
        /// Factory to the ControlSystem model
        /// </summary>
        public IUnitOfWorkFactory<ProcessContext> UnitOfWorkFactory { get; set; }

        /// <summary>
        /// Injected process storage to load processes
        /// </summary>
        public IProcessStorage ProcessStorage { get; set; }

        #endregion

        /// <summary>
        /// WARNING: Do this as the very first. The event execution must be redefined
        /// </summary>
        public int StartOrder => 0;

        public void Initialize()
        {
            ActivityPool.ProcessChanged += OnProcessChanged;
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void Dispose()
        {
            ActivityPool.ProcessChanged -= OnProcessChanged;
        }

        private void OnProcessChanged(object sender, ProcessEventArgs args)
        {
            switch (args.Trigger)
            {
                case ProcessState.Running:
                case ProcessState.Interrupted:
                case ProcessState.Discarded:
                case ProcessState.Success:
                case ProcessState.Failure:
                    // Only forward events relevant to the job state-machine
                    // ReSharper disable once PossibleNullReferenceException
                    ProcessChanged(this, args);
                    break;
            }
        }

        public IReadOnlyList<ProcessData> LoadProcesses(IJobData job)
        {
            using var uow = UnitOfWorkFactory.Create();
            // Restore completed processes
            ProcessStorage.LoadCompletedProcesses(uow, job, ((JobDataBase)job).AllProcesses);

            // Restore running processes
            var processes = ProcessStorage.GetRunningProcesses(uow, job);
            foreach (var processData in processes)
            {
                // Add to pool and return to caller
                processData.Job = job;
                ActivityPool.AddProcess(processData);
            }

            return processes;
        }

        public void Start(ProcessData processData)
        {
            // Add to pool and return to caller
            processData.State = ProcessState.Ready;
            ActivityPool.AddProcess(processData);
        }

        public void Interrupt(IEnumerable<ProcessData> processes, bool abort)
        {
            foreach (var process in processes)
            {
                var state = abort ? ProcessState.Aborting : ProcessState.Stopping;
                ActivityPool.UpdateProcess(process, state);
            }
        }

        public void Cleanup(IEnumerable<ProcessData> processes)
        {
            foreach (ProcessData processData in processes)
            {
                ActivityPool.UpdateProcess(processData, ProcessState.CleaningUp);
            }
        }

        public void Resume(IEnumerable<ProcessData> processes)
        {
            foreach (ProcessData processData in processes)
            {
                ActivityPool.UpdateProcess(processData, ProcessState.Restored);
            }
        }

        public event EventHandler<ProcessEventArgs> ProcessChanged;
    }
}
