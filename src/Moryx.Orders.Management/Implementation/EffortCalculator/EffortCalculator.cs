﻿// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Container;
using Moryx.ControlSystem.Jobs;

namespace Moryx.Orders.Management
{
    [Component(LifeCycle.Singleton, typeof(IEffortCalculator))]
    internal class EffortCalculator : IEffortCalculator
    {
        public IJobManagement JobManagement { get; set; }

        public IOperationDataPool OperationDataPool { get; set; }

        public void Start()
        {
            OperationDataPool.OperationUpdated += OnOperationUpdated;
            JobManagement.EvaluationsOutdated += OnEvaluationsOutdated;
        }

        public void Stop()
        {
            OperationDataPool.OperationUpdated -= OnOperationUpdated;
            JobManagement.EvaluationsOutdated += OnEvaluationsOutdated;
        }

        public void Dispose()
        {
        }

        private void OnOperationUpdated(object sender, OperationEventArgs operationEventArgs)
        {
        }


        private void OnEvaluationsOutdated(object sender, EventArgs eventArgs)
        {
        }
    }
}

