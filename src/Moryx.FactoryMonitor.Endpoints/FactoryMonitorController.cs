// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Concurrent;
using System.Globalization;
using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using System.Timers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Resources;
using Moryx.AspNetCore;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.Processes;
using Moryx.Factory;
using Moryx.FactoryMonitor.Endpoints.Converter;
using Moryx.FactoryMonitor.Endpoints.Extensions;
using Moryx.FactoryMonitor.Endpoints.Models;
using Moryx.FactoryMonitor.Endpoints.Properties;
using Moryx.Orders;
using Timer = System.Timers.Timer;

namespace Moryx.FactoryMonitor.Endpoints;

/// <summary>
/// Endpoint for the Factory Monitor
/// </summary>
/// to-dos: add extra cellState to display what the cell is doing before starting an activity or after an activity completed
[ApiController]
[Route("api/moryx/factory-monitor/")]
public class FactoryMonitorController : ControllerBase
{
    private readonly string[] _colorPalette = ["#97bf0d", "#0098a0", "#ffa906", "#03ad3b", "#d60f4e", "#4A4033", "#6EC1C5", "#93E0B0", "#BC9989", "#EE60EA", "#D7F7C3", "#CE9250", "#AF7E81", "#61666C", "#04629A", "#E39332", "#90A39E", "#98199E", "#DB97C9"];

    private readonly IResourceManagement _resourceManager;
    private readonly IProcessControl _processControl;
    private readonly IOrderManagement _orderManager;
    private readonly CellSerialization _serialization;

    private static readonly ConcurrentDictionary<Guid, Channel<(string EventType, string Data)>> _factoryStreamSubscribers = new();
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    private Dictionary<IMachineLocation, ICell> _locationToCellMappings;
    private Timer _resourceChangedTimer;
    private readonly ILogger<FactoryMonitorController> _logger;

    public FactoryMonitorController(IResourceManagement resourceManager, IProcessControl processControl, IOrderManagement orderManagement, ILogger<FactoryMonitorController> logger = null)
    {
        _resourceManager = resourceManager;
        _processControl = processControl;
        _orderManager = orderManagement;
        _serialization = new CellSerialization();
        _logger = logger;
    }

    /// <summary>
    /// Return the initial data/state of the factory
    /// </summary>
    /// <returns><see cref="FactoryStateModel"/></returns>
    [HttpGet("state")]
    public ActionResult<FactoryStateModel> InitialFactoryState()
    {
        var activityChangedModels = new List<ActivityChangedModel>();
        var cellStateChangedModels = new List<CellStateChangedModel>();
        var resourceChangedModels = new List<ResourceChangedModel>();

        var locations = _resourceManager.GetResources<IMachineLocation>();
        var locationsToCellMappings = MapCellsTo(locations);
        var orders = TryGetOrders();
        var activities = TryGetActivities();
        var converter = new Converter.Converter(_serialization, _logger);

        foreach (var l2cMapping in locationsToCellMappings)
        {
            var location = l2cMapping.Key;
            var cell = l2cMapping.Value;
            var activity = activities.FirstOrDefault(a => a.Tracing.ResourceId == cell.Id && a.Tracing.Started is not null && a.Tracing.Completed is null);
            if (activity is not null)
            {
                //to-do: handle multiple activities in one cell
                activityChangedModels.Add(cell.GetActivityChangedModel(activity, orders));
                cellStateChangedModels.Add(cell.GetCellStateChangedModel(ActivityProgress.Running));
            }
            else
            {
                cellStateChangedModels.Add(cell.GetCellStateChangedModel());
            }
            resourceChangedModels.Add(cell.GetResourceChangedModel(converter, _resourceManager, location));
        }

        activityChangedModels = [.. activityChangedModels.OrderBy(x => x.ResourceId)];
        cellStateChangedModels = [.. cellStateChangedModels.OrderBy(x => x.Id)];
        resourceChangedModels = [.. resourceChangedModels.OrderBy(x => x.Id)];

        var factory = _resourceManager.GetRootFactory();

        var model = new FactoryStateModel() { Id = factory.Id };
        model.ActivityChangedModels = activityChangedModels;
        model.CellStateChangedModels = cellStateChangedModels;
        model.ResourceChangedModels = resourceChangedModels;
        model.OrderModels = orders;

        return model;
    }

    private IEnumerable<AbstractionLayer.Activities.Activity> TryGetActivities()
    {
        try
        {
            return _processControl.GetRunningProcesses()
                .Select(p => p.CurrentActivity())
                .Where(a => a is not null && a.Tracing is not null);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not retrieve activity information in {name}", nameof(FactoryMonitorController));
            // We want to fail gracefully, if e.g. the facade is not yet available
            return [];
        }
    }

