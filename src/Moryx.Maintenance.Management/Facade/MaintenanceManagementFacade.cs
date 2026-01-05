// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Microsoft.Extensions.Logging;
using Moryx.ControlSystem.Supervision.MachineBreak;
using Moryx.Logging;
using Moryx.Maintenance.Management.Components;
using Moryx.Runtime.Modules;

namespace Moryx.Maintenance.Management.Facade;

internal sealed class  MaintenanceManagementFacade : FacadeBase, IMaintenanceManagement, IMachineBreakSource
{
    #region Dependencies
    public IMaintenanceManager? MaintenanceManager { get; set; }
    #endregion

    public IEnumerable<MaintenanceOrder> Orders
        => MaintenanceManager?.Orders ?? [];
    public IEnumerable<Acknowledgement> Acknowledgements
        => MaintenanceManager?.Acknowledgements ?? [];
    public IModuleLogger? Logger { get; set; }

    public MachineBreakSourceStatus Status
        => new()
        {
            ShouldBreak = HasOverdueMaintenance
        };

    public bool HasOverdueMaintenance =>
        MaintenanceManager?.HasOverdueMaintenance ?? false;

    public event EventHandler<MaintenanceOrder>? MaintenanceOverdue;
    public event EventHandler? MachineBreak;
    public event EventHandler? MaintenanceOrderAdded;
    public event EventHandler? MaintenanceOrderUpdated;
    public event EventHandler<MaintenanceOrder>? MaintenanceOrderAcknowledged;
    public event EventHandler? MaintenanceOrderSent;
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

    private void MaintenanceManager_OrdersSent(object? sender, EventArgs e)
        => MaintenanceOrderSent?.Invoke(sender, e);

    private void MaintenanceManager_OrderUpdated(object? sender, EventArgs e)
        => MaintenanceOrderUpdated?.Invoke(sender, e);

    private void MaintenanceManager_OrderAdded(object? sender, EventArgs e)
        => MaintenanceOrderAdded?.Invoke(sender, e);

    private void MaintenanceManager_OrderAcknowledged(object? sender, MaintenanceOrder e)
        => MaintenanceOrderAcknowledged?.Invoke(sender, e);

    public void AddMaintenanceOrder(MaintenanceOrder order)
    {
        ValidateHealthState();
        ArgumentNullException.ThrowIfNull(order);
        MaintenanceManager?.Add(order);
    }

    public void AcknowledgeMaintenanceOrder(long maintenanceOrderId, Acknowledgement data)
    {
        ValidateHealthState();
        ArgumentNullException.ThrowIfNull(data);
        Logger?.Log(LogLevel.Information, "Maintenance {id} will be acknowledged/rescheduled caused by facade a call.", maintenanceOrderId);
        MaintenanceManager?.Acknowledge(maintenanceOrderId, data);
    }

    public void UpdateMaintenanceOrder(MaintenanceOrder order)
    {
        ValidateHealthState();
        ArgumentNullException.ThrowIfNull(order);
        MaintenanceManager?.Update(order);
    }

    public void DeleteMaintenanceOrder(MaintenanceOrder order)
    {
        ValidateHealthState();
        ArgumentNullException.ThrowIfNull(order);
        MaintenanceManager?.Delete(order);
    }

    public void StartMaintenance(MaintenanceOrder order)
    {
        ValidateHealthState();
        ArgumentNullException.ThrowIfNull(order);
        MaintenanceManager?.Start(order.Id);
        MachineBreak?.Invoke(this, EventArgs.Empty);
    }
    #endregion
}
