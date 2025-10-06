// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.ControlSystem.Cells
{
    /// <summary>
    /// Base type for all implementations of <see cref="ICell"/>
    /// </summary>
    [Description("Base type for all cells within a production system")]
    public abstract class Cell : Resource, ICell
    {
        /// <inheritdoc />
        public abstract IEnumerable<Session> ControlSystemAttached();

        /// <inheritdoc />
        public abstract IEnumerable<Session> ControlSystemDetached();

        /// <inheritdoc />
        public abstract void StartActivity(ActivityStart activityStart);

        /// <inheritdoc />
        public virtual void ProcessAborting(IActivity affectedActivity) { }

        /// <inheritdoc />
        public abstract void SequenceCompleted(SequenceCompleted completed);
        private ICapabilities _capabilities = NullCapabilities.Instance;

        public ICapabilities Capabilities
        {
            get
            {
                return _capabilities;
            }
            protected set
            {
                _capabilities = value;
                this.CapabilitiesChanged?.Invoke(this, _capabilities);
            }
        }
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
        public event EventHandler<ICapabilities> CapabilitiesChanged;
    }
}
