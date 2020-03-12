// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using System.Diagnostics;
using Marvin.StateMachines;

namespace Marvin.Collections
{
    internal interface IDelayQueueContext : IStateContext
    {
        Stopwatch Stopwatch { get; }

        int QueueDelay { get; }

        Queue PendingItems { get; set; }

        void ExecuteDequeue(object item);

        void DequeueDelayed(object next, int queueDelay);

        void ExecuteDequeueParallel(object item);
    }
}
