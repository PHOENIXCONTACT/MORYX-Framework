// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moryx.AbstractionLayer.Products;
using Moryx.Orders.Endpoints.Properties;
using Moryx.Users;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Channels;
using Moryx.AspNetCore;
using Moryx.Orders.Endpoints.Models;

namespace Moryx.Orders.Endpoints;

/// <summary>
/// Definition of a REST API on the <see cref="IOrderManagement"/> facade.
/// </summary>
[ApiController]
[Route("api/moryx/orders/")]
[Produces("application/json")]
public class OrderManagementController : ControllerBase
{
    private readonly IOrderManagement _orderManagement;
    private readonly IUserManagement _userManagement;

    private static readonly JsonSerializerSettings _serializerSettings = CreateSerializerSettings();

    private static JsonSerializerSettings CreateSerializerSettings()
    {
        var serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        serializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        return serializerSettings;
    }

    public OrderManagementController(IOrderManagement orderManagement, IUserManagement userManagement = null)
    {
        _orderManagement = orderManagement;
        _userManagement = userManagement;
    }

    #region HttpGet
    [HttpGet]
    [ProducesResponseType(typeof(OperationModel[]), StatusCodes.Status200OK)]
    [Authorize(Policy = OrderPermissions.CanView)]
    public ActionResult<OperationModel[]> GetOperations(string orderNumber = null, string operationNumber = null)
    {
        return _orderManagement.GetOperations(_ => true)
            .Where(o => orderNumber is null || o.Order.Number == orderNumber)
            .Where(o => operationNumber is null || o.Number == operationNumber)
            .Select(Converter.ToModel).ToArray();
    }

