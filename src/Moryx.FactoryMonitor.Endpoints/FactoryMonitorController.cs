// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Processes;
using Moryx.Factory;
using Moryx.ControlSystem.Cells;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Moryx.Orders;
using Moryx.AbstractionLayer.Capabilities;
using System.Timers;
using Moryx.FactoryMonitor.Endpoints.Models;
using Moryx.AbstractionLayer.Processes;
using Moryx.AspNetCore;
using Moryx.FactoryMonitor.Endpoints.Properties;
//old models in '.Model' namespace. Only ones still in use: TransoirtRoute- / PathModel & CellSettingsModel
using Moryx.FactoryMonitor.Endpoints.Model;
using Moryx.FactoryMonitor.Endpoints.Extensions;
using Timer = System.Timers.Timer;

namespace Moryx.FactoryMonitor.Endpoints
{
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

        private List<ICell> _cells;
        private Timer resourceChangedTimer;
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
            var locations = _resourceManager.GetResources<IMachineLocation>();
            var cells = locations.Where(CellFilterBaseOnLocation)
                .Select(l => l.Machine)
                .Cast<ICell>().ToList();
            var activityChangedModels = new List<ActivityChangedModel>();
            var cellStateChangedModels = new List<CellStateChangedModel>();
            var resourceChangedModels = new List<ResourceChangedModel>();
            var orderModels = new List<OrderModel>();

            var activities = _processControl.GetRunningProcesses()
                .Select(p => p.CurrentActivity())
                .Where(a => a is not null && a.Tracing is not null);
            var converter = new Converter(_serialization);

            foreach (var cell in cells)
            {
                if (activities.Any(a => a.Tracing.ResourceId == cell.Id && a.Tracing.Started is not null && a.Tracing.Completed is null))
                {
                    var activity = activities.FirstOrDefault(a => a.Tracing.ResourceId == cell.Id);
                    //to-do: handle multiple activities in one cell
                    activityChangedModels.Add(cell.GetActivityChangedModel(activity, _orderManager.GetOrderModels(_colorPalette)));
                    cellStateChangedModels.Add(cell.GetCellStateChangedModel(ActivityProgress.Running, _resourceManager.ReadUnsafe(cell.Id, r => r)));
                }
                else
                {
                    cellStateChangedModels.Add(cell.GetCellStateChangedModel(_resourceManager.ReadUnsafe(cell.Id, r => r)));
                }
                resourceChangedModels.Add(cell.GetResourceChangedModel(converter, _resourceManager, CellFilterBaseOnLocation));
            }

            activityChangedModels = activityChangedModels.OrderBy(x => x.ResourceId).ToList();
            cellStateChangedModels = cellStateChangedModels.OrderBy(x => x.Id).ToList();
            resourceChangedModels = resourceChangedModels.OrderBy(x => x.Id).ToList();
            orderModels = _orderManager.GetOrderModels(_colorPalette).OrderBy(x => x.Order).ThenBy(x => x.Operation).ToList();

            var factory = _resourceManager.GetRootFactory();

            var model = Converter.ToFactoryStateModel(factory);
            model.ActivityChangedModels = activityChangedModels;
            model.CellStateChangedModels = cellStateChangedModels;
            model.ResourceChangedModels = resourceChangedModels;
            model.OrderModels = orderModels;

