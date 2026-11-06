// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Net;
using System.Net.ServerSentEvents;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moryx.AbstractionLayer.Products;
using Moryx.AspNetCore;
using Moryx.Orders.Endpoints.Models;
using Moryx.Orders.Endpoints.Properties;
using Moryx.Users;

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

    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    public OrderManagementController(IOrderManagement orderManagement, IUserManagement userManagement = null)
    {
        _orderManagement = orderManagement;
        _userManagement = userManagement;
    }

    #region HttpGet
    /// <summary>
    /// Retrieve all current operations that match the given filter parameters.
    /// Already completed or aborted Operations won't be included.
    /// </summary>
    /// <param name="orderNumber">Optional. If specified only operations that belong to this order will be returned</param>
    /// <param name="operationNumber">Optional. If specified only operations with this operation number will be returned</param>
    [HttpGet]
    [ProducesResponseType(typeof(OperationModel[]), StatusCodes.Status200OK)]
    [Authorize(Policy = OrderPermissions.CanView)]
    public ActionResult<OperationModel[]> GetOperations(string orderNumber = null, string operationNumber = null)
    {
        Func<Operation, bool> filter = (orderNumber, operationNumber) switch
        {
            (null, null) => _ => true,
            (string order, null) => o => o.Order.Number == order,
            (null, string operation) => o => o.Number == operation,
            (string order, string operation) => o => o.Number == operation && o.Order.Number == order
        };
        var operations = _orderManagement.GetOperations(filter)
            .Select(Converter.ToModel).ToArray();

        return operations;
    }

    /// <summary>
    /// Stream that allow subscribing to operation changes.
    /// </summary>
    [HttpGet("stream")]
    [ProducesSseEvent<OperationTypes>(OperationTypes.Start, typeof(OperationStartedModel), Description = "Published when operation has been started")]
    [ProducesSseEvent<OperationTypes>(OperationTypes.Progress, typeof(OperationModel))]
    [ProducesSseEvent<OperationTypes>(OperationTypes.Completed, typeof(OperationReportedModel))]
    [ProducesSseEvent<OperationTypes>(OperationTypes.Interrupted, typeof(OperationReportedModel))]
    [ProducesSseEvent<OperationTypes>(OperationTypes.Report, typeof(OperationReportedModel))]
    [ProducesSseEvent<OperationTypes>(OperationTypes.Advice, typeof(OperationAdvicedModel))]
    [ProducesSseEvent<OperationTypes>(OperationTypes.Update, typeof(OperationModel))]
    public async Task OperationStream(CancellationToken cancellationToken)
    {
        var channel = Channel.CreateUnbounded<SseItem<string>>();

        // Define event handlers using the broadcast helper
        EventHandler<OperationChangedEventArgs> updateEventHandler = (_, e) =>
            Broadcast(nameof(OperationTypes.Update), Converter.ToModel(e.Operation));

        EventHandler<OperationAdviceEventArgs> adviceEventHandler = (_, e) =>
            Broadcast(nameof(OperationTypes.Advice), new OperationAdvicedModel
            {
                OperationModel = Converter.ToModel(e.Operation),
                Advice = Converter.ToModel(e.Advice)
            });

        EventHandler<OperationReportEventArgs> reportEventHandler = (_, e) =>
            Broadcast(nameof(OperationTypes.Report), new OperationReportedModel
            {
                OperationModel = Converter.ToModel(e.Operation),
                Report = Converter.ToModel(e.Report)
            });

        EventHandler<OperationChangedEventArgs> interruptedEventHandler = (_, e) =>
            Broadcast(nameof(OperationTypes.Interrupted), Converter.ToModel(e.Operation));

        EventHandler<OperationReportEventArgs> completedEventHandler = (_, e) =>
            Broadcast(nameof(OperationTypes.Completed), new OperationReportedModel
            {
                OperationModel = Converter.ToModel(e.Operation),
                Report = Converter.ToModel(e.Report)
            });

        EventHandler<OperationStartedEventArgs> startedEventHandler = (_, e) =>
            Broadcast(nameof(OperationTypes.Start), new OperationStartedModel
            {
                OperationModel = Converter.ToModel(e.Operation),
                UserId = e.User.Identifier
            });

        EventHandler<OperationChangedEventArgs> changedEventHandler = (_, e) =>
            Broadcast(nameof(OperationTypes.Progress), Converter.ToModel(e.Operation));

        try
        {
            var result = TypedResults.ServerSentEvents(Subscribe(cancellationToken));

            // Register event handlers after result creation but before execution to ensure finally cleanup
            _orderManagement.OperationUpdated += updateEventHandler;
            _orderManagement.OperationAdviced += adviceEventHandler;
            _orderManagement.OperationPartialReport += reportEventHandler;
            _orderManagement.OperationInterrupted += interruptedEventHandler;
            _orderManagement.OperationCompleted += completedEventHandler;
            _orderManagement.OperationStarted += startedEventHandler;
            _orderManagement.OperationProgressChanged += changedEventHandler;

            await result.ExecuteAsync(HttpContext);
        }
        catch (OperationCanceledException)
        {
            // client disconnected — this is expected, not an error
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
        }

        return;

        IAsyncEnumerable<SseItem<string>> Subscribe(CancellationToken cancelToken)
        {
            return channel.Reader.ReadAllAsync(cancelToken);
        }

        // Local helper to push operation events to the client
        void Broadcast(string eventType, object data)
        {
            channel.Writer.TryWrite(new SseItem<string>(JsonSerializer.Serialize(data, _serializerOptions), eventType));
        }
    }

    /// <summary>
    /// Retrieves an operationmodel using the unique id of the operation
    /// (Can load completed operations from storage)
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Retrieve the documents attached to operation with the given id
    /// </summary>
    /// <param name="guid">Id of the operation</param>
    /// <returns>A list of document models describing the available documents</returns>
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

    /// <summary>
    /// Retrieves the content of the document specified using the operation id and the identifier of the document
    /// </summary>
    /// <param name="guid">Id of the operation</param>
    /// <param name="identifier">Identifier of the document</param>
    /// <returns></returns>
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
        // TODO: Change the return value here. This is not a bad request. It's not the callers fault that the ContentType is not set
        if (string.IsNullOrEmpty(document.ContentType))
            return BadRequest();

        return new FileStreamResult(document.GetStream(), document.ContentType);
    }

    /// <summary>
    /// Retrieve a list of product parts attached to the given operation
    /// </summary>
    /// <param name="guid">If of the operation</param>
    /// <returns>The list of product parts</returns>
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

    /// <summary>
    /// Retrieves the begin context for the given operation
    /// </summary>
    /// <param name="guid">Id of the operation</param>
    /// <returns>An object that can be used to popualte a begin dialog to the user</returns>
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

    /// <summary>
    /// Retrieves a report context for the given operation 
    /// </summary>
    /// <param name="guid">Id of the operation</param>
    /// <returns>An object that can be used to populate a report dialog for the user</returns>
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

    /// <summary>
    /// Retrieves a interrupt context for the given operation 
    /// </summary>
    /// <param name="guid">Id of the operation</param>
    /// <returns>An object that can be used to populate a interrupt dialog for the user</returns>
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
    /// <summary>
    /// Retrieves a advice context for the given operation 
    /// </summary>
    /// <param name="guid">Id of the operation</param>
    /// <returns>An object that can be used to populate a advice dialog for the user</returns>
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

    /// <summary>
    /// Retrieves logs written during the creation of the operation
    /// </summary>
    /// <param name="guid">Id of the operation</param>
    /// <returns>A list of log messages</returns>
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

    /// <summary>
    /// Get's the list of recipes available to produce the given product
    /// </summary>
    /// <param name="identifier">Product identifier</param>
    /// <param name="revision">Product revision</param>
    /// <returns></returns>
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
    /// <summary>
    /// Add an operation to the order stock
    /// </summary>
    /// <param name="contextModel">Description of the operation</param>
    /// <param name="sourceId">Identifier of the application adding the operation</param>
    /// <returns></returns>
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

    /// <summary>
    /// Begin producing the operation using the given parameters
    /// </summary>
    /// <param name="guid">Id of the operation</param>
    /// <param name="beginModel">Begin parameters</param>
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

    /// <summary>
    /// Abort the specified operation (Remove it before being started)
    /// </summary>
    /// <param name="guid">Id of the operation</param>
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

    /// <summary>
    /// Report produced amounts of a given operation
    /// </summary>
    /// <param name="guid">Id of the operation</param>
    /// <param name="report">Parameter object</param>
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
        {
            return NotFound(new MoryxExceptionResponse { Title = Strings.OrderManagementController_OperationNotFound });
        }

        await _orderManagement.ReportOperationAsync(operation, Converter.FromModel(report, _userManagement));
        return Ok();
    }

    /// <summary>
    /// Interrupt the given operation
    /// </summary>
    /// <param name="guid">Id of the operation</param>
    /// <param name="userIdentifier">Identifier of the user requesting the interrupt (Optional depending on configuration)</param>
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
        {
            return NotFound(new MoryxExceptionResponse { Title = Strings.OrderManagementController_OperationNotFound });
        }

        var user = string.IsNullOrEmpty(userIdentifier) ? null : _userManagement?.GetUser(userIdentifier);

        await _orderManagement.InterruptOperationAsync(operation, user);
        return Ok();
    }

    /// <summary>
    /// Advice operation
    /// </summary>
    /// <param name="guid">Id of the operation</param>
    /// <param name="advice">Parameter object</param>
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

    /// <summary>
    /// Update the operation
    /// </summary>
    /// <param name="guid">Id of the operation</param>
    /// <param name="update">Paramter object</param>
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

    /// <summary>
    /// Reload the operation, rerunning the order assignment steps
    /// </summary>
    /// <param name="guid">Id of the operation</param>
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
