// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using System.Diagnostics;
using Moryx.StateMachines;

namespace Moryx.Collections
{
    internal interface IDelayQueueContext : IStateContext
    {
        Stopwatch Stopwatch { get; }

        int QueueDelay { get; }

        Queue PendingItems { get; }

        void ExecuteDequeue(object item);

        void DequeueDelayed(object next, int queueDelay);

        void ExecuteDequeueParallel(object item);
    }
}
