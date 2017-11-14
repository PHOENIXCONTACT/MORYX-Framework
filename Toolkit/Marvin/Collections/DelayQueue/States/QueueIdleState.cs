namespace Marvin.Collections
{
    /// <summary>
    /// Default state of the queue. Incoming messages are send if the delay requirement
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

        public override void Enqueue(object obj)
        {
            NextState(PendingMessageState);

            var currentTime = Context.Stopwatch.ElapsedMilliseconds;
            if (currentTime >= Context.QueueDelay)
            {
                Context.ExecuteDequeueParallel(obj);
            }
            else
            {
                var remainingDelay = Context.QueueDelay - currentTime;
                Context.DequeueDelayed(obj, (int)remainingDelay);
            }
        }
    }
}