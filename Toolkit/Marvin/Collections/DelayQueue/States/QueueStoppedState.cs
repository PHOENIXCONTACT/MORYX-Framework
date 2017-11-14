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
    }
}