    private List<OrderModel> TryGetOrders()
    {
        try
        {
            return _orderManager.GetOrderModels(_colorPalette);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not retrieve order information in {name}", nameof(FactoryMonitorController));
            // We want to fail gracefully, if e.g. the facade is not yet available
            return [];
        }
    }

    // ToDo: This is sub-optimal as IMachineLocation does not require a cell to be referenced (and shouldn't) but we
    // assume each location to have a cell in this endpoint and the UI. Could be changed for MORYX 12;
    // Also this constitutes duplicate scan-through-the-resource-graph-logic as in the SimpleGraph implementation, this should be unified
    private Dictionary<IMachineLocation, ICell> MapCellsTo(IEnumerable<IMachineLocation> locations) =>
        locations.ToDictionary(l => l, GetReferencedCellOrChildCell).Where(kv => kv.Value is not null).ToDictionary();

    // Currently returns a list of all visualizable items and is not used, so remove or at least rename in MORYX 12
    /// <summary>
    /// List of all the cells
    /// </summary>
    /// <returns></returns>
    [HttpGet("cells")]
    public ActionResult<List<VisualizableItemModel>> AllCells()
    {
        var locations = _resourceManager.GetResources<IMachineLocation>();
        var locationToCellMappings = MapCellsTo(locations);

        var converter = new Converter.Converter(_serialization, _logger);
        return locationToCellMappings.Select(l2cMapping => new SimpleGraph { Id = l2cMapping.Key.Id }
            .ToVisualItemModel(_resourceManager, _logger, converter)).ToList();
    }

    // ToDo: Endpoints usually return arrays and pagination would be good.
    /// <summary>
    /// Get the list of displayable item for this current factory view
    /// </summary>
    [HttpGet("factory-content/{factoryId}")]
    public ActionResult<List<VisualizableItemModel>> FactoryContent(long factoryId)
    {
        // check if there is a factory with the given id
        var factory = _resourceManager.GetResource<IManufacturingFactory>(x => x.Id == factoryId);
        if (factory is null)
        {
            return NotFound(Strings.FactoryMonitorController_FactoryNotFound_);
        }

        var converter = new Converter.Converter(_serialization, _logger);
        var graph = _resourceManager.ReadUnsafe(factory.Id, SimpleGraph.Create);

        var output = graph.Children
            .Select(e => e.ToVisualItemModel(_resourceManager, _logger, converter))
            .Where(x => x is not null).ToList();

        return output;
    }

    /// <summary>
    /// Returns the navigation item use by the edit menu in the UI. (Go back button)
    /// </summary>
    /// <param name="factoryId"></param>
    /// <returns></returns>
    [HttpGet("navigation/{factoryId}")]
    public ActionResult<FactoryModel> GetNavigation(long factoryId)
    {
        var root = _resourceManager.GetRootFactory();
        if (factoryId == root.Id) return new FactoryModel
        {
            Id = factoryId,
            Title = root.Name,
            BackgroundURL =
                root.BackgroundUrl
        };

        var factory = _resourceManager.ReadUnsafe(factoryId, e => (ManufacturingFactory)e);
        if (factory is not ManufacturingFactory manufacturingFactory)
        {
            BadRequest(Strings.FactoryMonitorController_FactoryNotFound_);
        }

        var parentFactory = factory.GetFactory(); //Get Parent factory

        return new FactoryModel
        {
            Id = factoryId,
            BackgroundURL = factory.BackgroundUrl,
            Title = factory.Name,
            ParentId = parentFactory?.Id ?? -1
        };
    }

