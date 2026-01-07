// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Maintenance.Exceptions;
using Moryx.Maintenance.Management.Components;
using Moryx.Maintenance.Management.ModuleController;
using Moryx.Maintenance.Model;
using Moryx.Maintenance.Model.Mappers;
using Moryx.Maintenance.Model.Storage;
using Moryx.Model.Repositories;
using Moryx.Threading;

namespace Moryx.Maintenance.Management.Implementations;

[Component(LifeCycle.Singleton, typeof(IMaintenanceManager))]
internal sealed class MaintenanceManager : IMaintenanceManager
{
    private int _timerId;
    private const int OneHourInMs = 60 * 60 * 1000;
    private readonly object _locker = new();
    private readonly List<MaintenanceOrder> _maintenanceOrders = [];
    private readonly List<Acknowledgement> _acknowledgements = [];
    private List<IMaintainableResource> _maintainableResources = [];

    #region Dependencies
    public required IUnitOfWorkFactory<MaintenanceContext> UnitOfWorkFactory { get; set; }
    public required ModuleConfig ModuleConfig { get; set; }
    public required IResourceManagement ResourceManagement { get; set; }
    public IReadOnlyList<MaintenanceOrder> Orders
        => _maintenanceOrders.AsReadOnly();
    public IReadOnlyList<Acknowledgement> Acknowledgements
        => _acknowledgements.AsReadOnly();

    public required IParallelOperations ParallelOperations { get; set; }

    public required IModuleLogger Logger { get; set; }
    #endregion

    #region Facade
    public void Start()
    {
        if (ResourceManagement is not null)
        {
            _maintainableResources = ResourceManagement.GetResources<IMaintainableResource>().ToList() ?? [];
            ResourceManagement.ResourceAdded += ResourceManagement_ResourceAdded;
            ResourceManagement.ResourceRemoved += ResourceManagement_ResourceRemoved;
            SubscribeToMaintainableResources();
        }

        RestoreMaintenanceOrders();

        _timerId = ParallelOperations?.ScheduleExecution(Timer_Elapsed, 0, ModuleConfig?.RefreshPeriodMs ?? OneHourInMs) ?? -1;
    }
    public void Stop()
    {
        ParallelOperations?.StopExecution(_timerId);
        _maintenanceOrders.Clear();
        _acknowledgements.Clear();
        _maintainableResources.Clear();
        UnSubscribeFromMaintainableResources();
    }

    #endregion

    public bool HasOverdueMaintenance
        => _maintenanceOrders.Any(x => x.Interval?.Overdue > 0);

    public event EventHandler<MaintenanceOrder>? OrderAdded;
    public event EventHandler<MaintenanceOrder>? OrderUpdated;
    public event EventHandler<MaintenanceOrder>? OrderAcknowledged;
    public event EventHandler<MaintenanceOrder>? OrdersSent;
    public event EventHandler<MaintenanceOrder>? MaintenanceOverdue;
    public event EventHandler<MaintenanceOrder>? MaintenanceStarted;

    public async Task AcknowledgeAsync(long orderId, Acknowledgement data, CancellationToken cancellationToken)
    {
        var maintenance = _maintenanceOrders.FirstOrDefault(x => x.Id == orderId)
            ?? throw new MaintenanceNotFoundException(orderId);
        maintenance.Interval?.Reset();

        maintenance.Acknowledgements = [.. maintenance.Acknowledgements, data];
        using var uow = UnitOfWorkFactory?.Create();
        var acknowledgement = (await MaintenanceStorage.SaveAsync(uow!, maintenance, cancellationToken)).Acknowledgements.LastOrDefault();
        Logger?.LogInformation("MaintenanceOrder '{orderId}' acknowledged!", orderId);
        if (acknowledgement is not null)
        {
            lock (_locker)
            {
                _acknowledgements.Add(acknowledgement.ToModel());
            }
            maintenance.MaintenanceStarted = false;
            OrderAcknowledged?.Invoke(this, maintenance);
        }
    }

