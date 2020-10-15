// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;

namespace Moryx.Collections
{
    /// <summary>
    /// State when one or more messages await transmition
    /// </summary>
    internal class QueuePendingState : QueueStateBase
    {
        public QueuePendingState(IDelayQueueContext context, StateMap stateMap) : base(context, stateMap)
        {
        }

        public override void Stop()
        {
            NextState(StoppedState);

            Context.Stopwatch.Reset();
        }

        public override void Enqueue(object item)
        {
            Context.PendingItems.Enqueue(item);
        }

        public override void DelayedDequeue(object item)
        {
            Context.ExecuteDequeue(item);
        }

        public override void MessageSent(object item)
        {
            if (Context.PendingItems.Count == 0)
            {
                NextState(QueueIdleState);

                Context.Stopwatch.Restart();
            }
            else
            {
                var next = Context.PendingItems.Dequeue();
                Context.DequeueDelayed(next, Context.QueueDelay);
            }
        }
    }
}
