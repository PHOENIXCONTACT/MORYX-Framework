// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Linq;
using Moryx.AbstractionLayer.Products;
using Moryx.Container;
using Moryx.Modules;
using Moryx.Orders;
using Moryx.Threading;

namespace Moryx.ProcessData.Adapter.OrderManagement
{
    [Plugin(LifeCycle.Singleton)]
    internal class OrderManagementAdapter : IPlugin
    {
        private const string MeasurementPrefix = "orders_";

        #region Dependencies

        public IOrderManagement OrderManagement { get; set; }

        public IProcessDataMonitor ProcessDataMonitor { get; set; }

        public IParallelOperations ParallelOperations { get; set; }

        public ModuleConfig ModuleConfig { get; set; }

        #endregion

        /// <inheritdoc />
        public void Start()
        {
            OrderManagement.OperationStarted += OnOperationStateChanged;
            OrderManagement.OperationInterrupted += OnOperationStateChanged;
            OrderManagement.OperationCompleted += OnOperationStateChanged;

            OrderManagement.OperationProgressChanged += OnOperationProgressChanged;
        }

        /// <summary>
        /// Stops the adapter component
        /// </summary>
        public void Stop()
        {
            OrderManagement.OperationStarted -= OnOperationStateChanged;
            OrderManagement.OperationInterrupted -= OnOperationStateChanged;
            OrderManagement.OperationCompleted -= OnOperationStateChanged;

            OrderManagement.OperationProgressChanged -= OnOperationProgressChanged;
        }

        private readonly ICollection<Operation> _pendingProgressChanges = new List<Operation>();
        private int _timerId;

        private void OnOperationProgressChanged(object sender, OperationChangedEventArgs e)
        {
            var operation = e.Operation;
            lock (_pendingProgressChanges)
            {
                // If operation is currently in queue, replace facade object
                var existing = _pendingProgressChanges.FirstOrDefault(o => o.Identifier == operation.Identifier);
                if (existing != null)
                    _pendingProgressChanges.Remove(existing);

                _pendingProgressChanges.Add(operation);
            }

            if (_timerId == 0)
                _timerId = ParallelOperations.ScheduleExecution(ReportProgressMeasurement, ModuleConfig.ReportInterval, -1);
        }

        private void ReportProgressMeasurement()
        {
            _timerId = 0;
            Operation[] changes;

            lock (_pendingProgressChanges)
            {
                changes = _pendingProgressChanges.ToArray();
                _pendingProgressChanges.Clear();
            }

            foreach (var operation in changes)
            {
                var progress = operation.Progress;
                var measurement = new Measurement(MeasurementPrefix + "operationProgress");

                measurement.Add(new DataField("running", progress.RunningCount));
                measurement.Add(new DataField("success", progress.SuccessCount));
                measurement.Add(new DataField("failure", progress.FailureCount));
                measurement.Add(new DataField("reworked", progress.ReworkedCount));
                measurement.Add(new DataField("scrap", progress.ScrapCount));
                measurement.Add(new DataField("pending", progress.PendingCount));

                measurement.Add(new DataTag("operation", operation.Number));
                measurement.Add(new DataTag("order", operation.Order.Number));

                var productIdentity = (ProductIdentity)operation.Product.Identity;
                measurement.Add(new DataTag("productIdent", productIdentity.Identifier));
                measurement.Add(new DataTag("productRev", productIdentity.Revision.ToString("D2")));

                ProcessDataMonitor.Add(measurement);
            }

        }

        private void OnOperationStateChanged(object sender, OperationChangedEventArgs args)
        {
            var operation = args.Operation;
            var measurement = new Measurement(MeasurementPrefix + "operationStates");
            measurement.Add(new DataField("classification", (int)operation.State));
            measurement.Add(new DataField("pending", operation.Progress.PendingCount));

            measurement.Add(new DataTag("name", operation.State.ToString()));
            measurement.Add(new DataTag("operation", operation.Number));
            measurement.Add(new DataTag("order", operation.Order.Number));

            ProcessDataMonitor.Add(measurement);
        }
    }
}