    private void RestoreMaintenanceOrders()
    {
        using var uow = UnitOfWorkFactory?.Create();
        var maintenanceRepo = uow?.GetRepository<IMaintenanceOrderRepository>();

        var restored = maintenanceRepo?.GetAll()
                        .Select(e => MaintenanceStorage.Load(e, _maintainableResources)) ?? [];
        _maintenanceOrders.AddRange(restored);

        var acknowledgementRepo = uow?.GetRepository<IAcknowledgementRepository>();
        var acknowledgedRestored = acknowledgementRepo?.GetAll().Select(e => e.ToModel()) ?? [];
        _acknowledgements.AddRange(acknowledgedRestored);

        RestoreActiveMaintenance(_maintenanceOrders.Where(x => x.MaintenanceStarted));
    }

    private static void RestoreActiveMaintenance(IEnumerable<MaintenanceOrder> maintenances)
    {
        foreach (var maintenance in maintenances)
        {
            maintenance.Resource?.StartMaintenance(new MaintenanceOrderStart
            {
                OrderId = maintenance.Id,
                Instructions = maintenance.Instructions
            }
            );
        }
    }

    private void ResourceManagement_ResourceRemoved(object? sender, IResource e)
    {
        if (e is not IMaintainableResource resource)
        {
            return;
        }
        _maintainableResources.Remove(resource);
        resource.CycleChanged -= Resource_CycleChanged;
        resource.MaintenanceCompleted -= Resource_MaintenanceCompleted;
    }

    private void ResourceManagement_ResourceAdded(object? sender, IResource e)
    {
        if (e is not IMaintainableResource resource)
        {
            return;
        }

        lock (_locker)
        {
            _maintainableResources.Add(resource);
        }
        resource.CycleChanged += Resource_CycleChanged;
        resource.MaintenanceCompleted += Resource_MaintenanceCompleted;
    }

    private void SubscribeToMaintainableResources()
    {
        foreach (var resource in _maintainableResources)
        {
            resource.CycleChanged += Resource_CycleChanged;
            resource.MaintenanceCompleted += Resource_MaintenanceCompleted;
            resource.MaintenanceStarted += Resource_MaintenanceStarted; ;
            Logger?.LogDebug("Maintenance Management found resource '{id}'", resource.Id);
        }
    }

    private async void Resource_MaintenanceStarted(object? sender, MaintenanceEventArgs e)
    {
        var found = _maintenanceOrders.FirstOrDefault(x => x.Id == e.MaintenanceOrderId);
        if (found != null)
        {
            found.MaintenanceStarted = true;

            using var uow = UnitOfWorkFactory?.Create()!;
            await MaintenanceStorage.SaveAsync(uow, found, CancellationToken.None);
            MaintenanceStarted?.Invoke(sender, found);
        }
    }

    private void UnSubscribeFromMaintainableResources()
    {
        foreach (var resource in _maintainableResources)
        {
            resource.CycleChanged -= Resource_CycleChanged;
            resource.MaintenanceCompleted -= Resource_MaintenanceCompleted;
            Logger?.LogDebug("Maintenance Management is unsubscribing resource '{id}'", resource.Id);
        }
    }

    private async void Resource_MaintenanceCompleted(object? sender, AcknowledgementEventArgs e)
    {
        var acknowledgement = new Acknowledgement
        {
            Description = e.Description,
            OperatorId = e.OperatorId,
            Created = DateTime.UtcNow
        };

        await AcknowledgeAsync(e.MaintenanceOrderId, acknowledgement, CancellationToken.None);
    }

    private async void Resource_CycleChanged(object? sender, int count)
    {
        if (sender is not IMaintainableResource resource)
        {
            return;
        }

        var maintenances = _maintenanceOrders.Where(x => x.Resource!.Id == resource.Id && x.IsActive).ToList();
        foreach (var maintenance in maintenances)
        {
            await TriggerUpdate(maintenance, count);
        }
    }

