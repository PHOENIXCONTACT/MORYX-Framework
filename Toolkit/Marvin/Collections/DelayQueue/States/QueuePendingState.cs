using System.Collections;

namespace Marvin.Collections
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

        public override void Enqueue(object obj)
        {
            // Create queue on demand only if a third message appears within the time slice
            if(Context.PendingMessages == null)
                Context.PendingMessages = new Queue();

            Context.PendingMessages.Enqueue(obj);
        }

        public override void DelayedDequeue(object obj)
        {
            Context.ExecuteDequeue(obj);
        }

        public override void MessageSent(object obj)
        {
            if (Context.PendingMessages == null || Context.PendingMessages.Count == 0)
            {
                NextState(QueueIdleState);

                Context.Stopwatch.Restart();
            }
            else
            {
                var next = Context.PendingMessages.Dequeue();
                Context.DequeueDelayed(next, Context.QueueDelay);
            }
        }
    }
}