// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moryx.ControlSystem.Jobs.Endpoints.Properties;
using System.Threading.Channels;
using Moryx.AspNetCore;

namespace Moryx.ControlSystem.Jobs.Endpoints;

/// <summary>
/// Definition of a REST API on the <see cref="IJobManagement"/> facade.
/// </summary>
[ApiController]
[Route("api/moryx/jobs/")]
[Produces("application/json")]
public class JobManagementController : ControllerBase
{
    private readonly IJobManagement _jobManagement;

    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    public JobManagementController(IJobManagement jobManagement)
        => _jobManagement = jobManagement;

    [HttpGet]
    [Route("{jobId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = JobPermissions.CanView)]
    public ActionResult<JobModel> GetJob(long jobId)
    {
        var job = _jobManagement.Get(jobId);
        if (job == null)
            return NotFound(new MoryxExceptionResponse { Title = Strings.JobManagementController_JobNotFound });

        return Converter.ToModel(job);
    }

    [HttpGet]
    [Authorize(Policy = JobPermissions.CanView)]
    public ActionResult<JobModel[]> GetAll()
    {
        return _jobManagement.GetAll().Select(Converter.ToModel).ToArray();
    }

    [HttpPost]
    [Route("{jobId}/complete")]
    [Authorize(Policy = JobPermissions.CanComplete)]
    public ActionResult Complete(long jobId)
    {
        var job = _jobManagement.Get(jobId);
        if (job == null)
            return NotFound(new MoryxExceptionResponse { Title = Strings.JobManagementController_JobNotFound });

        _jobManagement.Complete(job);
        return Ok();
    }

    [HttpPost]
    [Route("{jobId}/abort")]
    [Authorize(Policy = JobPermissions.CanAbort)]
    public ActionResult Abort(long jobId)
    {
        var job = _jobManagement.Get(jobId);
        if (job == null)
            return NotFound(new MoryxExceptionResponse { Title = Strings.JobManagementController_JobNotFound });

        _jobManagement.Abort(job);
        return Ok();
    }

    [HttpGet("stream")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task ProgressStream(CancellationToken cancellationToken)
    {
        var channel = Channel.CreateUnbounded<string>();

        // Define event handlers using the broadcast helper
        EventHandler<Job> progressEventHandler = (_, job) =>
            Broadcast(job);

        EventHandler<JobStateChangedEventArgs> stateEventHandler = (_, e) =>
            Broadcast(e.Job);

        try
        {
            var result = TypedResults.ServerSentEvents(Subscribe(cancellationToken));

            // Register event handlers after result creation but before execution to ensure finally cleanup
            _jobManagement.ProgressChanged += progressEventHandler;
            _jobManagement.StateChanged += stateEventHandler;

            await result.ExecuteAsync(HttpContext);
        }
        catch (OperationCanceledException)
        {
            // client disconnected — this is expected, not an error
        }
        finally
        {
            _jobManagement.ProgressChanged -= progressEventHandler;
            _jobManagement.StateChanged -= stateEventHandler;
        }

        return;

        IAsyncEnumerable<string> Subscribe(CancellationToken cancelToken)
        {
            return channel.Reader.ReadAllAsync(cancelToken);
        }

        // Local helper to push job updates to the client
        void Broadcast(Job job)
        {
            channel.Writer.TryWrite(JsonSerializer.Serialize(Converter.ToModel(job), _serializerOptions));
        }
    }
}