            return model;
        }

        /// <summary>
        /// List of all the cells
        /// </summary>
        /// <returns></returns>
        [HttpGet("cells")]
        public ActionResult<List<VisualizableItemModel>> AllCells()
        {
            var cells = _resourceManager.GetResources<IMachineLocation>()
                .Where(CellFilterBaseOnLocation);

            var converter = new Converter(_serialization);
            return cells.Select(x => new SimpleGraph { Id = x.Id }.ToVisualItemModel(_resourceManager, _logger, converter, CellFilterBaseOnLocation)).ToList();
        }

        /// <summary>
        /// Get the list of displayable item for this current factory view
        /// </summary>
        [HttpGet("factory-content/{factoryId}")]
        public ActionResult<List<VisualizableItemModel>> FactoryContent(long factoryId)
        {
            // check if there is a factory with the given id
            var factory = _resourceManager.GetResource<IManufacturingFactory>(x => x.Id == factoryId);
            if (factory is null) return NotFound(Strings.FactoryMonitorController_FactoryNotFound_);
            var converter = new Converter(_serialization);

            var root = _resourceManager.GetRootFactory();
            SimpleGraph graph = _resourceManager.ReadUnsafe(factory.Id, e => SimpleGraph.Create(e as ManufacturingFactory));

            //root level (Factory)
            if (root.Id == factoryId)
                return graph.Children.Select(e => e.ToVisualItemModel(_resourceManager, _logger, converter, CellFilterBaseOnLocation))
                     .Where(x => x is not null).ToList();

            // 1 level tree graph
            graph = graph.GetSubGraphById(factoryId);
            if (graph is null) return NotFound();

            var output = graph.Children.Select(e => e.ToVisualItemModel(_resourceManager, _logger, converter, CellFilterBaseOnLocation))
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
            if (factory is not ManufacturingFactory manufacturingFactory) BadRequest(Strings.FactoryMonitorController_FactoryNotFound_);
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
        public async Task FactoryStatesStream(CancellationToken cancelToken)
        {
            var response = Response;
            response.Headers.Append("Content-Type", "text/event-stream");

            // Configure Serialization Settings
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            serializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            var _factoryChannel = Channel.CreateUnbounded<Tuple<string, string>>();

            resourceChangedTimer = new();
            resourceChangedTimer.Interval = 5000;
            resourceChangedTimer.AutoReset = true;
            resourceChangedTimer.Enabled = true;

            var locations = _resourceManager.GetResources<IMachineLocation>();
            _cells = locations.Where(CellFilterBaseOnLocation)
                .Select(l => l.Machine)
                .Cast<ICell>().ToList();
            if (_cells.Count == 0)
            {
                await response.WriteAsync("retry: 5000\n", cancellationToken: cancelToken);
                await response.CompleteAsync();
                return;
            }
            var converter = new Converter(_serialization);

            var resourceEventHandler = new ElapsedEventHandler(async (sender, eventArgs) =>
                await FactoryMonitorHelper.ResourceUpdated(serializerSettings, _factoryChannel, _resourceManager, CellFilterBaseOnLocation, converter, cancelToken)); //resource events are substitute with a timer event since there are no such events

            var capabilitiesEventHandler = new EventHandler<ICapabilities>(async (sender, eventArgs) =>
                await FactoryMonitorHelper.PublishCellUpdate((sender as ICell).GetCellStateChangedModel(_resourceManager.ReadUnsafe((sender as ICell).Id, r => r)), serializerSettings, _factoryChannel, cancelToken));

            var orderStartedEventHandler = new EventHandler<OperationStartedEventArgs>(async (sender, eventArgs) =>
                await FactoryMonitorHelper.OrderStarted(eventArgs, serializerSettings, _factoryChannel, cancelToken));

            var orderEventHandler = new EventHandler<OperationChangedEventArgs>(async (sender, eventArgs) =>
                await FactoryMonitorHelper.OrderUpdated(eventArgs, serializerSettings, _factoryChannel, cancelToken));

            var activityEventHandler = new EventHandler<ActivityUpdatedEventArgs>(async (sender, eventArgs) => await
                FactoryMonitorHelper.ActivityUpdated(eventArgs, serializerSettings, _factoryChannel, _cells, _resourceManager.ReadUnsafe(eventArgs.Activity.Tracing.ResourceId, r => r), converter, _orderManager.GetOrderModels(_colorPalette), cancelToken));

            foreach (var cell in _cells)
            {
                //register to cell notready-to-work event
                cell.CapabilitiesChanged += capabilitiesEventHandler;
            }

            _orderManager.OperationStarted += orderStartedEventHandler;
            _orderManager.OperationUpdated += orderEventHandler;
            _processControl.ActivityUpdated += activityEventHandler;
            resourceChangedTimer.Elapsed += resourceEventHandler;

            try
            {
                // Create infinite loop awaiting changes or cancellation
                while (!cancelToken.IsCancellationRequested)
                {
                    var changes = await _factoryChannel.Reader.ReadAsync(cancelToken);

                    await response.WriteAsync($"type: {changes.Item1}\n", cancelToken);
                    await response.WriteAsync($"data: {changes.Item2}\r\r", cancelToken);
                }
            }
            finally
            {
                // Unregister handler
                foreach (var cell in _cells)
                {
                    cell.CapabilitiesChanged -= capabilitiesEventHandler;
                }

                _orderManager.OperationStarted -= orderStartedEventHandler;
                _orderManager.OperationUpdated -= orderEventHandler;
                _processControl.ActivityUpdated -= activityEventHandler;
                resourceChangedTimer.Elapsed -= resourceEventHandler;

                _factoryChannel.Writer.Complete();
            }
            await response.CompleteAsync();
        }

        /// <summary>
        /// Return the location of the cell in the factory
        /// </summary>
        /// <returns><see cref="CellLocationModel"/></returns>
        [HttpPost("move-cell")]
        public async Task<ActionResult<CellLocationModel>> MoveCell(CellLocationModel location)
        {
            var cellLocation = _resourceManager.GetResource<IMachineLocation>(l => l.Id == location.Id);

            await _resourceManager.ModifyUnsafe(cellLocation, r =>
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
            await _resourceManager.ModifyUnsafe(manufacturingConfig.Id, r =>
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
                        .FirstOrDefault(x => x.Machine?.Id.ToString() == identifier);

            if (cellLocation == null)
                return NotFound(new MoryxExceptionResponse { Title = "Cell/Resource not found" });

            var converter = new Converter(_serialization);
            var cell = _resourceManager.ReadUnsafe(cellLocation.Machine.Id, r => r);
            return converter.ToResourceChangedModel(cell)?.CellPropertySettings;
        }

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

                await _resourceManager.CreateUnsafe(typeof(TransportPath), resource =>
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
                await _resourceManager.ModifyUnsafe(destinationPath, r =>
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

            await _resourceManager.ModifyUnsafe(cellLocation, r =>
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

        private bool CellFilterBaseOnLocation(IMachineLocation locationParam)
        {
            var location = _resourceManager.ReadUnsafe(locationParam.Id, e => (MachineLocation)e);
            var machine = location.Children?.FirstOrDefault(x => x is ICell);

            if (location is null || machine is null) return false;

            return true;
        }
    }
}

