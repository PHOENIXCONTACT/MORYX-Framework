// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Products;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.Processes;
using Moryx.Logging;
using Moryx.Runtime.Modules;
using Moryx.Threading;

namespace Moryx.ControlSystem.ProcessEngine
{
    internal class ProcessControlFacade : FacadeBase, IProcessControl
    {
        #region Dependencies

        public IActivityPool ActivityPool { get; set; }

        public IActivityDataPool ActivityDataPool { get; set; }

        public IProcessArchive ProcessArchive { get; set; }

        public ProcessRemoval ProcessRemoval { get; set; }

        public IModuleLogger Logger { get; set; }

        public IParallelOperations ParallelOperations { get; set; }

        #endregion

        public override void Activate()
        {
            base.Activate();
            ActivityPool.ProcessUpdated += ParallelOperations.DecoupleListener<ProcessUpdatedEventArgs>(OnProcessChanged);
            ActivityPool.ActivityUpdated += ParallelOperations.DecoupleListener<ActivityUpdatedEventArgs>(OnActivityChanged);
        }

        public override void Deactivate()
        {
            ActivityPool.ProcessUpdated -= ParallelOperations.RemoveListener<ProcessUpdatedEventArgs>(OnProcessChanged);
            ActivityPool.ActivityUpdated -= ParallelOperations.RemoveListener<ActivityUpdatedEventArgs>(OnActivityChanged);
            base.Deactivate();
        }

        public IReadOnlyList<Process> GetRunningProcesses()
        {
            return GetRunningProcesses(_ => true);
        }

        public IReadOnlyList<Process> GetRunningProcesses(Func<Process, bool> predicate)
        {
            ValidateHealthState();
            return ActivityDataPool.Processes.Select(p => p.Process).Where(predicate).ToArray();
        }

        public Task<IReadOnlyList<Process>> GetArchivedProcessesAsync(ProductInstance productInstance, CancellationToken cancellationToken = default)
        {
            ValidateHealthState();
            return ProcessArchive.GetProcesses(productInstance, cancellationToken);
        }

        public IAsyncEnumerable<IProcessChunk> GetArchivedProcessesAsync(ProcessRequestFilter filterType, DateTime start, DateTime end, long[] jobIds,
            CancellationToken cancellationToken = default)
        {
            ValidateHealthState();
            return ProcessArchive.GetProcesses(filterType, start, end, jobIds, cancellationToken);
        }

        public IReadOnlyList<ICell> Targets(Process process)
        {
            ValidateHealthState();
            return ActivityDataPool.GetProcess(process)?.NextTargets() ?? Array.Empty<ICell>();
        }

        public IReadOnlyList<ICell> Targets(Activity activity)
        {
            ValidateHealthState();
            return ActivityDataPool.GetByActivity(activity)?.Targets ?? Array.Empty<ICell>();
        }

        public void Report(Process process, ReportAction action)
        {
            ValidateHealthState();
            ProcessRemoval.Report(process, action);
        }

        private void OnProcessChanged(object sender, ProcessUpdatedEventArgs args)
        {
            ProcessUpdated?.Invoke(this, args);
        }

        private void OnActivityChanged(object sender, ActivityUpdatedEventArgs args)
        {
            ActivityUpdated?.Invoke(this, args);
        }

        /// <inheritdoc />
        public event EventHandler<ProcessUpdatedEventArgs> ProcessUpdated;

        public event EventHandler<ActivityUpdatedEventArgs> ActivityUpdated;
    }
}
