using System.Collections;
using System.Diagnostics;
using Marvin.StateMachines;

namespace Marvin.Collections
{
    internal interface IDelayQueueContext : IStateContext
    {
        Stopwatch Stopwatch { get; }

        int QueueDelay { get; }

        Queue PendingMessages { get; set; }

        void ExecuteDequeue(object obj);

        void DequeueDelayed(object next, int queueDelay);

        void ExecuteDequeueParallel(object obj);
    }
}