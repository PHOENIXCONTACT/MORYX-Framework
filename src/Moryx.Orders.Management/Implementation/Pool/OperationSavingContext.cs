// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Repositories;
using Moryx.Orders.Management.Model;

namespace Moryx.Orders.Management;

internal class OperationSavingContext : IOperationSavingContext
{
    private readonly IUnitOfWorkFactory<OrdersContext> _unitOfWorkFactory;

    public OperationSavingContext(IUnitOfWorkFactory<OrdersContext> unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    async Task IOperationSavingContext.SaveOperation(IOperationData operationData)
    {
        // Only save the operation of the classification is more than just ready
        // Initial or ready operations have not to be stored because they can be created again any time from the ERP system
        if (operationData.State.Classification < OperationStateClassification.Ready)
        {
            return;
        }

        using var uow = _unitOfWorkFactory.Create();

        var orderData = operationData.OrderData;
        var orderId = ((IPersistentObject)orderData).Id;
        var orderEntity = orderId == 0
            ? await OperationStorage.SaveOrder(uow, orderData)
            : await uow.GetRepository<IOrderEntityRepository>().GetByKeyAsync(orderId);

        var operationEntity = await OperationStorage.SaveOperation(uow, operationData);
        operationEntity.Order = orderEntity;

        await uow.SaveChangesAsync();
    }

    async Task IOperationSavingContext.RemoveOperation(IOperationData operationData)
    {
        using var uow = _unitOfWorkFactory.Create();
        OperationStorage.RemoveOperation(uow, operationData);

        await uow.SaveChangesAsync();
    }
}
