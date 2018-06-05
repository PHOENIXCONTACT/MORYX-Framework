using System;

namespace Marvin.Collections
{
    internal class QueueStoppedState : QueueStateBase
    {
        public QueueStoppedState(IDelayQueueContext context, StateMap stateMap) : base(context, stateMap)
        {
        }

        public override void Start()
        {
            NextState(QueueIdleState);

            Context.Stopwatch.Start();
        }

        public override void Enqueue(object item)
        {
            throw new InvalidOperationException("The queue is not running.");
        }
    }
}