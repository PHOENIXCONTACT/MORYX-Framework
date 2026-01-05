// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moryx.AbstractionLayer.Resources;
using Moryx.Maintenance.Endpoints.Dtos;
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

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<GetAllMaintenanceOrderResponse> GetAll()
    {
        return Response(() => new GetAllMaintenanceOrderResponse([.._maintenanceService.GetAll()]));
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Entry> Get(long id)
    {
        return Response(() =>
        {
            return _maintenanceService.Get(id);
        });
    }

    [HttpGet("prototype")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Entry> Prototype()
    {
        return Response(_maintenanceService.Prototype);
    }

    [HttpPost("new")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult Add(Entry entry)
    {
        return Response(() =>
        {
            _maintenanceService.Add(entry);
        });
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult Update(long id, Entry entry)
    {
        return Response(() =>
        {
            _maintenanceService.Update(id, entry);
        });
    }

    [HttpPut("{id:long}/acknowledge")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult Acknowledge(long id, Acknowledgement data)
    {
        return Response(() =>
        {
            _maintenanceService.Acknowledge(id, data);
        });
    }

    [HttpPut("{id:long}/start")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult Start(long id)
    {
        return Response(() =>
        {
            _maintenanceService.Start(id);
        });
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult Delete(long id)
    {
        return Response(() =>
        {
            _maintenanceService.Delete(id);
        });
    }

    [HttpGet("stream")]
    [ProducesResponseType(typeof(MaintenanceOrderModel), StatusCodes.Status200OK)]
    public async Task Stream(CancellationToken cancelToken)
    {
        await _maintenanceService.Stream(cancelToken, HttpContext);
    }

    private new ActionResult<TResult> Response<TResult>(Func<TResult> func, Func<TResult, ActionResult<TResult>>? onSuccess = null)
    {
        try
        {
            var result = func();
            return onSuccess != null
                ? onSuccess(result)
                : (ActionResult<TResult>)Ok(result);
        }
        catch (Exception ex)
        {
            return MapToResponse(ex);
        }
    }

    private new ActionResult Response(Action action)
    {
        try
        {
            action();
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
