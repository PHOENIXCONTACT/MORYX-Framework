using Marvin.StateMachines;

namespace Marvin.Collections
{
    internal abstract class QueueStateBase : StateBase<IDelayQueueContext>
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