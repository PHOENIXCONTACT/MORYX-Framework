// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Concurrent;
using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moryx.Material.Endpoints.Model;
using Moryx.Material.Facade;
using Moryx.Material.Integrations.Orders;
using Moryx.Tools;

namespace Moryx.Material.Endpoints;

// ToDo: Add problemdetails to endpoint errors
// ToDo: Add server sent event stream fix
/// <summary>
/// Definition of a REST API on the <see cref="IMaterialManagement"/> facade.
/// </summary>
[ApiController]
[Route("api/moryx/materials/")]
[Produces("application/json")]
public class MaterialManagementController(IMaterialManagement materialManagement, IOrderIntegration? orderIntegration, ILogger<MaterialManagementController> logger) : ControllerBase
{
    private readonly IMaterialManagement _materialManagement = materialManagement ?? throw new ArgumentNullException(nameof(materialManagement));
    private readonly IOrderIntegration? _orderIntegration = orderIntegration;

    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    #region Material Containers
    #region GET

    [HttpGet("containers")]
    [ProducesResponseType(typeof(MaterialContainerModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = MaterialPermissions.CanRead)]
    public ActionResult<MaterialContainerModel[]> GetContainers()
    {
        var containers = _materialManagement.GetContainers();
        return Ok(containers.ToModels());
    }

    [HttpGet("containers/types")]
    [ProducesResponseType(typeof(MaterialContainerTypeModel[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = MaterialPermissions.CanRead)]
    public ActionResult<MaterialContainerTypeModel[]> GetTypes()
    {
        var types = _materialManagement.GetContainerTypes();
        return Ok(types.Select(t => t.ToModel()).ToArray());
    }

    #endregion

    #region POST
    [HttpPost("containers/pre-advice")]
    [ProducesResponseType(typeof(MaterialContainerModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = MaterialPermissions.CanUpdate)]
    public async Task<ActionResult<MaterialContainerModel>> PreAdviceAsync(PreAdviceModel preAdvice, CancellationToken cancellationToken)
    {
        if (preAdvice.ContainerId <= 0)
        {
            return BadRequest("Container Id must be a positive number");
        }

        try
        {
            var updatedContainer = await _materialManagement.PreAdviceMaterialAsync(preAdvice.ToBusiness(), cancellationToken);
            return Ok(updatedContainer.ToModel());
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Container id '{preAdvice.ContainerId}' could not be found.");
        }
    }
    #endregion

    #region Delete
    [HttpDelete("containers/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = MaterialPermissions.CanDelete)]
    public async Task<ActionResult> Deregister(long id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest("Container Id must be a positive number");
        }

        try
        {
            await _materialManagement.DeregisterContainerAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Container with id '{id}' could not be found.");
        }
    }
    #endregion

    #region Server Sent Events

    private static readonly ConcurrentDictionary<Guid, Channel<string>> _containerStreamSubscribers = new();

    [HttpGet("containers/stream")]
    [ProducesResponseType(typeof(MaterialContainerModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [Authorize(Policy = MaterialPermissions.CanRead)]
    public async Task ContainerChanges(CancellationToken cancellationToken)
    {
        // Define event handlers using the broadcast helper
        var updateEventHandler = new EventHandler<ContainerUpdatedEventArgs>((_, e) => Broadcast(e.Container));
        var stateChangeEventHandler = new EventHandler<ContainerStateChangedEventArgs>((_, e) => Broadcast(e.Container));

        try
        {
            var result = TypedResults.ServerSentEvents(Subscribe(cancellationToken));
            // Register event handlers after result creation but before execution to ensure finally cleanup
            _materialManagement.ContainerUpdated += updateEventHandler;
            _materialManagement.ContainerStateChanged += stateChangeEventHandler;

            await result.ExecuteAsync(HttpContext);
        }
        catch (OperationCanceledException)
        {
            // client disconnected — this is expected, not an error
        }
        finally
        {
            _materialManagement.ContainerUpdated -= updateEventHandler;
            _materialManagement.ContainerStateChanged -= stateChangeEventHandler;
        }

        return;

        async IAsyncEnumerable<SseItem<string>> Subscribe([EnumeratorCancellation] CancellationToken cancelToken)
        {
            var channel = Channel.CreateUnbounded<string>();
            var id = Guid.NewGuid();
            _containerStreamSubscribers[id] = channel;

            try
            {
                await foreach (var data in channel.Reader.ReadAllAsync(cancelToken))
                {
                    yield return new SseItem<string>(data) { ReconnectionInterval = TimeSpan.FromSeconds(10) };
                }
            }
            finally
            {
                _containerStreamSubscribers.TryRemove(id, out _);
            }
        }

        // Local helper to broadcast instruction changes to all matching subscribers
        void Broadcast(IMaterialContainer container) => _containerStreamSubscribers.Values.ForEach(channel =>
            channel.Writer.TryWrite(JsonSerializer.Serialize(container.ToModel(), _serializerOptions)));
    }
    #endregion
    #endregion

    #region Order References
    #region GET

    [HttpGet("integrations/orders/available")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [Authorize(Policy = MaterialPermissions.CanRead)]
    public ActionResult<bool> HasOrderIntegration() => Ok(_orderIntegration is not null);

    [HttpGet("integrations/orders")]
    [ProducesResponseType(typeof(OrderReferenceModel[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, Description = $"Send if {nameof(HasOrderIntegration)} returns false.")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = MaterialPermissions.CanRead)]
    public ActionResult<OrderReference[]> GetOrderReferences()
    {
        // ToDo: Fully fill problem detail responses
        if (_orderIntegration is null)
        {
            return Problem("Order Integration is unavailable in this application", statusCode: StatusCodes.Status404NotFound);
        }

        var references = _orderIntegration.GetOrderReferences();
        return Ok(references.ToModels());
    }
    #endregion
    #endregion
}
