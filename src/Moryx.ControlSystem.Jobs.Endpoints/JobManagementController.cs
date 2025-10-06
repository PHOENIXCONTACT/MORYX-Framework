// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moryx.Asp.Extensions;
using Moryx.ControlSystem.Jobs.Endpoints.Properties;
using Newtonsoft.Json;
using System.Threading.Channels;
using Newtonsoft.Json.Serialization;

namespace Moryx.ControlSystem.Jobs.Endpoints
{
    /// <summary>
    /// Definition of a REST API on the <see cref="IJobManagement"/> facade.
    /// </summary>
    [ApiController]
    [Route("api/moryx/jobs/")]
    [Produces("application/json")]
    public class JobManagementController : ControllerBase
    {
        private readonly IJobManagement _jobManagement;

        private static readonly JsonSerializerSettings _serializerSettings = CreateSerializerSettings();

        private static JsonSerializerSettings CreateSerializerSettings()
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            serializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            return serializerSettings;
        }

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
                return NotFound(new MoryxExceptionResponse { Title = Strings.JOB_NOT_FOUND });

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
                return NotFound(new MoryxExceptionResponse { Title = Strings.JOB_NOT_FOUND });

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
                return NotFound(new MoryxExceptionResponse { Title = Strings.JOB_NOT_FOUND });

            _jobManagement.Abort(job);
            return Ok();
        }

        private static void WriteToEventQueue(ChannelWriter<string> writer, Job job)
        {
            var serialized = JsonConvert.SerializeObject(Converter.ToModel(job), _serializerSettings);
            writer.TryWrite(serialized);
        }

        [HttpGet("stream")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task ProgressStream(CancellationToken cancelToken)
        {
            var response = Response;
            response.Headers["Content-Type"] = "text/event-stream";

            // Define event handling
            var jobUpdates = Channel.CreateUnbounded<string>();
            var progressEventHandler = new EventHandler<Job>((_, job) => WriteToEventQueue(jobUpdates.Writer, job));
            var stateEventHandler = new EventHandler<JobStateChangedEventArgs>((_, eventArgs) => WriteToEventQueue(jobUpdates.Writer, eventArgs.Job));

            // Register handler to facade
            _jobManagement.ProgressChanged += progressEventHandler;
            _jobManagement.StateChanged += stateEventHandler;

            try
            {
                // Create infinite loop awaiting changes or cancellation
                while (!cancelToken.IsCancellationRequested)
                {
                    // Await entry in queue
                    var streamContent = await jobUpdates.Reader.ReadAsync(cancelToken);
                    await response.WriteAsync($"data: {streamContent}\r\r", cancelToken);
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
                _jobManagement.ProgressChanged -= progressEventHandler;
                _jobManagement.StateChanged -= stateEventHandler;

                jobUpdates.Writer.TryComplete();
            }

            await response.CompleteAsync();
        }
    }
}

