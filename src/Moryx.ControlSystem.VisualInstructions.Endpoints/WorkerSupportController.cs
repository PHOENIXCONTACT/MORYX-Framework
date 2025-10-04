// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Serialization;
using Moryx.Serialization;
using System.Threading.Channels;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.ControlSystem.VisualInstructions.Endpoints
{
    /// <summary>
    /// Definition of a REST API on the <see cref="IWorkerSupport"/> facade.
    /// </summary>
    [ApiController]
    [Route("api/moryx/instructions/")]
    [Produces("application/json")]
    public class WorkerSupportController : ControllerBase
    {
        private const string CookieName = "moryx-client-identifier";
        private readonly IResourceManagement _resourceMgmt;

        private static readonly JsonSerializerSettings _serializerSettings = CreateSerializerSettings();

        private static JsonSerializerSettings CreateSerializerSettings()
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            serializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            return serializerSettings;
        }

        public WorkerSupportController(IResourceManagement resourceManagement)
            => _resourceMgmt = resourceManagement;

        [HttpGet("stream")]
        [ProducesResponseType(typeof(InstructionModel[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Authorize(Policy = VisualInstructionPermissions.CanView)]

        public async Task InstructionStream(CancellationToken cancelToken)
        {
            var response = Response;
            response.Headers.Add("Content-Type", "text/event-stream");

            // Read identifier cookie
            var identifier = Request.Cookies[CookieName];
            if (identifier == null)
            {
                await response.CompleteAsync();
                BadRequest($"The expected cookie {CookieName} was not send. Make sure the cookie is sent and try again.");
                return;
            }

            var instructor = _resourceMgmt.GetResource<IVisualInstructionSource>(identifier);
            if (instructor == null)
            {
                await response.CompleteAsync();
                BadRequest($"There is no resource with identifier {identifier}. Make sure the identifier is correct and try again.");
                return;
            }

            var instructionSetChannel = Channel.CreateUnbounded<string>();

            // Define event handling
            var eventHandler = new EventHandler<ActiveInstruction>((_, eventArgs) =>
            {
                var json = JsonConvert.SerializeObject(instructor.Instructions, _serializerSettings);
                instructionSetChannel.Writer.TryWrite(json);
            });
            instructor.Added += eventHandler;
            instructor.Cleared += eventHandler;

            try
            {
                // Send initial instruction set via stream
                var json = JsonConvert.SerializeObject(instructor.Instructions, _serializerSettings);
                instructionSetChannel.Writer.TryWrite(json);

                // Create infinite loop awaiting changes or cancellation
                while (!cancelToken.IsCancellationRequested)
                {
                    var changes = await instructionSetChannel.Reader.ReadAsync(cancelToken);
                    await response.WriteAsync($"data: {changes}\r\r", cancelToken);
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
                instructor.Added -= eventHandler;
                instructor.Cleared -= eventHandler;

                instructionSetChannel.Writer.TryComplete();
            }

            await response.CompleteAsync();
        }

        [HttpGet("{identifier}")]
        [ProducesResponseType(typeof(InstructionModel[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Authorize(Policy = VisualInstructionPermissions.CanView)]
        public ActionResult<InstructionModel[]> GetAll(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
                return BadRequest($"{identifier} is not a valid identifier");

            var instructor = _resourceMgmt.GetResource<IVisualInstructionSource>(identifier);
            if (instructor == null)
                return NotFound($"There is no resource with identifier {identifier}");

            return instructor.Instructions.Select(Converter.ToModel).ToArray();
        }

        [HttpPut("{identifier}/response")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = VisualInstructionPermissions.CanComplete)]
        public void CompleteInstruction(string identifier, InstructionResponseModel response)
        {
            var instructor = _resourceMgmt.GetResource<IVisualInstructionSource>(identifier);
            if (instructor is null)
                NotFound($"There is no resource with identifier {identifier}");

            var activeInstruction = instructor.Instructions.FirstOrDefault(ai => ai.Id == response.Id);
            if (activeInstruction is null)
                NotFound($"There is no active instruction corresponding to response id {response.Id}");

            var instructionResponse = new ActiveInstructionResponse
            {
                Id = response.Id,
                SelectedResult = new InstructionResult
                {
                    Key = response.SelectedResult?.Key ?? response.Result,
                    DisplayValue = response.SelectedResult?.DisplayValue
                }
            };

            // Update inputs if any were given
            if (response.Inputs != null && activeInstruction.Inputs != null)
            {
                EntryConvert.UpdateInstance(activeInstruction.Inputs, response.Inputs);
                instructionResponse.Inputs = activeInstruction.Inputs;
            }

            instructor.Completed(instructionResponse);
        }

        [HttpGet("instructors")]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
        [Authorize(Policy = VisualInstructionPermissions.CanView)]
        public ActionResult<string[]> GetInstructors()
        {
            var instructors = _resourceMgmt.GetResources<IVisualInstructionSource>();
            return instructors.Select(i => i.Identifier).ToArray();
        }
    }
}

