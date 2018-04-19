namespace Marvin.Collections
{
    /// <summary>
    /// Default state of the queue. Incoming items are dequeued if the delay requirement
    /// is met and the stop watch restarted.
    /// </summary>
    internal class QueueIdleState : QueueStateBase
    {
        public QueueIdleState(IDelayQueueContext context, StateMap stateMap) : base(context, stateMap)
        {
        }

        public override void Stop()
        {
            NextState(StoppedState);
            Context.Stopwatch.Reset();
        }

        public override void Enqueue(object item)
        {
            NextState(PendingItemsState);

            var currentTime = Context.Stopwatch.ElapsedMilliseconds;
            if (currentTime >= Context.QueueDelay)
            {
                Context.ExecuteDequeueParallel(item);
            }
            else
            {
                var remainingDelay = Context.QueueDelay - currentTime;
                Context.DequeueDelayed(item, (int)remainingDelay);
            }
        }
    }
}