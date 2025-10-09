// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Products;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.Processes;
using Moryx.Logging;
using Moryx.Modules;
using Moryx.Runtime.Modules;
using Moryx.Threading;

namespace Moryx.ControlSystem.ProcessEngine
{
    internal class ProcessControlFacade : IProcessControl, IFacadeControl
    {
        #region Dependencies

        public Action ValidateHealthState { get; set; }

        public IActivityPool ActivityPool { get; set; }

        public IActivityDataPool ActivityDataPool { get; set; }

        public IProcessArchive ProcessArchive { get; set; }

        public IModuleLogger Logger { get; set; }

        public IParallelOperations ParallelOperations { get; set; }

        #endregion

        #region Fields and Properties

        /// <inheritdoc />
        public IReadOnlyList<IProcess> RunningProcesses
        {
            get
            {
                ValidateHealthState();
                return ActivityDataPool.Processes.Select(p => p.Process).ToArray();
            }
        }

        #endregion

        public void Activate()
        {
            ActivityPool.ProcessUpdated += ParallelOperations.DecoupleListener<ProcessUpdatedEventArgs>(OnProcessChanged);
            ActivityPool.ActivityUpdated += ParallelOperations.DecoupleListener<ActivityUpdatedEventArgs>(OnActivityChanged);
        }

        public void Deactivate()
        {
            ActivityPool.ProcessUpdated -= ParallelOperations.RemoveListener<ProcessUpdatedEventArgs>(OnProcessChanged);
            ActivityPool.ActivityUpdated -= ParallelOperations.RemoveListener<ActivityUpdatedEventArgs>(OnActivityChanged);
        }

        public IReadOnlyList<IProcess> GetProcesses(ProductInstance productInstance)
        {
            ValidateHealthState();
            return ProcessArchive.GetProcesses(productInstance);
        }

        public IReadOnlyList<ICell> Targets(IProcess process)
        {
            ValidateHealthState();
            return ActivityDataPool.GetProcess(process)?.NextTargets() ?? Array.Empty<ICell>();
        }

        public IReadOnlyList<ICell> Targets(IActivity activity)
        {
            ValidateHealthState();
            return ActivityDataPool.GetByActivity(activity)?.Targets ?? Array.Empty<ICell>();
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