    [HttpGet("stream")]
    [ProducesResponseType(typeof(OperationChangedModel), StatusCodes.Status200OK)]
    public async Task OperationStream(CancellationToken cancellationToken)
    {
        var response = Response;
        response.Headers["Content-Type"] = "text/event-stream";

        var operationsChannel = Channel.CreateUnbounded<Tuple<string, string>>();

        // Define event handling
        var updateEventHandler = new EventHandler<OperationChangedEventArgs>((_, eventArgs) =>
        {
            var json = JsonConvert.SerializeObject(Converter.ToModel(eventArgs.Operation), _serializerSettings);
            operationsChannel.Writer.TryWrite(new Tuple<string, string>(nameof(OperationTypes.Update), json));
        });
        _orderManagement.OperationUpdated += updateEventHandler;

        var adviceEventHandler = new EventHandler<OperationAdviceEventArgs>((_, eventArgs) =>
        {
            var advicedOperation = new OperationAdvicedModel
            {
                OperationModel = Converter.ToModel(eventArgs.Operation),
                Advice = Converter.ToModel(eventArgs.Advice)
            };
            var json = JsonConvert.SerializeObject(advicedOperation, _serializerSettings);
            operationsChannel.Writer.TryWrite(new Tuple<string, string>(nameof(OperationTypes.Advice), json));
        });
        _orderManagement.OperationAdviced += adviceEventHandler;

        var reportEventHandler = new EventHandler<OperationReportEventArgs>((_, eventArgs) =>
        {
            var reportedOperation = new OperationReportedModel
            {
                OperationModel = Converter.ToModel(eventArgs.Operation),
                Report = Converter.ToModel(eventArgs.Report)
            };
            var json = JsonConvert.SerializeObject(reportedOperation, _serializerSettings);
            operationsChannel.Writer.TryWrite(new Tuple<string, string>(nameof(OperationTypes.Report), json));
        });
        _orderManagement.OperationPartialReport += reportEventHandler;

        var interruptedEventHandler = new EventHandler<OperationChangedEventArgs>((_, eventArgs) =>
        {
            var json = JsonConvert.SerializeObject(Converter.ToModel(eventArgs.Operation), _serializerSettings);
            operationsChannel.Writer.TryWrite(new Tuple<string, string>(nameof(OperationTypes.Interrupted), json));
        });
        _orderManagement.OperationInterrupted += interruptedEventHandler;

        var completedEventHandler = new EventHandler<OperationReportEventArgs>((_, eventArgs) =>
        {
            var completedOperation = new OperationReportedModel
            {
                OperationModel = Converter.ToModel(eventArgs.Operation),
                Report = Converter.ToModel(eventArgs.Report)
            };
            var json = JsonConvert.SerializeObject(completedOperation, _serializerSettings);
            operationsChannel.Writer.TryWrite(new Tuple<string, string>(nameof(OperationTypes.Completed), json));
        });
        _orderManagement.OperationCompleted += completedEventHandler;

        var startedEventHandler = new EventHandler<OperationStartedEventArgs>((_, eventArgs) =>
        {
            var startedOperation = new OperationStartedModel
            {
                OperationModel = Converter.ToModel(eventArgs.Operation),
                UserId = eventArgs.User.Identifier
            };
            var json = JsonConvert.SerializeObject(startedOperation, _serializerSettings);
            operationsChannel.Writer.TryWrite(new Tuple<string, string>(nameof(OperationTypes.Start), json));
        });
        _orderManagement.OperationStarted += startedEventHandler;

        var changedEventHandler = new EventHandler<OperationChangedEventArgs>((_, eventArgs) =>
        {
            var json = JsonConvert.SerializeObject(Converter.ToModel(eventArgs.Operation), _serializerSettings);
            operationsChannel.Writer.TryWrite(new Tuple<string, string>(nameof(OperationTypes.Progress), json));
        });
        _orderManagement.OperationProgressChanged += changedEventHandler;

        try
        {
            // Create infinite loop awaiting changes or cancellation
            while (!cancellationToken.IsCancellationRequested)
            {
                var changes = await operationsChannel.Reader.ReadAsync(cancellationToken);

                await response.WriteAsync($"event: {changes.Item1}\n", cancellationToken);
                await response.WriteAsync($"data: {changes.Item2}\r\r", cancellationToken);
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
            _orderManagement.OperationUpdated -= updateEventHandler;
            _orderManagement.OperationAdviced -= adviceEventHandler;
            _orderManagement.OperationPartialReport -= reportEventHandler;
            _orderManagement.OperationInterrupted -= interruptedEventHandler;
            _orderManagement.OperationCompleted -= completedEventHandler;
            _orderManagement.OperationStarted -= startedEventHandler;
            _orderManagement.OperationProgressChanged -= changedEventHandler;

            operationsChannel.Writer.TryComplete();
        }

        await response.CompleteAsync();
    }

    [HttpGet]
    [ProducesResponseType(typeof(OperationModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
    [Route("{guid}")]
    [Authorize(Policy = OrderPermissions.CanView)]
    public async Task<ActionResult<OperationModel>> GetOperation(Guid guid)
    {
        var operation = await _orderManagement.LoadOperationAsync(guid);
        if (operation == null)
            return NotFound(new MoryxExceptionResponse { Title = Strings.OrderManagementController_OperationNotFound });

        return Converter.ToModel(operation);
    }

    [HttpGet]
    [ProducesResponseType(typeof(DocumentModel[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
    [Route("{guid}/documents")]
    [Authorize(Policy = OrderPermissions.CanViewDocuments)]
    public async Task<ActionResult<DocumentModel[]>> GetDocuments(Guid guid)
    {
        var operation = await _orderManagement.LoadOperationAsync(guid);
        if (operation == null)
            return NotFound(new MoryxExceptionResponse { Title = Strings.OrderManagementController_GetDocuments_DocumentNotFound });

        return operation.Documents.Select(Converter.ToModel).ToArray();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
    [Route("{guid}/document/{identifier}/stream")]
    [ResponseCache(NoStore = false, Location = ResponseCacheLocation.Client, Duration = 86400)]
    [Authorize(Policy = OrderPermissions.CanViewDocuments)]
    public async Task<IActionResult> GetDocumentStream(Guid guid, string identifier)
    {
        var operation = await _orderManagement.LoadOperationAsync(guid);
        if (operation == null)
            return NotFound(new MoryxExceptionResponse { Title = Strings.OrderManagementController_GetDocuments_DocumentNotFound });

        var document = operation.Documents.FirstOrDefault(x => x.Identifier == identifier);
        if (document == null)
            return NotFound(new MoryxExceptionResponse { Title = Strings.OrderManagementController_GetDocuments_DocumentNotFound });

        if (string.IsNullOrEmpty(document.ContentType))
            return BadRequest();

        return new FileStreamResult(document.GetStream(), document.ContentType);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ProductPartModel[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
    [Route("{guid}/productparts")]
    [Authorize(Policy = OrderPermissions.CanView)]
    public async Task<ActionResult<ProductPartModel[]>> GetProductParts(Guid guid)
    {
        var operation = await _orderManagement.LoadOperationAsync(guid);
        if (operation == null)
            return NotFound(new MoryxExceptionResponse { Title = Strings.OrderManagementController_GetProductParts_ProductPartsNotFound });

        return operation.Parts.Select(Converter.ToModel).ToArray();
    }

    [HttpGet]
    [ProducesResponseType(typeof(BeginContext), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
    [Route("{guid}/begin")]
    [Authorize(Policy = OrderPermissions.CanBegin)]
    public async Task<ActionResult<BeginContext>> GetBeginContext(Guid guid)
    {
        var operation = await _orderManagement.LoadOperationAsync(guid);
        if (operation == null)
        {
            return NotFound(new MoryxExceptionResponse { Title = Strings.OrderManagementController_OperationNotFound });
        }

        var beginContext = _orderManagement.GetBeginContext(operation);
        if (beginContext == null)
        {
            return NotFound(new MoryxExceptionResponse { Title = Strings.OrderManagementController_OperationNotFound });
        }

        return beginContext;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ReportContext), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
    [Route("{guid}/report")]
    [Authorize(Policy = OrderPermissions.CanReport)]
    public async Task<ActionResult<ReportContext>> GetReportContext(Guid guid)
    {
        var operation = await _orderManagement.LoadOperationAsync(guid);
        if (operation == null)
            return NotFound(new MoryxExceptionResponse { Title = Strings.OrderManagementController_OperationNotFound });

        var reportContext = _orderManagement.GetReportContext(operation);
        if (reportContext == null)
            return NotFound(new MoryxExceptionResponse { Title = Strings.OrderManagementController_OperationNotFound });

        return reportContext;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ReportContext), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
    [Route("{guid}/interrupt")]
    [Authorize(Policy = OrderPermissions.CanInterrupt)]
    public async Task<ActionResult<ReportContext>> GetInterruptContext(Guid guid)
    {
        var operation = await _orderManagement.LoadOperationAsync(guid);
        if (operation == null)
            return NotFound(new MoryxExceptionResponse { Title = Strings.OrderManagementController_OperationNotFound });

        var reportContext = _orderManagement.GetInterruptContext(operation);
        if (reportContext == null)
            return NotFound(new MoryxExceptionResponse { Title = Strings.OrderManagementController_OperationNotFound });

        return reportContext;
    }

    [HttpGet]
    [ProducesResponseType(typeof(AdviceContext), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
    [Route("{guid}/advice")]
    [Authorize(Policy = OrderPermissions.CanAdvice)]
    public async Task<ActionResult<AdviceContext>> GetAdviceContext(Guid guid)
    {
        var operation = await _orderManagement.LoadOperationAsync(guid);
        if (operation == null)
            return NotFound(new MoryxExceptionResponse { Title = Strings.OrderManagementController_OperationNotFound });

        var adviceContext = _orderManagement.GetAdviceContext(operation);
        if (adviceContext == null)
            return NotFound(new MoryxExceptionResponse { Title = Strings.OrderManagementController_OperationNotFound });

        return adviceContext;
    }

    [HttpGet]
    [Route("{guid}/logs")]
    [ProducesResponseType(typeof(OperationLogMessageModel[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
    [Authorize(Policy = OrderPermissions.CanView)]
    public async Task<ActionResult<OperationLogMessageModel[]>> GetLogs(Guid guid)
    {
        var operation = await _orderManagement.LoadOperationAsync(guid);
        if (operation == null)
            return NotFound(new MoryxExceptionResponse { Title = Strings.OrderManagementController_OperationNotFound });

        return _orderManagement.GetLogs(operation).Select(Converter.ToModel).ToArray();
    }

    [HttpGet]
    [Route("recipes")]
    [ProducesResponseType(typeof(OperationRecipeModel[]), StatusCodes.Status200OK)]
    [Authorize(Policy = OrderPermissions.CanView)]
    public async Task<ActionResult<OperationRecipeModel[]>> GetAssignableRecipes(string identifier, short revision)
    {
        var identity = new ProductIdentity(WebUtility.HtmlEncode(identifier), revision);
        var assignableRecipes = await _orderManagement.GetAssignableRecipesAsync(identity);
        return assignableRecipes.Select(Converter.ToModel).ToArray();
    }
    #endregion

    #region HttpPost
    [HttpPost]
    [ProducesResponseType(typeof(OperationModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [Authorize(Policy = OrderPermissions.CanAdd)]
    public async Task<ActionResult<OperationModel>> AddOperation(OperationCreationContextModel contextModel, string sourceId = null)
    {
        var context = contextModel.ConvertToContext();

        if (context == null)
            return BadRequest("Context is null");

        if (sourceId == null)
        {
            var operation = await _orderManagement.AddOperationAsync(context);
            return Converter.ToModel(operation);
        }
        else
        {
            var operation = await _orderManagement.AddOperationAsync(context, new ClientOperationSource(sourceId));
            return Converter.ToModel(operation);
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
    [Route("{guid}/begin")]
    [Authorize(Policy = OrderPermissions.CanBegin)]
    public async Task<ActionResult> BeginOperation(Guid guid, BeginModel beginModel)
    {
        var operation = await _orderManagement.LoadOperationAsync(guid);
        if (operation == null)
            return NotFound(new MoryxExceptionResponse { Title = Strings.OrderManagementController_OperationNotFound });

        if (beginModel.UserId is null)
        {
            await _orderManagement.BeginOperationAsync(operation, beginModel.Amount);
        }
        else
        {
            await _orderManagement.BeginOperationAsync(operation, beginModel.Amount, _userManagement?.GetUser(beginModel.UserId));
        }

        return Ok();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
    [Route("{guid}/abort")]
    [Authorize(Policy = OrderPermissions.CanInterrupt)]
    public async Task<ActionResult> AbortOperation(Guid guid)
    {
        var operation = await _orderManagement.LoadOperationAsync(guid);
        if (operation == null)
            return NotFound(new MoryxExceptionResponse { Title = Strings.OrderManagementController_OperationNotFound });

        await _orderManagement.AbortOperationAsync(operation);
        return Ok();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
    [Route("{guid}/report")]
    [Authorize(Policy = OrderPermissions.CanReport)]
    public async Task<ActionResult> ReportOperation(Guid guid, ReportModel report)
    {
        var operation = await _orderManagement.LoadOperationAsync(guid);
        if (operation == null)
            return NotFound(new MoryxExceptionResponse { Title = Strings.OrderManagementController_OperationNotFound });

        await _orderManagement.ReportOperationAsync(operation, Converter.FromModel(report, _userManagement));
        return Ok();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
    [Route("{guid}/interrupt")]
    [Authorize(Policy = OrderPermissions.CanInterrupt)]
    public async Task<ActionResult> InterruptOperation(Guid guid, string userIdentifier)
    {
        var operation = await _orderManagement.LoadOperationAsync(guid);
        if (operation == null)
            return NotFound(new MoryxExceptionResponse { Title = Strings.OrderManagementController_OperationNotFound });

        var user = _userManagement?.GetUser(userIdentifier);

        await _orderManagement.InterruptOperationAsync(operation, user);
        return Ok();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
    [Route("{guid}/advice")]
    [Authorize(Policy = OrderPermissions.CanAdvice)]
    public async Task<ActionResult> AdviceOperation(Guid guid, AdviceModel advice)
    {
        var operation = await _orderManagement.LoadOperationAsync(guid);
        if (operation == null)
            return NotFound(new MoryxExceptionResponse { Title = Strings.OrderManagementController_OperationNotFound });

        var result = await _orderManagement.TryAdviceAsync(operation, Converter.FromModel(advice, operation));
        if (result.Success)
            return Ok();
        else
            return BadRequest(result.Message);
    }
    #endregion

    #region HttpPut

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
    [Route("{guid}/update")]
    [Authorize(Policy = OrderPermissions.CanManage)]
    public async Task<ActionResult> UpdateOperation(Guid guid, OperationUpdateModel update)
    {
        var operation = await _orderManagement.LoadOperationAsync(guid);
        if (operation == null)
            return NotFound(new MoryxExceptionResponse { Title = Strings.OrderManagementController_OperationNotFound });

        await _orderManagement.UpdateOperationAsync(operation, Converter.FromModel(update));
        return Ok();
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
    [Route("{guid}/reload")]
    [Authorize(Policy = OrderPermissions.CanManage)]
    public async Task<ActionResult> Reload(Guid guid)
    {
        var operation = await _orderManagement.LoadOperationAsync(guid);
        if (operation == null)
            return NotFound(new MoryxExceptionResponse { Title = Strings.OrderManagementController_OperationNotFound });

        await _orderManagement.ReloadAsync(operation);
        return Ok();
    }
    #endregion

}