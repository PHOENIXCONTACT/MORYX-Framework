// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.ControlSystem.Cells
{
    /// <summary>
    /// Base type for all implementations of <see cref="ICell"/>
    /// </summary>
    [Description("Base type for all cells within a production system")]
    public abstract class Cell : Resource, ICell
    {
        /// <summary>
        /// Context class used by the process engine to inform cell about updates
        /// </summary>
        public ProcessEngineContext ProcessEngineContext { get; private set; }

        /// <inheritdoc />
        IEnumerable<Session> ICell.ProcessEngineAttached(ProcessEngineContext context)
        {
            ProcessEngineContext = context;
            return ProcessEngineAttached();
        }

        /// <see cref="ICell.ProcessEngineAttached"/>
        protected abstract IEnumerable<Session> ProcessEngineAttached();

        /// <inheritdoc />
        IEnumerable<Session> ICell.ProcessEngineDetached()
        {
            ProcessEngineContext = null;
            return ProcessEngineDetached();
        }

        /// <see cref="ICell.ProcessEngineDetached"/>
        protected abstract IEnumerable<Session> ProcessEngineDetached();

        /// <inheritdoc />
        public abstract void StartActivity(ActivityStart activityStart);

        /// <inheritdoc />
        public virtual void ProcessAborting(Activity affectedActivity)
        {

        }

        /// <inheritdoc />
        public abstract void SequenceCompleted(SequenceCompleted completed);

        /// <summary>
        /// Publish a <see cref="ReadyToWork"/> from the resource
        /// </summary>
        public void PublishReadyToWork(ReadyToWork readyToWork)
        {
            ReadyToWork?.Invoke(this, readyToWork);
        }

        /// <inheritdoc />
        public event EventHandler<ReadyToWork> ReadyToWork;

        /// <summary>
        /// Publish a <see cref="NotReadyToWork"/> from the resource
        /// </summary>
        public void PublishNotReadyToWork(NotReadyToWork notReadyToWork)
        {
            NotReadyToWork?.Invoke(this, notReadyToWork);
        }

        /// <inheritdoc />
        public event EventHandler<NotReadyToWork> NotReadyToWork;

        /// <summary>
        /// Publish <see cref="ActivityCompleted"/> from the resource
        /// </summary>
        /// <param name="activityResult"></param>
        public void PublishActivityCompleted(ActivityCompleted activityResult)
        {
            ActivityCompleted?.Invoke(this, activityResult);
        }

        /// <inheritdoc />
        public event EventHandler<ActivityCompleted> ActivityCompleted;
    }
}
