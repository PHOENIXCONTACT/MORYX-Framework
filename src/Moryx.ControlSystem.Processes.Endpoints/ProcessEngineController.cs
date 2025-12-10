// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading.Channels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Resources;
using Moryx.AspNetCore;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.Processes.Endpoints.Extensions;
using Moryx.ControlSystem.Processes.Endpoints.Models;
using Moryx.ControlSystem.Processes.Endpoints.Properties;
using Moryx.ControlSystem.Processes.Endpoints.StreamServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Moryx.ControlSystem.Processes.Endpoints;

/// <summary>
/// Definition of a REST API on the <see cref="IProcessEnging"/> facade.
/// </summary>
[ApiController]
[Route("api/moryx/processes/")]
[Produces("application/json")]
public class ProcessEngineController : ControllerBase
{
    private readonly IProcessControl _processControl;
    private readonly IProductManagement _productManagement;
    private readonly IResourceManagement _resourceManagement;
    private readonly IJobManagement _jobManagement;

    private static readonly JsonSerializerSettings _serializerSettings = CreateSerializerSettings();

    private static JsonSerializerSettings CreateSerializerSettings()
    {
        var serializerSettings = new JsonSerializerSettings();
        serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        serializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        return serializerSettings;
    }

    public ProcessEngineController(IProcessControl processControl, IProductManagement productManagement,
        IResourceManagement resourceManagement, IJobManagement jobManagement)
    {
        _processControl = processControl;
        _productManagement = productManagement;
        _resourceManagement = resourceManagement;
        _jobManagement = jobManagement;
    }

    [HttpGet]
    [Route("job")]
    [ProducesResponseType(typeof(JobProcessModel[]), StatusCodes.Status200OK)]
    [Authorize(Policy = ProcessPermissions.CanView)]
    public ActionResult<JobProcessModel[]> GetRunningProcessesOfJob(long jobId, bool allProcesses)
    {
        try
        {
            var job = _jobManagement.Get(jobId);
            if (allProcesses)
            {
                return ConvertProcesses(job.AllProcesses);
            }

            return ConvertProcesses(job.RunningProcesses);
        }
        catch (Exception)
        {
            return Array.Empty<JobProcessModel>();
        }

    }

    [HttpGet]
    [Route("running")]
    [ProducesResponseType(typeof(JobProcessModel[]), StatusCodes.Status200OK)]
    [Authorize(Policy = ProcessPermissions.CanView)]
    public ActionResult<JobProcessModel[]> GetRunningProcesses()
    {
        var processes = _processControl.GetRunningProcesses();
        if (processes == null)
        {
            return Array.Empty<JobProcessModel>();
        }

        return ConvertProcesses(processes);
    }

