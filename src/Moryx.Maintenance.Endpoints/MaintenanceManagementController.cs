// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moryx.AbstractionLayer.Resources;
using Moryx.Maintenance.Endpoints.Models;
using Moryx.Maintenance.Endpoints.Services;
using Moryx.Maintenance.Exceptions;
using Moryx.Maintenance.Management.Models;
using Moryx.Runtime.Modules;
using Moryx.Serialization;

namespace Moryx.Maintenance.Endpoints;

/// <summary>
/// Definition of a REST API on the <see cref="IMaintenanceManagement"/> facade.
/// </summary>
[ApiController]
[Route("api/moryx/maintenances/")]
[Produces("application/json")]
public class MaintenanceManagementController(
    IMaintenanceManagement maintenanceManagement,
    IServiceProvider serviceProvider,
    IModuleManager moduleManager,
    IResourceManagement resourceManagement
    ) : ControllerBase
{
    private readonly MaintenanceService _maintenanceService = new(maintenanceManagement, serviceProvider, moduleManager, resourceManagement);


    /// <summary>
    ///  Returns the list of maintenance orders
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<ActionResult<GetAllMaintenanceOrderResponse>> GetAll()
    {
        return Response(() => Task.FromResult(new GetAllMaintenanceOrderResponse([.. _maintenanceService.GetAll()])));
    }

    /// <summary>
    /// Given an <paramref name="id"/> returns the corresponding <see cref="Entry"/>
    /// </summary>
    /// <param name="id">Id of the maintenance order</param>
    /// <returns></returns>
    [HttpGet("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<ActionResult<Entry>> Get(long id)
    {
        return Response(() =>
        {
            return Task.FromResult(_maintenanceService.Get(id));
        });
    }

    /// <summary>
    /// Returns a prototype of a maintenance order entry
    /// </summary>
    /// <returns></returns>
    [HttpGet("prototype")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<ActionResult<Entry>> Prototype()
    {
        return Response(() => Task.FromResult(_maintenanceService.Prototype()));
    }

    /// <summary>
    /// Adds a new maintenance order from the given <paramref name="entry"/>
    /// </summary>
    /// <param name="entry">The maintenance order entry to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    [HttpPost("new")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<ActionResult> Add(Entry entry, CancellationToken cancellationToken)
    {
        return Response(() =>
        {
            return _maintenanceService.AddAsync(entry, cancellationToken);
        });
    }

    /// <summary>
    /// Updates an existing maintenance order from the given <paramref name="entry"/>
    /// </summary>
    /// <param name="id">Id of the maintenance order to update</param>
    /// <param name="entry">The maintenance order entry to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<ActionResult> Update(long id, Entry entry, CancellationToken cancellationToken)
    {
        return Response(() =>
        {
            return _maintenanceService.UpdateAsync(id, entry, cancellationToken);
        });
    }

    /// <summary>
    /// Acknowledges the maintenance order with the given <paramref name="id"/>
    /// </summary>
    /// <param name="id">Id of the maintenance order to acknowledge</param>
    /// <param name="data">Acknowledgement data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    [HttpPut("{id:long}/acknowledge")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<ActionResult> Acknowledge(long id, Acknowledgement data, CancellationToken cancellationToken)
    {
        return Response(() =>
        {
            return _maintenanceService.AcknowledgeAsync(id, data, cancellationToken);
        });
    }

    /// <summary>
    /// Starts the maintenance for the given <paramref name="id"/>
    /// </summary>
    /// <param name="id">Id of the maintenance order to start</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    [HttpPut("{id:long}/start")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<ActionResult> Start(long id, CancellationToken cancellationToken)
    {
        return Response(() =>
        {
            return _maintenanceService.StartAsync(id, cancellationToken);
        });
    }

    /// <summary>
    /// Deletes the maintenance order with the given <paramref name="id"/>
    /// </summary>
    /// <param name="id">Id of the maintenance order to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<ActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        return Response(async () =>
        {
            await _maintenanceService.DeleteAsync(id, cancellationToken);
        });
    }

    /// <summary>
    /// Streams the updates and changes of all maintenance orders
    /// </summary>
    /// <param name="cancelToken">Cancellation token</param>
    [HttpGet("stream")]
    [ProducesResponseType(typeof(MaintenanceOrderModel), StatusCodes.Status200OK)]
    public async Task Stream(CancellationToken cancelToken)
    {
        await _maintenanceService.Stream(cancelToken, HttpContext);
    }

    private new async Task<ActionResult<TResult>> Response<TResult>(Func<Task<TResult>> func, Func<TResult, ActionResult<TResult>>? onSuccess = null)
    {
        try
        {
            var result = await func();
            return onSuccess != null
                ? onSuccess(result)
                : (ActionResult<TResult>)Ok(result);
        }
        catch (Exception ex)
        {
            return MapToResponse(ex);
        }
    }

    private new async Task<ActionResult> Response(Func<Task> action)
    {
        try
        {
            await action();
            return Ok();
        }
        catch (Exception ex)
        {
            return MapToResponse(ex);
        }
    }

    private ObjectResult MapToResponse(Exception ex) => ex switch
    {
        NotFoundException _ => NotFound(ex.Message),
        ArgumentNullException _ => BadRequest(ex.Message),
        ArgumentException _ => BadRequest(ex.Message),
        KeyNotFoundException _ => StatusCode(500, ex.Message),
        HealthStateException _ => StatusCode(500, ex.Message),
        _ => StatusCode(500, ex.Message),
    };
}
