// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Runtime.Modules;
using Moryx.Serialization;
using static Moryx.VisualInstructions.Endpoints.Converter;

namespace Moryx.VisualInstructions.Endpoints;

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
    private readonly ILogger<VisualInstructionsController> _logger;
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };
    private readonly Converter _converter;
    public VisualInstructionsController(IVisualInstructions visualInstructions, IModuleManager moduleManager, IServiceProvider serviceProvider, ILogger<VisualInstructionsController> logger)
    {
        _visualInstructions = visualInstructions;
        _logger = logger;
        _converter = new Converter(new PossibleValuesSerialization(moduleManager.AllModules.FirstOrDefault(module => module is IFacadeContainer<IVisualInstructions>)?.Container, serviceProvider, new EmptyValueProvider())); ; 
    }

    [HttpGet("stream")]
    [ProducesResponseType(typeof(InstructionModel[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [Authorize(Policy = VisualInstructionsPermissions.CanView)]
    public async Task InstructionStream(CancellationToken cancellationToken)
    {
        // Read identifier cookie
        var identifier = Request.Cookies[CookieName];
        if (identifier == null)
        {
            BadRequest($"The expected cookie {CookieName} was not send. Make sure the cookie is sent and try again.");
            return;
        }

        var channel = Channel.CreateUnbounded<string>();
        // Define event handlers using the broadcast helper
        EventHandler<InstructionEventArgs> eventHandler = (_, e) =>
        {
            if (e.Identifier == identifier)
            {
                Broadcast();
            }
        };

        EventHandler<bool> stateChangedEventHandler = (args, ready) =>
        {
            if (ready)
            {
                Broadcast();
            }
        };

        try
        {
            var result = TypedResults.ServerSentEvents(Subscribe(cancellationToken));
            // Register event handlers after result creation but before execution to ensure finally cleanup
            _visualInstructions.InstructionAdded += eventHandler;
            _visualInstructions.InstructionCleared += eventHandler;
            if (_visualInstructions is ILifeCycleBoundFacade lf)
            {
                lf.StateChanged += stateChangedEventHandler;
            } 

            await result.ExecuteAsync(HttpContext);
        }
        catch (OperationCanceledException)
        {
            // client disconnected — this is expected, not an error
        }
        finally
        {
            _visualInstructions.InstructionAdded -= eventHandler;
            _visualInstructions.InstructionCleared -= eventHandler;
            if (_visualInstructions is ILifeCycleBoundFacade lf)
            {
                lf.StateChanged -= stateChangedEventHandler;
            } 
        }

        return;

        async IAsyncEnumerable<SseItem<string>> Subscribe([EnumeratorCancellation] CancellationToken cancelToken)
        {
            
            InstructionModel[] initialInstructions = [];

            try
            {
                
                // Send all instructions as first item
                initialInstructions = _visualInstructions.GetInstructions(identifier).Select(_converter.ToModel).ToArray();
            }
            catch(HealthStateException)
            {
                // Ignore if module is not ready yet.
            }
            yield return new SseItem<string>(JsonSerializer.Serialize(initialInstructions, _serializerOptions));

            await foreach (var data in channel.Reader.ReadAllAsync(cancelToken))
            {
                yield return new SseItem<string>(data);
            }
        }

        // Local helper to broadcast instruction changes to all matching subscribers
        void Broadcast()
        {
            var instructions = _visualInstructions.GetInstructions(identifier).Select(_converter.ToModel).ToArray();
            channel.Writer.TryWrite(JsonSerializer.Serialize(instructions, _serializerOptions));
        }
    }

    [HttpGet("{identifier}")]
    [ProducesResponseType(typeof(InstructionModel[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [Authorize(Policy = VisualInstructionsPermissions.CanView)]
    public ActionResult<InstructionModel[]> GetAll(string identifier)
    {
        if (string.IsNullOrEmpty(identifier))
            return BadRequest($"{identifier} is not a valid identifier");

        return _visualInstructions.GetInstructions(identifier).Select(_converter.ToModel).ToArray();
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