    /// <summary>
    /// Stream of state of the factory (individual cell states and processes/orders states).
    /// </summary>
    [HttpGet("state-stream")]
    [ProducesResponseType(typeof(ActivityChangedModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResourceChangedModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CellStateChangedModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OrderChangedModel), StatusCodes.Status200OK)]
    public async Task FactoryStatesStream(CancellationToken cancellationToken)
    {
        // Early validation - check if there are cells to monitor
        var locations = _resourceManager.GetResources<IMachineLocation>();
        // ToDo: Added and removed resources not reflected
        _locationToCellMappings = MapCellsTo(locations);

        if (_locationToCellMappings.Count == 0)
        {
            Response.Headers.Append("Retry-After", "5");
            return;
        }
        var converter = new Converter.Converter(_serialization, _logger);

        // Define event handlers using helper methods
        var resourceEventHandler = new ElapsedEventHandler((_, _) =>
            FactoryMonitorHelper.ResourceUpdated(_resourceManager, l => MapCellsTo(l), converter, Broadcast));

        var capabilitiesEventHandler = new EventHandler<ICapabilities>((sender, _) =>
            FactoryMonitorHelper.PublishCellUpdate((sender as ICell)?.GetCellStateChangedModel(), Broadcast));

        var orderStartedEventHandler = new EventHandler<OperationStartedEventArgs>((_, eventArgs) =>
            FactoryMonitorHelper.OrderStarted(eventArgs, TryGetOrders(), Broadcast));

        var orderEventHandler = new EventHandler<OperationChangedEventArgs>((sender, eventArgs) =>
            FactoryMonitorHelper.OrderUpdated(eventArgs, Broadcast));

        // ToDo: This breaks if new cells were added and process activities
        var activityEventHandler = new EventHandler<ActivityUpdatedEventArgs>((sender, eventArgs) =>
            FactoryMonitorHelper.ActivityUpdated(eventArgs, [.. _locationToCellMappings.Values],
                TryGetOrders(), Broadcast));

        // Setup timer
        _resourceChangedTimer = new();
        _resourceChangedTimer.Interval = 5000;
        _resourceChangedTimer.AutoReset = true;
        _resourceChangedTimer.Enabled = true;

        try
        {
            var result = TypedResults.ServerSentEvents(Subscribe(cancellationToken));

            // Register event handlers after result creation but before execution to ensure finally cleanup
            foreach (var l2cMapping in _locationToCellMappings)
            {
                l2cMapping.Value.CapabilitiesChanged += capabilitiesEventHandler;
            }

            _orderManager.OperationStarted += orderStartedEventHandler;
            _orderManager.OperationUpdated += orderEventHandler;
            _processControl.ActivityUpdated += activityEventHandler;
            _resourceChangedTimer.Elapsed += resourceEventHandler;

            await result.ExecuteAsync(HttpContext);
        }
        catch (OperationCanceledException)
        {
            // client disconnected — this is expected, not an error
        }
        finally
        {
            // Unregister handlers
            foreach (var l2cMapping in _locationToCellMappings)
            {
                l2cMapping.Value.CapabilitiesChanged -= capabilitiesEventHandler;
            }

            _orderManager.OperationStarted -= orderStartedEventHandler;
            _orderManager.OperationUpdated -= orderEventHandler;
            _processControl.ActivityUpdated -= activityEventHandler;
            _resourceChangedTimer.Elapsed -= resourceEventHandler;
            _resourceChangedTimer?.Dispose();
        }

        return;

        async IAsyncEnumerable<SseItem<string>> Subscribe([EnumeratorCancellation] CancellationToken cancelToken)
        {
            var channel = Channel.CreateUnbounded<(string EventType, string Data)>();
            var id = Guid.NewGuid();
            _factoryStreamSubscribers[id] = channel;

            try
            {
                await foreach (var (eventType, data) in channel.Reader.ReadAllAsync(cancelToken))
                {
                    yield return new SseItem<string>(data, eventType);
                }
            }
            finally
            {
                _factoryStreamSubscribers.TryRemove(id, out _);
            }
        }

        // Local helper to broadcast events to all connected clients
        static void Broadcast(string eventType, object data)
        {
            foreach (var channel in _factoryStreamSubscribers.Values)
            {
                channel.Writer.TryWrite((eventType, JsonSerializer.Serialize(data, _serializerOptions)));
            }
        }
    }

    // ToDo: Rename to move location in MORYX 12 and
    /// <summary>
    /// Return the location of the cell in the factory
    /// </summary>
    /// <returns><see cref="CellLocationModel"/></returns>
    [HttpPost("move-cell")]
    public async Task<ActionResult<CellLocationModel>> MoveCell(CellLocationModel location)
    {
        var cellLocation = _resourceManager.GetResource<IMachineLocation>(l => l.Id == location.Id);

        await _resourceManager.ModifyUnsafeAsync(cellLocation, r =>
        {
            var machineLocation = (IMachineLocation)r;
            machineLocation.Position = new Position { PositionX = location.PositionX, PositionY = location.PositionY };
            return Task.FromResult(true);
        });
        return Ok(location);
    }

