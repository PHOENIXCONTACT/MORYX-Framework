// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.Logging;
using Moryx.Threading;

namespace Moryx.ControlSystem.ProcessEngine.Jobs
{
    /// <summary>
    /// Default IJobDispatcher implementation
    /// </summary>
    [Component(LifeCycle.Singleton, typeof(IJobDispatcher))]
    internal class JobDispatcher : IJobDispatcher, ILoggingComponent
    {
        #region Dependencies

        public IJobDataList JobList { get; set; }

        public IFailurePredictor FailurePredictor { get; set; }

        public IProcessController ProcessController { get; set; }

        public IModuleLogger Logger { get; set; }

        public IParallelOperations ParallelOperations { get; set; }

        /// <summary>
        /// Module configuration to configure sample size for the dispatcher shutdown
        /// </summary>
        public ModuleConfig Config { get; set; }
        
        #endregion

        /// <inheritdoc cref="IJobDispatcher"/>
        public void Start()
        {
            ProcessController.ProcessChanged += ParallelOperations.DecoupleListener<ProcessEventArgs>(OnProcessChanged);
            FailurePredictor.ProcessWillFail += OnProcessWillFail;
        }

        public bool RebootCompleteInitial => Config.RebootCompleteInitial;

        /// <inheritdoc cref="IJobDispatcher"/>
        public void Stop()
        {
            foreach (var jobData in JobList.ToList())
                jobData.Interrupt();
        }

        /// <inheritdoc cref="IJobDispatcher"/>
        public void Dispose()
        {
            ProcessController.ProcessChanged -= ParallelOperations.RemoveListener<ProcessEventArgs>(OnProcessChanged);
            FailurePredictor.ProcessWillFail -= OnProcessWillFail;
        }

        private static void OnProcessChanged(object sender, ProcessEventArgs args)
        {
            var processData = args.ProcessData;
            processData.Job.ProcessChanged(processData, args.Trigger);
        }

        private static void OnProcessWillFail(object sender, ProcessData processData) =>
            processData.Job.FailurePredicted(processData);

        /// <inheritdoc cref="IJobDispatcher"/>
        public void LoadProcesses(IJobData jobData)
        {
            var restoredProcesses = ProcessController.LoadProcesses(jobData);
            jobData.AddProcesses(restoredProcesses);
        }

        /// <inheritdoc cref="IJobDispatcher"/>
        public void StartProcess(IJobData jobData)
        {
            // Create new process, generate id and add to job
            var process = jobData.Recipe.CreateProcess();
            var processData = new ProcessData(process) {Job = jobData};
            process.Id = IdShiftGenerator.Generate(jobData.Id, jobData.AllProcesses.Count);
            jobData.AddProcess(processData);
            // Pass to controller
            ProcessController.Start(processData);
        }

        /// <inheritdoc cref="IJobDispatcher"/>
        public void Resume(IJobData jobData) =>
            ProcessController.Resume(jobData.RunningProcesses);

        /// <inheritdoc cref="IJobDispatcher"/>
        public void Complete(IJobData jobData)
        {
            var cached = jobData.RunningProcesses[jobData.RunningProcesses.Count - 1];
            ProcessController.Interrupt(new[] {cached}, false);
        }

        /// <inheritdoc cref="IJobDispatcher"/>
        public void Abort(IJobData jobData) =>
            ProcessController.Interrupt(jobData.RunningProcesses, true);

        /// <inheritdoc cref="IJobDispatcher"/>
        public void Cleanup(IJobData jobData) =>
            ProcessController.Cleanup(jobData.RunningProcesses);

        /// <inheritdoc cref="IJobDispatcher"/>
        public void Interrupt(IJobData jobData) =>
            ProcessController.Interrupt(jobData.RunningProcesses, false);
    }
}
