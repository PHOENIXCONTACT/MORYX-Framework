// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading.Channels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moryx.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Moryx.VisualInstructions.Endpoints
{
    /// <summary>
    /// Definition of a REST API on the <see cref="IVisualInstructions"/> facade.
    /// </summary>
    [ApiController]
    [Route("api/moryx/instructions/")]
    [Produces("application/json")]
    public class VisualInstructionsController : ControllerBase
    {
        private const string CookieName = "moryx-client-identifier";
        private readonly IVisualInstructions _visualInstructions;

        private static readonly JsonSerializerSettings _serializerSettings = CreateSerializerSettings();

        private static JsonSerializerSettings CreateSerializerSettings()
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            serializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            return serializerSettings;
        }

        public VisualInstructionsController(IVisualInstructions visualInstructions)
            => _visualInstructions = visualInstructions;

        [HttpGet("stream")]
        [ProducesResponseType(typeof(InstructionModel[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Authorize(Policy = VisualInstructionsPermissions.CanView)]
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

            var instructionSetChannel = Channel.CreateUnbounded<string>();

            // Define event handling
            var eventHandler = new EventHandler<InstructionEventArgs>((_, eventArgs) =>
            {
                if (eventArgs.Identifier != identifier)
                {
                    return;
                }

                WriteIdentifiers(identifier, instructionSetChannel);
            });
            _visualInstructions.InstructionAdded += eventHandler;
            _visualInstructions.InstructionCleared += eventHandler;

            try
            {
                // Send initial instruction set via stream
                WriteIdentifiers(identifier, instructionSetChannel);

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
                _visualInstructions.InstructionAdded -= eventHandler;
                _visualInstructions.InstructionCleared -= eventHandler;

                instructionSetChannel.Writer.TryComplete();
            }

            await response.CompleteAsync();
        }

        private void WriteIdentifiers(string identifier, Channel<string> instructionSetChannel)
        {
            var instructions = _visualInstructions.GetInstructions(identifier).Select(Converter.ToModel);
            var json = JsonConvert.SerializeObject(instructions, _serializerSettings);
            instructionSetChannel.Writer.TryWrite(json);
        }

        [HttpGet("{identifier}")]
        [ProducesResponseType(typeof(InstructionModel[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Authorize(Policy = VisualInstructionsPermissions.CanView)]
        public ActionResult<InstructionModel[]> GetAll(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
                return BadRequest($"{identifier} is not a valid identifier");

            return _visualInstructions.GetInstructions(identifier).Select(Converter.ToModel).ToArray();
        }

        [HttpPost("{identifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Policy = VisualInstructionsPermissions.CanAdd)]
        public void AddInstruction(string identifier, InstructionModel instruction)
        {
            _visualInstructions.AddInstruction(identifier, Converter.FromModel(instruction));
        }

        [HttpDelete("{identifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Policy = VisualInstructionsPermissions.CanClear)]
        public void ClearInstruction(string identifier, InstructionModel instruction)
        {
            _visualInstructions.ClearInstruction(identifier, Converter.FromModel(instruction));
        }

        [HttpPut("{identifier}/response")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = VisualInstructionsPermissions.CanComplete)]
        public void CompleteInstruction(string identifier, InstructionResponseModel response)
        {
            var activeInstruction = _visualInstructions.GetInstructions(identifier)?.FirstOrDefault(ai => ai.Id == response.Id);
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

            _visualInstructions.CompleteInstruction(identifier, instructionResponse);
        }

        [HttpGet("instructors")]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
        [Authorize(Policy = VisualInstructionsPermissions.CanView)]
        public ActionResult<string[]> GetInstructors()
        {
            return _visualInstructions.GetInstructors().ToArray();
        }
    }
}