    /// <summary>
    /// Changes the background of the factory
    /// </summary>
    [HttpPost("background")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> ChangeBackground(long resourceId, string url)
    {
        if (string.IsNullOrEmpty(url))
            return UnprocessableEntity($"The provided {nameof(url)} is invalid");

        var manufacturingConfig = _resourceManager.GetResources<IManufacturingFactory>().SingleOrDefault(f => f.Id == resourceId);
        if (manufacturingConfig is null)
            return NotFound("The resource to be modified could not be found");
        await _resourceManager.ModifyUnsafeAsync(manufacturingConfig.Id, r =>
        {
            ((IManufacturingFactory)r).BackgroundUrl = url;
            return Task.FromResult(true);
        });
        manufacturingConfig.BackgroundUrl = url;
        return Ok();
    }

    /// <summary>
    /// Return the properties settings of a cell as a Dictionary, with the property as the key.
    /// </summary>
    /// <param name="identifier">Identifier of the Cell</param>
    [HttpGet("cell-properties/{identifier}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Dictionary<string, CellPropertySettings>> GetCellPropertiesSettings(string identifier)
    {
        var cellLocation = _resourceManager.GetResources<IMachineLocation>()?
            .FirstOrDefault(x => x.Machine?.Id.ToString(CultureInfo.CurrentCulture) == identifier);

        if (cellLocation == null)
            return NotFound(new MoryxExceptionResponse { Title = "Cell/Resource not found" });

        var converter = new Converter.Converter(_serialization, _logger);
        var cell = _resourceManager.ReadUnsafe(cellLocation.Machine.Id, r => r);
        return converter.ToResourceChangedModel(cell)?.CellPropertySettings;
    }

    // ToDo: Unused in MORYX 10 can be removed. If transports should be reflected, use different model
    /// <summary>
    /// Traces the route between 2 machines locations
    /// </summary>
    /// <param name="route"> Model of the route</param>
    [HttpPost("traceroute")]
    public async Task<ActionResult> TraceRoute(TransportRouteModel route)
    {
        var path = route.Paths.ToList();
        var originCellLocation = _resourceManager.GetResource<IMachineLocation>(l => l.Machine.Id == route.IdCellOfOrigin);
        var destinationCellLocation = _resourceManager.GetResource<IMachineLocation>(l => l.Machine.Id == route.IdCellOfDestination);

        var destinationPath = originCellLocation.Destinations.FirstOrDefault(dest => dest.Destination == destinationCellLocation);
        if (destinationPath == null)
        {
            var bareOrigin = (MachineLocation)_resourceManager.ReadUnsafe(originCellLocation, bare => bare);
            var bareDestination = (MachineLocation)_resourceManager.ReadUnsafe(destinationCellLocation, bare => bare);

            await _resourceManager.CreateUnsafeAsync(typeof(TransportPath), resource =>
            {
                var newPath = (TransportPath)resource;
                newPath.Name = $"{originCellLocation.Name}=>{destinationCellLocation.Name}";
                newPath.Origin = bareOrigin;
                newPath.Destination = bareDestination;
                newPath.WayPoints = path.Select(x => new Position { PositionX = x.PositionX, PositionY = x.PositionY }).ToList();
                bareOrigin.Children.Add(newPath);
                return Task.CompletedTask;
            });
        }
        else
        {
            await _resourceManager.ModifyUnsafeAsync(destinationPath, r =>
            {
                ((ITransportPath)r).WayPoints = path;
                return Task.FromResult(true);
            });
        }
        return Ok();
    }

    /// <summary>
    /// Updates the settings of a cell
    /// </summary>
    /// <param name="settings"> Model of the settings (icon,image)</param>
    [HttpPut("cell-settings/{id}")]
    public async Task<ActionResult> CellSettings(long id, CellSettingsModel settings)
    {
        var cellLocation = _resourceManager.GetResource<IMachineLocation>(l => l.Machine?.Id == id);

        if (cellLocation is null)
            return NotFound();

        await _resourceManager.ModifyUnsafeAsync(cellLocation, r =>
        {
            ((IMachineLocation)r).SpecificIcon = string.IsNullOrEmpty(settings.Icon) ? ((IMachineLocation)r).SpecificIcon : settings.Icon;
            ((IMachineLocation)r).Image = string.IsNullOrEmpty(settings.Image) ? ((IMachineLocation)r).Image : settings.Image;
            return Task.FromResult(true);
        });
        return Ok();
    }

    /// <summary>
    /// Retourn the list of all the route between machines/cells
    /// </summary>
    [HttpPost("routes")]
    public ActionResult<List<TransportRouteModel>> GetRoutes()
    {
        var locations = _resourceManager.GetResources<IMachineLocation>()?.ToList();
        return FactoryMonitorHelper.CreateRoutes(locations);
    }

    private ICell GetReferencedCellOrChildCell(IMachineLocation location)
    {
        var resource = _resourceManager.ReadUnsafe(location.Id, e => e);

        if (resource is not IMachineLocation locationResource)
        {
            throw new ArgumentException($"This cannot happen, unless there is a bug in the {_resourceManager}", nameof(location));
        }

        // Location is referencing a cell directly
        if (locationResource.Machine is ICell referencedCell)
        {
            // ToDo: Return Resource Proxy instead
            return referencedCell;
        }

        // Use a child cell instead
        return GetChildCell(resource);
    }

    private static ICell GetChildCell(Resource resource)
    {
        // Location has no children so we don't have a referenced cell
        if (!resource.Children.Any())
        {
            return null;
        }

        var childCell = resource.Children.FirstOrDefault(x => x is ICell) as ICell;
        if (childCell is null)
        {
            foreach (var notACell in resource.Children)
            {
                var cell = GetChildCell(notACell);
                if (cell is not null)
                {
                    return cell;
                }
            }
        }

        return childCell;
    }
}