    private async void Timer_Elapsed()
    {
        foreach (var maintenance in _maintenanceOrders.Where(x => x.IsActive))
        {
            switch (maintenance.Interval)
            {
                case Days:
                case Hours:
                    await TriggerUpdate(maintenance);
                    break;
                default:
                    break;
            }

        }
    }

    private async Task TriggerUpdate(MaintenanceOrder maintenance, int elapsed = 1)
    {
        if (maintenance.MaintenanceStarted)
        {
            return;
        }

        if (maintenance.Interval is not null && UnitOfWorkFactory is not null)
        {
            maintenance.Interval.Update(elapsed);
            using var uow = UnitOfWorkFactory.Create();
            await MaintenanceStorage.SaveAsync(uow!, maintenance, CancellationToken.None);

            if (maintenance.Interval.IsOverdue())
            {
                TriggerOverdue(maintenance);
            }
            else if (maintenance.Interval.IsDue())
            {
                TriggerDue(maintenance);
            }
        }
    }

    private void TriggerDue(MaintenanceOrder maintenance)
    {
        Logger?.LogInformation("Maintenance Management triggered a maintenance for resource '{id}'", maintenance.Resource?.Id ?? -1);
        maintenance.Resource?.StartMaintenance(new MaintenanceOrderStart
        {
            OrderId = maintenance.Id,
            Instructions = maintenance.Instructions
        });
        OrdersSent?.Invoke(this, maintenance);
    }

    private void TriggerOverdue(MaintenanceOrder maintenance)
    {
        Logger?.LogInformation("Maintenance Management triggered an Overdue maintenance for resource '{id}'", maintenance.Resource?.Id ?? -1);
        maintenance.Resource?.StartMaintenance(new MaintenanceOrderStart
        {
            OrderId = maintenance.Id,
            Instructions = maintenance.Instructions
        });
        OrdersSent?.Invoke(this, maintenance);
        MaintenanceOverdue?.Invoke(this, maintenance);
    }

    public async Task UpdateAsync(MaintenanceOrder model, CancellationToken cancellationToken)
    {
        using var uow = UnitOfWorkFactory?.Create();
        await MaintenanceStorage.SaveAsync(uow!, model, cancellationToken);
        OrderUpdated?.Invoke(this, model);
        Logger?.LogInformation("MaintenanceOrder '{id}' updated", model.Id);
    }

    public async Task AddAsync(MaintenanceOrder model, CancellationToken cancellationToken)
    {
        var match = _maintenanceOrders.FirstOrDefault(x => x.Resource?.Id == model.Resource!.Id);

        if (match is not null)
        {
            throw new DuplicatedMaintenanceOrderException(model.Resource?.Name ?? "Undefined");
        }

        using var uow = UnitOfWorkFactory?.Create();
        var result = await MaintenanceStorage.SaveAsync(uow!, model, cancellationToken);
        Logger?.LogInformation("MaintenanceOrder '{id}' created!", result.Id);
        lock (_locker)
        {
            var order = MaintenanceStorage.Load(result, _maintainableResources);
            _maintenanceOrders.Add(order);
        }
        OrderAdded?.Invoke(this, model);
    }

    public async Task DeleteAsync(MaintenanceOrder model, CancellationToken cancellationToken)
    {
        using var uow = UnitOfWorkFactory?.Create();
        await MaintenanceStorage.DeleteAsync(uow!, model.Id, cancellationToken);
        Logger?.LogInformation("MaintenanceOrder '{id}' deleted!", model.Id);
    }

    public Task StartAsync(long maintenanceOrderId, CancellationToken cancellationToken)
    {
        var match = _maintenanceOrders.FirstOrDefault(x => x.Id == maintenanceOrderId);
        match?.Resource?.StartMaintenance(new MaintenanceOrderStart
        {
            OrderId = match.Id,
            Instructions = match.Instructions
        });
        return Task.CompletedTask;
    }
}
