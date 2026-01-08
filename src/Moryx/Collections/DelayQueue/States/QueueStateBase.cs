// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.StateMachines;

namespace Moryx.Collections
{
    internal abstract class QueueStateBase : SyncStateBase<IDelayQueueContext>
    {
        protected QueueStateBase(IDelayQueueContext context, StateMap stateMap) : base(context, stateMap)
        {
        }

        public virtual void Start()
        {
        }

        public virtual void Stop()
        {
        }

        public virtual void Enqueue(object item)
        {
        }

        public virtual void DelayedDequeue(object item)
        {
        }

        public virtual void MessageSent(object item)
        {
        }

        [StateDefinition(typeof(QueuePendingState))]
        public const int PendingItemsState = 10;

        [StateDefinition(typeof(QueueIdleState))]
        public const int QueueIdleState = 20;

        [StateDefinition(typeof(QueueStoppedState), IsInitial = true)]
        public const int StoppedState = 30;
    }
}
