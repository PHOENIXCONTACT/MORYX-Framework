// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Microsoft.Extensions.Logging;
using Moryx.Logging;
using Moryx.Maintenance.Management.Components;
using Moryx.Runtime.Modules;

namespace Moryx.Maintenance.Management.Facade;

internal sealed class  MaintenanceManagementFacade : FacadeBase, IMaintenanceManagement
{
    #region Dependencies
    public required IMaintenanceManager MaintenanceManager { get; set; }
    #endregion

    public IReadOnlyList<MaintenanceOrder> Orders
        => MaintenanceManager.Orders;
    public IReadOnlyList<Acknowledgement> Acknowledgements
        => MaintenanceManager.Acknowledgements;
    public required IModuleLogger Logger { get; set; }

    public bool HasOverdueMaintenance =>
        MaintenanceManager.HasOverdueMaintenance;
    public event EventHandler<MaintenanceOrder>? MaintenanceOverdue;
    public event EventHandler<MaintenanceOrder>? MachineBreak;
    public event EventHandler<MaintenanceOrder>? MaintenanceOrderAdded;
    public event EventHandler<MaintenanceOrder>? MaintenanceOrderUpdated;
    public event EventHandler<MaintenanceOrder>? MaintenanceOrderAcknowledged;
    public event EventHandler<MaintenanceOrder>? MaintenanceOrderSent;
    public event EventHandler<MaintenanceOrder>? MaintenanceStarted;

    #region FacadeBase
    public override void Activated()
    {
        if (MaintenanceManager != null)
        {
            MaintenanceManager.OrderAcknowledged += MaintenanceManager_OrderAcknowledged;
            MaintenanceManager.OrderAdded += MaintenanceManager_OrderAdded;
            MaintenanceManager.OrderUpdated += MaintenanceManager_OrderUpdated;
            MaintenanceManager.OrdersSent += MaintenanceManager_OrdersSent;
            MaintenanceManager.MaintenanceOverdue += MaintenanceManager_MaintenanceOverdue;
            MaintenanceManager.MaintenanceStarted += MaintenanceManager_MaintenanceStarted;
        }
    }

    public override void Deactivated()
    {
        if (MaintenanceManager != null)
        {
            MaintenanceManager.OrderAcknowledged -= MaintenanceManager_OrderAcknowledged;
            MaintenanceManager.OrderAdded -= MaintenanceManager_OrderAdded;
            MaintenanceManager.OrderUpdated -= MaintenanceManager_OrderUpdated;
            MaintenanceManager.OrdersSent -= MaintenanceManager_OrdersSent;
            MaintenanceManager.MaintenanceOverdue -= MaintenanceManager_MaintenanceOverdue;
        }
    }

    private void MaintenanceManager_MaintenanceStarted(object? sender, MaintenanceOrder e)
        => MaintenanceStarted?.Invoke(sender, e);

    private void MaintenanceManager_MaintenanceOverdue(object? sender, MaintenanceOrder e)
        => MaintenanceOverdue?.Invoke(sender, e);

    private void MaintenanceManager_OrdersSent(object? sender, MaintenanceOrder e)
        => MaintenanceOrderSent?.Invoke(sender, e);

    private void MaintenanceManager_OrderUpdated(object? sender, MaintenanceOrder e)
        => MaintenanceOrderUpdated?.Invoke(sender, e);

    private void MaintenanceManager_OrderAdded(object? sender, MaintenanceOrder e)
        => MaintenanceOrderAdded?.Invoke(sender, e);

    private void MaintenanceManager_OrderAcknowledged(object? sender, MaintenanceOrder e)
        => MaintenanceOrderAcknowledged?.Invoke(sender, e);

    public Task AddMaintenanceOrderAsync(MaintenanceOrder order, CancellationToken cancellationToken)
    {
        ValidateHealthState();
        ArgumentNullException.ThrowIfNull(order);
        return MaintenanceManager.AddAsync(order, cancellationToken);
    }

    public Task AcknowledgeMaintenanceOrderAsync(long maintenanceOrderId, Acknowledgement data, CancellationToken cancellationToken)
    {
        ValidateHealthState();
        ArgumentNullException.ThrowIfNull(data);
        Logger?.Log(LogLevel.Information, "Maintenance {id} will be acknowledged/rescheduled caused by facade a call.", maintenanceOrderId);
        return MaintenanceManager.AcknowledgeAsync(maintenanceOrderId, data, cancellationToken);
    }

    public Task UpdateMaintenanceOrderAsync(MaintenanceOrder order, CancellationToken cancellationToken)
    {
        ValidateHealthState();
        ArgumentNullException.ThrowIfNull(order);
        return MaintenanceManager.UpdateAsync(order, cancellationToken);
    }

    public Task DeleteMaintenanceOrderAsync(MaintenanceOrder order, CancellationToken cancellationToken)
    {
        ValidateHealthState();
        ArgumentNullException.ThrowIfNull(order);
        return MaintenanceManager.DeleteAsync(order, cancellationToken);
    }

    public async Task StartMaintenanceAsync(MaintenanceOrder order, CancellationToken cancellationToken)
    {
        ValidateHealthState();
        ArgumentNullException.ThrowIfNull(order);
        await MaintenanceManager?.StartAsync(order.Id, cancellationToken);
        MachineBreak?.Invoke(this, order);
    }
    #endregion
}
