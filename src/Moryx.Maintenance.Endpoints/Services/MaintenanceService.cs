// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Microsoft.AspNetCore.Http;
using Moryx.AbstractionLayer.Resources;
using Moryx.Maintenance.Endpoints.Dtos;
using Moryx.Maintenance.Endpoints.Extensions;
using Moryx.Maintenance.Endpoints.StreamServices;
using Moryx.Maintenance.Exceptions;
using Moryx.Maintenance.Management.Mappers;
using Moryx.Maintenance.Management.Models;
using Moryx.Runtime.Modules;
using Moryx.Serialization;

namespace Moryx.Maintenance.Endpoints.Services;

/// <summary>
/// Handles Maintenance client and backend interactions
/// </summary>
internal class MaintenanceService
{
    private readonly IMaintenanceManagement _maintenanceManagement;
    private readonly ICustomSerialization _serialization;
    private readonly IResourceManagement _resourceManagement;
    public MaintenanceService(
       IMaintenanceManagement maintenanceManagement,
       IServiceProvider serviceProvider,
       IModuleManager moduleManager,
       IResourceManagement resourceManagement
   )
    {
        _maintenanceManagement = maintenanceManagement ?? throw new ArgumentNullException(nameof(maintenanceManagement));
        _resourceManagement = resourceManagement ?? throw new ArgumentNullException(nameof(resourceManagement));
        // this is needed to get the ServerModule of the IMaintenanceManagement.
        // IMaintenanceManagement doesn't provide the ServerModule by itself.
        var module = moduleManager.AllModules.FirstOrDefault(module => module is IFacadeContainer<IMaintenanceManagement>);
        if (module != null)
        {
            _serialization = new MaintenanceOrderSerialization(module.Container, serviceProvider);
        }
    }

    /// <summary>
    /// Returns the list of maintenance orders
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MaintenanceOrderModel> GetAll()
    {
        var orders = _maintenanceManagement.Orders.Select(x => x.ToDto());
        return [.. orders.OrderBy(x => x.Resource.Name)];
    }

    /// <summary>
    /// Given an <paramref name="id"/> returns the corresponding <see cref="Entry"/>
    /// </summary>
    /// <param name="id">OrderId of the maintenance order</param>
    /// <returns></returns>
    /// <exception cref="MaintenanceNotFoundException">Thrown when no maintenance order found</exception>
    public Entry Get(long id)
    {
        var dto = _maintenanceManagement.Orders
                  .FirstOrDefault(x => x.Id == id)
                  ?.ToDto();
        var form = dto?.ToOrderEntry();
        var entry = form == null
        ? throw new MaintenanceNotFoundException(id)
        : form.ToEntry(_serialization);
        return entry;
    }

    /// <summary>
    /// Return a prototype <see cref="Entry"/> for the maintenance order
    /// </summary>
    /// <returns></returns>
    public Entry Prototype()
    {
        return EntryConvert.EncodeClass(typeof(MaintenanceOrderResponse), _serialization);
    }

    /// <summary>
    /// Adds a new maintenance order from the given <paramref name="entry"/>
    /// </summary>
    /// <param name="entry"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void Add(Entry entry)
    {
        var form = EntryConvert.CreateInstance<MaintenanceOrderResponse>(entry, _serialization) ?? throw new ArgumentNullException();
        var resource = _resourceManagement.GetResource<IMaintainableResource>(form.Resource);
        var model = new MaintenanceOrder
        {
            Resource = resource,
            Block = form.Block,
            Description = form.Description,
            Instructions = form.Instructions,
            Interval = form.Interval,
            IsActive = form.IsActive,
        };
        _maintenanceManagement.AddMaintenanceOrder(model);
    }

    /// <summary>
    /// Update the maintenance order for the given <paramref name="id"/>
    /// </summary>
    /// <param name="id">OrderId of the maintenance order</param>
    /// <param name="entry">Entry that contains the data of the updated maintenance order</param>
    /// <exception cref="MaintenanceNotFoundException"></exception>
    public void Update(long id, Entry entry)
    {
        var order = _maintenanceManagement.Orders.FirstOrDefault(x => x.Id == id) ?? throw new MaintenanceNotFoundException(id);
        var oldForm = order.ToDto().ToOrderEntry();
        EntryConvert.UpdateInstance(oldForm, entry, _serialization);

        order.Interval = oldForm.Interval;
        order.IsActive = oldForm.IsActive;
        order.Description = oldForm.Description;
        order.Instructions = oldForm.Instructions;
        order.Block = oldForm.Block;

        if (order.Resource?.Name != oldForm.Resource)
        {
            order.Resource = _resourceManagement.GetResource<IMaintainableResource>(oldForm.Resource);
        }
        _maintenanceManagement.UpdateMaintenanceOrder(order);
    }

    /// <summary>
    /// Acknowledges the maintenance for the given <paramref name="id"/>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="data"></param>
    /// <exception cref="MaintenanceNotFoundException"></exception>
    public void Acknowledge(long id, Acknowledgement data)
    {
        var order = _maintenanceManagement
        .Orders
        .First(x => x.Id == id) ?? throw new MaintenanceNotFoundException(id);
        _maintenanceManagement.AcknowledgeMaintenanceOrder(id, data);
    }

    /// <summary>
    /// Given a maintenance order <paramref name="id"/>, start/sends the order to the corresponding cell/resource
    /// </summary>
    /// <param name="id">OrderId of the maintenance order</param>
    /// <exception cref="MaintenanceNotFoundException"></exception>
    public void Start(long id)
    {
        var order = _maintenanceManagement
        .Orders
        .First(x => x.Id == id) ?? throw new MaintenanceNotFoundException(id);
        _maintenanceManagement.StartMaintenance(order);
    }

    /// <summary>
    /// Given a maintenance order <paramref name="id"/>, marks the corresponding entity as 'DELETED'
    /// </summary>
    /// <param name="id">OrderId of the maintenance order</param>
    /// <exception cref="MaintenanceNotFoundException"></exception>
    public void Delete(long id)
    {
        var order = _maintenanceManagement.Orders.FirstOrDefault(x => x.Id == id) ?? throw new MaintenanceNotFoundException(id);
        _maintenanceManagement.DeleteMaintenanceOrder(order);
    }

    /// <summary>
    /// Streams the updates and changes of all maintenance orders
    /// </summary>
    /// <param name="cancelToken">Cancellation token</param>
    /// <param name="context">the Http context of the stream</param>
    /// <returns></returns>
    public async Task Stream(CancellationToken cancelToken, HttpContext context)
    {
        var stream = new MaintenanceStream(_maintenanceManagement);
        await stream.Start(context, cancelToken);
    }
}
