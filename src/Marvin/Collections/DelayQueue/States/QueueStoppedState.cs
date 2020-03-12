// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

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
