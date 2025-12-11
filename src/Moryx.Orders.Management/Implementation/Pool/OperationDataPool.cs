// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Model;
using Moryx.Model.Repositories;
using Moryx.Orders.Management.Model;
using Moryx.Tools;

namespace Moryx.Orders.Management
{
    [Component(LifeCycle.Singleton, typeof(IOperationDataPool), typeof(IOperationPool))]
    internal class OperationDataPool : IOperationDataPool, IOperationPool
    {
        #region Dependencies

        public IUnitOfWorkFactory<OrdersContext> UnitOfWorkFactory { get; set; }

        public IModuleLogger Logger { get; set; }

        public IOperationFactory OperationFactory { get; set; }

        #endregion

        #region Fields and Properties

        private readonly IList<IOperationData> _operations = new List<IOperationData>();
        private readonly ReaderWriterLockSlim _lock = new();
        private OperationSavingContext _savingContext;

        #endregion

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            _savingContext = new OperationSavingContext(UnitOfWorkFactory);

            using var uow = UnitOfWorkFactory.Create();

            var operationRepo = uow.GetRepository<IOperationEntityRepository>();
            // Restore only operations which are not completed
            var restored = await operationRepo.Linq.Active()
                .Where(o => o.State < OperationDataStateBase.CompletedKey).ToArrayAsync();

            foreach (var entity in restored)
                await RestoreByEntity(entity);

            _lock.EnterReadLock();
            var operations = _operations.ToList();
            _lock.ExitReadLock();

            foreach (var operationData in operations)
                await operationData.Resume();
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            _lock.EnterWriteLock();

            _operations.ForEach(DetachFromOperationEvents);
            _operations.Clear();

            _lock.ExitWriteLock();

            _savingContext = null;

            return Task.CompletedTask;
        }

        public IReadOnlyList<IOperationData> GetAll() =>
            GetAll(_ => true);

        public IOperationData Get(Operation operation)
        {
            return GetAll(data => data.Operation == operation).FirstOrDefault();
        }

        public IReadOnlyList<IOrderData> GetOrders()
        {
            _lock.EnterReadLock();
            var orders = _operations.Select(o => o.OrderData).Distinct().ToArray();
            _lock.ExitReadLock();

            return orders;
        }

