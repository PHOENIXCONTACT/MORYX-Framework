﻿// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
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

        #endregion

        public void Start()
        {
            using var uow = UnitOfWorkFactory.Create();

            var operationRepo = uow.GetRepository<IOperationEntityRepository>();
            // Restore only operations which are not completed
            var restored = operationRepo.Linq.Active().Where(o => o.State < OperationDataStateBase.CompletedKey).ToArray();

            foreach (var entity in restored)
                RestoreByEntity(entity);

            _lock.EnterReadLock();
            var operations = _operations.ToList();
            _lock.ExitReadLock();

            foreach (var operationData in operations)
                operationData.Resume();
        }

        public void Stop()
        {
            _lock.EnterWriteLock();

            _operations.ForEach(DetachFromOperationEvents);
            _operations.Clear();

            _lock.ExitWriteLock();
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

        public IOperationData Get(Guid identifier)
        {
            Logger.Log(LogLevel.Debug, "Trying to fetch operation with identifier: {0}.", identifier);

            return GetOperationByIdentifier(o => o.Identifier == identifier, o => o.Identifier == identifier);
        }

        /// <summary>
        /// Will return the operation with the given order and operation numbers
        /// </summary>
        public IOperationData Get(string orderNumber, string operationNumber)
        {
            Logger.Log(LogLevel.Debug, "Trying to fetch operation with order and operation: {0}-{1}.", orderNumber, operationNumber);

            return GetOperationByIdentifier(o => o.Number.Equals(operationNumber) && o.OrderData.Number.Equals(orderNumber),
                o => o.Number.Equals(operationNumber) && o.Order.Number.Equals(orderNumber));
        }

        private IOperationData GetOperationByIdentifier(Func<IOperationData, bool> predicate, Expression<Func<OperationEntity, bool>> entityPredicate)
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
                else if (results.Count() > 1)
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
                        operationData = results.FirstOrDefault(o => o.State.Classification != OperationClassification.Completed);
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

            operationData = RestoreByEntity(operationEntity);

            return operationData;
        }

        public IOperationData Add(OperationCreationContext context, IOperationSource source)
        {
            // Get the order from the current operation list
            var orderData = GetOrderByNumber(context.Order.Number);
            // If there is not the needed order then try to get it from the database. May be this is an old order
            if (orderData == null)
            {
                using var uow = UnitOfWorkFactory.Create();

                var entity = uow.GetRepository<IOrderEntityRepository>().GetByNumber(context.Order.Number);
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
            var operationData = OperationFactory.Create();
            operationData.Initialize(context, orderData, source);

            AttachToOperationEvents(operationData);

            _lock.EnterWriteLock();
            _operations.Add(operationData);
            _lock.ExitWriteLock();

            // Decouple creation
            Task.Run(operationData.Assign);

            return operationData;
        }

        private IOperationData RestoreByEntity(OperationEntity entity)
        {
            var orderData = GetOrderByNumber(entity.Order.Number) ??
                                    OperationStorage.LoadOrder(entity.Order);

            var operationData = OperationFactory.Create();
            operationData.Initialize(entity, orderData);

            var restoreTask = operationData.Restore();

            // TODO: Run synchronously until start process is task based
            restoreTask.Wait();

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

        Operation IOperationPool.Get(Guid identifier) =>
            Get(identifier)?.Operation;

        Operation IOperationPool.Get(string orderNumber, string operationNumber) =>
            Get(orderNumber, operationNumber)?.Operation;

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
            var operationData = (IOperationData)sender;

            // Only save the operation of the classification is more than just ready
            // Initial or ready operations have not to be stored because they can be created again any time from the ERP system
            if (operationData.State.Classification < OperationClassification.Ready)
            {
                OperationUpdated?.Invoke(this, eventArgs);
                return;
            }

            _lock.EnterWriteLock();
            try
            {
                using (var uow = UnitOfWorkFactory.Create())
                {
                    var orderData = operationData.OrderData;
                    var orderId = ((IPersistentObject)orderData).Id;
                    var orderEntity = orderId == 0
                        ? OperationStorage.SaveOrder(uow, orderData)
                        : uow.GetRepository<IOrderEntityRepository>().GetByKey(orderId);

                    var operationEntity = OperationStorage.SaveOperation(uow, operationData);
                    operationEntity.Order = orderEntity;

                    uow.SaveChanges();
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }

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

            // Mark deleted in database
            using (var uow = UnitOfWorkFactory.Create())
            {
                OperationStorage.RemoveOperation(uow, operationData);
                uow.SaveChanges();
            }

            OperationAborted?.Invoke(this, eventArgs);
        }

        public event EventHandler<StartedEventArgs> OperationStarted;
        private void OnOperationStarted(object sender, StartedEventArgs eventArgs)
            => OperationStarted?.Invoke(this, eventArgs);

        public event EventHandler<ReportEventArgs> OperationCompleted;
        private void OnOperationCompleted(object sender, ReportEventArgs eventArgs)
            => OperationCompleted?.Invoke(this, eventArgs);

        public event EventHandler<ReportEventArgs> OperationInterrupted;
        private void OnOperationInterrupted(object sender, ReportEventArgs eventArgs)
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