    [HttpGet]
    [ProducesResponseType(typeof(JobProcessModel[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [Route("instance/{productInstanceId}")]
    [Authorize(Policy = ProcessPermissions.CanView)]
    public async Task<ActionResult<JobProcessModel[]>> GetProcesses(long productInstanceId)
    {
        var productInstance = _productManagement.GetInstance(productInstanceId);
        if (productInstance == null)
        {
            return NotFound($"No product instace corresponding to the Id {productInstanceId} found");
        }

        var processes = await _processControl.GetArchivedProcesses(productInstance);

        return ConvertProcesses(processes);
    }

    private JobProcessModel[] ConvertProcesses(IReadOnlyList<IProcess> processes)
    {
        var modelList = new List<JobProcessModel>();

        foreach (var p in processes)
        {
            modelList.Add(Converter.ConvertProcess(p, _processControl, _resourceManagement));
        }
        return modelList.ToArray();
    }

    [HttpGet]
    [ProducesResponseType(typeof(JobProcessModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [Route("running/{id}")]
    [Authorize(Policy = ProcessPermissions.CanView)]
    public ActionResult<JobProcessModel> GetProcess(long id)
    {
        var process = _processControl.GetRunningProcesses(p => p.Id == id).FirstOrDefault();
        if (process == null)
        {
            return NotFound(new MoryxExceptionResponse { Title = Strings.ProcessEngineController_ProcessNotFoundExceptionMessage });
        }

        return Converter.ConvertProcess(process, _processControl, _resourceManagement);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ProcessActivityModel[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [Route("running/{id}/activities")]
    [Authorize(Policy = ProcessPermissions.CanView)]
    public ActionResult<ProcessActivityModel[]> GetActivities(long id)
    {
        var process = _processControl.GetRunningProcesses(p => p.Id == id).FirstOrDefault();
        if (process == null)
        {
            return NotFound(new MoryxExceptionResponse { Title = Strings.ProcessEngineController_ProcessNotFoundExceptionMessage });
        }

        var activities = process.GetActivities();
        var modelList = new List<ProcessActivityModel>();

        foreach (var a in activities)
        {
            modelList.Add(Converter.ConvertActivity(a, _processControl, _resourceManagement));
        }
        return modelList.ToArray(); ;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ActivityResourceModel[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [Route("running/{id}/targets")]
    [Authorize(Policy = ProcessPermissions.CanView)]
    public ActionResult<ActivityResourceModel[]> GetTargets(long id)
    {
        var process = _processControl.GetRunningProcesses(p => p.Id == id).FirstOrDefault();
        if (process == null)
        {
            return NotFound(new MoryxExceptionResponse { Title = Strings.ProcessEngineController_ProcessNotFoundExceptionMessage });
        }

        var targetList = new List<ActivityResourceModel>();
        foreach (var t in _processControl.Targets(process))
        {
            targetList.Add(new ActivityResourceModel { Id = t.Id, Name = t.Name });
        }

        return targetList.ToArray();
    }

    [HttpGet]
    [ProducesResponseType(typeof(ActivityResourceModel[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
    [Route("running/{id}/targets/{activityId}")]
    [Authorize(Policy = ProcessPermissions.CanView)]
    public ActionResult<ActivityResourceModel[]> GetTargets(long id, long activityId)
    {
        var process = _processControl.GetRunningProcesses(p => p.Id == id).FirstOrDefault();
        if (process == null)
        {
            return NotFound(new MoryxExceptionResponse { Title = Strings.ProcessEngineController_ProcessNotFoundExceptionMessage });
        }

        var activity = process.GetActivities(a => a.Id == activityId).FirstOrDefault();
        if (activity == null)
        {
            return NotFound(new MoryxExceptionResponse { Title = Strings.ProcessEngineController_ActivityNotFoundExceptionMessage });
        }

        var targetList = new List<ActivityResourceModel>();
        foreach (var t in _processControl.Targets(activity))
        {
            targetList.Add(new ActivityResourceModel { Id = t.Id, Name = t.Name });
        }

        return targetList.ToArray();
    }

    [HttpGet]
    [Route("stream/processes")]
    [ProducesResponseType(typeof(JobProcessModel), StatusCodes.Status200OK)]
    public async Task ProcessUpdatesStream(CancellationToken cancelToken)
    {
        var response = Response;
        response.Headers["Content-Type"] = "text/event-stream";

        var serializerSettings = new JsonSerializerSettings();
        serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        serializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

        // Define event handling
        var processes = Channel.CreateUnbounded<ProcessUpdatedEventArgs>();
        var eventHandler = new EventHandler<ProcessUpdatedEventArgs>((_, processEventArgs) =>
        {
            processes.Writer.TryWrite(processEventArgs);
        });
        _processControl.ProcessUpdated += eventHandler;

        try
        {
            // Create infinite loop awaiting changes or cancellation
            while (!cancelToken.IsCancellationRequested)
            {
                // Write processes
                var processArgs = await processes.Reader.ReadAsync(cancelToken);
                var processModel = Converter.ConvertProcess(processArgs.Process, _processControl, _resourceManagement);
                processModel.State = processArgs.Progress;

                var json = JsonConvert.SerializeObject(processModel, _serializerSettings);
                await response.WriteAsync($"data: {json}\r\r", cancelToken);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (ChannelClosedException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        finally
        {
            // Unregister handler from facade
            _processControl.ProcessUpdated -= eventHandler;
            processes.Writer.TryComplete();
        }
        await response.CompleteAsync();
    }

    [HttpGet]
    [Route("stream/activities")]
    [ProducesResponseType(typeof(ProcessActivityModel), StatusCodes.Status200OK)]
    public async Task ActivitiesUpdatesStream(CancellationToken cancelToken)
    {
        var response = Response;
        response.Headers["Content-Type"] = "text/event-stream";
        var activities = Channel.CreateUnbounded<ActivityUpdatedEventArgs>();

        // Define event handling
        var eventHandler = new EventHandler<ActivityUpdatedEventArgs>((sender, activityEventArgs) =>
        {
            activities.Writer.TryWrite(activityEventArgs);
        });
        _processControl.ActivityUpdated += eventHandler;

        try
        {
            // Create infinite loop awaiting changes or cancellation
            while (!cancelToken.IsCancellationRequested)
            {
                // Write notifications
                var activityArgs = await activities.Reader.ReadAsync(cancelToken);
                var activityModel = Converter.ConvertActivity(activityArgs.Activity, _processControl, _resourceManagement);
                activityModel.State = activityArgs.Progress.ToString();

                var json = JsonConvert.SerializeObject(activityModel, _serializerSettings);
                await response.WriteAsync($"data: {json}\r\r", cancelToken);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (ChannelClosedException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        finally
        {
            // Unregister handler from facade
            _processControl.ActivityUpdated -= eventHandler;
            activities.Writer.TryComplete();
        }
        await response.CompleteAsync();
    }

    #region Process Holder

    [HttpGet("holders/groups")]
    [ProducesResponseType(typeof(ApiResponse<ProcessHolderGroupModel[]>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<ProcessHolderGroupModel[]>> GetGroups()
    {
        return HttpResponse(() =>
        {
            var allPositions = _resourceManagement.GetResourcesUnsafe<IProcessHolderPosition>(x => true);
            var ungrouped = allPositions.GetUngroupedPostions();
            var groups = _resourceManagement.GetResourcesUnsafe<IProcessHolderGroup>(x => true);

            return new ApiResponse<ProcessHolderGroupModel[]>([.. groups.ToDto(), .. ungrouped.ToDto()]);
        });
    }

    [HttpGet("holders/stream")]
    [ProducesResponseType(typeof(ProcessHolderGroupModel), StatusCodes.Status200OK)]
    public async Task GroupStream(CancellationToken cancelToken)
    {
        var stream = new ProcessHolderGroupStream(_resourceManagement, _serializerSettings);
        await stream.Start(HttpContext, cancelToken);
    }

    [HttpPost("holders/groups/{id:int}/reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult ResetGroup(long id)
    {
        return HttpResponse(() =>
        {
            var group = _resourceManagement.GetResource<IProcessHolderGroup>(x => x.Id == id) ?? throw new KeyNotFoundException($"No ProcessHolderGroup/Resource found with id '{id}'.");
            group.Reset();

        });
    }

    [HttpPost("holders/positions/{id:int}/reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult ResetPosition(long id)
    {
        return HttpResponse(() =>
        {
            var position = _resourceManagement.GetResource<IProcessHolderPosition>(x => x.Id == id);
            if (position == null)
            {
                throw new KeyNotFoundException($"No ProcessHolderPosition found with id '{id}'.");
            }

            position.Reset();

        });
    }
    #endregion

    private new ActionResult<TResult> HttpResponse<TResult>(Func<TResult> func, Func<TResult, ActionResult<TResult>> onSuccess = null)
    {
        try
        {
            var result = func();
            if (onSuccess != null)
            {
                return onSuccess(result);
            }
            else
            {
                return Ok(result);
            }
        }
        catch (Exception ex)
        {
            return MapToResponse(ex);
        }
    }

    private new ActionResult HttpResponse(Action action)
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
        ResourceNotFoundException _ => NotFound(ex.Message),
        ArgumentNullException _ => BadRequest(ex.Message),
        ArgumentException _ => BadRequest(ex.Message),
        KeyNotFoundException _ => StatusCode(500, ex.Message),
        _ => StatusCode(500, ex.Message),
    };
}