        public IReadOnlyList<IOperationData> GetAll(Func<IOperationData, bool> filter)
        {
            try
            {
                _lock.EnterReadLock();
                var operations = _operations.Where(filter).ToList();
                return operations;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public Task<IOperationData> Get(Guid identifier)
        {
            Logger.Log(LogLevel.Debug, "Trying to fetch operation with identifier: {0}.", identifier);

            return GetOperationByIdentifier(o => o.Identifier == identifier, o => o.Identifier == identifier);
        }

        /// <summary>
        /// Will return the operation with the given order and operation numbers
        /// </summary>
        public Task<IOperationData> Get(string orderNumber, string operationNumber)
        {
            Logger.Log(LogLevel.Debug, "Trying to fetch operation with order and operation: {0}-{1}.", orderNumber, operationNumber);

            return GetOperationByIdentifier(o => o.Number.Equals(operationNumber) && o.OrderData.Number.Equals(orderNumber),
                o => o.Number.Equals(operationNumber) && o.Order.Number.Equals(orderNumber));
        }

        private async Task<IOperationData> GetOperationByIdentifier(Func<IOperationData, bool> predicate, Expression<Func<OperationEntity, bool>> entityPredicate)
        {
            IOperationData operationData = null;
            try
            {
                _lock.EnterReadLock();

                var results = _operations.Where(predicate).ToList();

                if (results.Count == 1)
                {
                    operationData = results[0];
                }
                else if (results.Count > 1)
                {
                    var firstElement = results.First();
                    if (results.All(x => ReferenceEquals(firstElement, x)))
                    {
                        Logger.Log(LogLevel.Error, "More than 1 operation was found for the given constraint. All results represent the same object");
                        operationData = firstElement;
                    }
                    else
                    {
                        Logger.Log(LogLevel.Error, "More than 1 operation was found for the given constraint. Picking the operation with the most plausible state");
                        operationData = results.FirstOrDefault(o => o.State.Classification != OperationStateClassification.Completed);
                    }
                }
                else
                {
                    Logger.Log(LogLevel.Warning, "No operation was found. Trying to load from database");
                }
            }
            finally
            {
                _lock.ExitReadLock();
            }

            if (operationData != null)
                return operationData;

            // Look into the database because the requested order and operation could be already completed
            using var uow = UnitOfWorkFactory.Create();

            var operationEntity = uow.GetRepository<IOperationEntityRepository>().Linq.Active()
                .SingleOrDefault(entityPredicate);

            if (operationEntity == null)
                return null;

            operationData = await RestoreByEntity(operationEntity);

            return operationData;
        }

        public async Task<IOperationData> Add(OperationCreationContext context, IOperationSource source)
        {
            // Get the order from the current operation list
            var orderData = GetOrderByNumber(context.Order.Number);
            // If there is not the needed order then try to get it from the database. Maybe this is an old order
            if (orderData == null)
            {
                using var uow = UnitOfWorkFactory.Create();

                var entity = await uow.GetRepository<IOrderEntityRepository>().Linq
                    .FirstOrDefaultAsync(o => o.Number.Equals(context.Order.Number));

                // Load an OrderData object from the storage with the given entity
                // or create a new one if the needed order was not in the database
                if (entity == null)
                {
                    orderData = new OrderData();
                    orderData.Initialize(context.Order);
                }
                else
                {
                    orderData = OperationStorage.LoadOrder(entity);
                }
            }

            // If operation is existing and not completed, it can be returned without creating a new one
            var existingOperation = orderData.Operations.FirstOrDefault(o => o.Number.Equals(context.Number) && o.State.Key < OperationDataStateBase.CompletedKey);
            if (existingOperation != null)
                return existingOperation;

            // Create new operation
            var operationData = OperationFactory.Create(_savingContext);
            await operationData.Initialize(context, orderData, source);

            AttachToOperationEvents(operationData);

            _lock.EnterWriteLock();
            _operations.Add(operationData);
            _lock.ExitWriteLock();

            // Decouple creation
            _ = Task.Run(() =>
            {
                try
                {
                    operationData.Assign();
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Error during assigning of operation {operationIdentifier}", operationData.Identifier);
                }
            });

            return operationData;
        }

        private async Task<IOperationData> RestoreByEntity(OperationEntity entity)
        {
            var orderData = GetOrderByNumber(entity.Order.Number) ??
                                    OperationStorage.LoadOrder(entity.Order);

            var operationData = OperationFactory.Create(_savingContext);
            await operationData.Initialize(entity, orderData);

            await operationData.Restore();

            AttachToOperationEvents(operationData);

            _lock.EnterWriteLock();
            _operations.Add(operationData);
            _lock.ExitWriteLock();

            return operationData;
        }

        private IOrderData GetOrderByNumber(string orderNumber)
        {
            var orders = GetOrders();
            return orders.FirstOrDefault(o => o.Number == orderNumber);
        }

        #region IOperationPool

        IReadOnlyList<Operation> IOperationPool.GetAll() =>
            GetAll().Select(data => data.Operation).ToArray();

        IReadOnlyList<Operation> IOperationPool.GetAll(Func<Operation, bool> filter) =>
            GetAll(data => filter(data.Operation)).Select(data => data.Operation).ToArray();

        async Task<Operation> IOperationPool.GetAsync(Guid identifier, CancellationToken cancellationToken = default) =>
            (await Get(identifier))?.Operation;

        async Task<Operation> IOperationPool.GetAsync(string orderNumber, string operationNumber, CancellationToken cancellationToken = default) =>
            (await Get(orderNumber, operationNumber))?.Operation;

        IReadOnlyList<Order> IOperationPool.GetOrders() =>
            GetOrders().Select(data => data.Order).ToArray();

        #endregion

        #region Events

        private void AttachToOperationEvents(IOperationData operationData)
        {
            operationData.Updated += OnOperationUpdated;
            operationData.Aborted += OnOperationAborted;
            operationData.Started += OnOperationStarted;
            operationData.Completed += OnOperationCompleted;
            operationData.Interrupted += OnOperationInterrupted;
            operationData.PartialReport += OnOperationPartialReport;
            operationData.Adviced += OnOperationAdviced;
            operationData.ProgressChanged += OnOperationProgressChanged;
        }

        private void DetachFromOperationEvents(IOperationData operationData)
        {
            operationData.Updated -= OnOperationUpdated;
            operationData.Aborted -= OnOperationAborted;
            operationData.Started -= OnOperationStarted;
            operationData.Completed -= OnOperationCompleted;
            operationData.Interrupted -= OnOperationInterrupted;
            operationData.PartialReport -= OnOperationPartialReport;
            operationData.Adviced -= OnOperationAdviced;
            operationData.ProgressChanged -= OnOperationProgressChanged;
        }

        private void OnOperationUpdated(object sender, OperationEventArgs eventArgs)
        {
            OperationUpdated?.Invoke(this, eventArgs);
        }

        public event EventHandler<OperationEventArgs> OperationAborted;

        private void OnOperationAborted(object sender, OperationEventArgs eventArgs)
        {
            var operationData = eventArgs.OperationData;

            DetachFromOperationEvents(operationData);

            _lock.EnterWriteLock();

            // Remove from pool and order
            _operations.Remove(operationData);
            operationData.OrderData.RemoveOperation(operationData);

            _lock.ExitWriteLock();

            OperationAborted?.Invoke(this, eventArgs);
        }

        public event EventHandler<StartedEventArgs> OperationStarted;
        private void OnOperationStarted(object sender, StartedEventArgs eventArgs)
            => OperationStarted?.Invoke(this, eventArgs);

        public event EventHandler<ReportEventArgs> OperationCompleted;
        private void OnOperationCompleted(object sender, ReportEventArgs eventArgs)
            => OperationCompleted?.Invoke(this, eventArgs);

        public event EventHandler<OperationEventArgs> OperationInterrupted;
        private void OnOperationInterrupted(object sender, OperationEventArgs eventArgs)
            => OperationInterrupted?.Invoke(this, eventArgs);

        public event EventHandler<ReportEventArgs> OperationPartialReport;

        private void OnOperationPartialReport(object sender, ReportEventArgs eventArgs)
            => OperationPartialReport?.Invoke(this, eventArgs);

        public event EventHandler<AdviceEventArgs> OperationAdviced;

        private void OnOperationAdviced(object sender, AdviceEventArgs eventArgs)
            => OperationAdviced?.Invoke(this, eventArgs);

        public event EventHandler<OperationEventArgs> OperationProgressChanged;

        private void OnOperationProgressChanged(object sender, OperationEventArgs eventArgs)
            => OperationProgressChanged?.Invoke(this, eventArgs);

        public event EventHandler<OperationEventArgs> OperationUpdated;

        #endregion
    }
}
