// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moryx.AbstractionLayer.Products;
using Moryx.Asp.Extensions;
using Moryx.Orders.Endpoints.Properties;
using Moryx.Users;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Channels;
using Moryx.Orders.Endpoints.Models;

namespace Moryx.Orders.Endpoints
{
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
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
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
            return _orderManagement.GetOperations(o => true)
                .Where(o => orderNumber is null || o.Order.Number == orderNumber)
                .Where(o => operationNumber is null || o.Number == operationNumber)
                .Select(Converter.ToModel).ToArray();
        }

        [HttpGet("stream")]
        [ProducesResponseType(typeof(OperationChangedModel), StatusCodes.Status200OK)]
        public async Task OperationStream(CancellationToken cancelToken)
        {
            var response = Response;
            response.Headers["Content-Type"] = "text/event-stream";

            var operationsChannel = Channel.CreateUnbounded<Tuple<string, string>>();

            // Define event handling
            var updateEventHandler = new EventHandler<OperationChangedEventArgs>((_, eventArgs) =>
            {
                var json = JsonConvert.SerializeObject(Converter.ToModel(eventArgs.Operation), _serializerSettings);
                operationsChannel.Writer.TryWrite(new Tuple<string, string>(OperationTypes.Update.ToString(), json));
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
                operationsChannel.Writer.TryWrite(new Tuple<string, string>(OperationTypes.Advice.ToString(), json));
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
                operationsChannel.Writer.TryWrite(new Tuple<string, string>(OperationTypes.Report.ToString(), json));
            });
            _orderManagement.OperationPartialReport += reportEventHandler;

            var interruptedEventHandler = new EventHandler<OperationReportEventArgs>((_, eventArgs) =>
            {
                var interruptedOperation = new OperationReportedModel
                {
                    OperationModel = Converter.ToModel(eventArgs.Operation),
                    Report = Converter.ToModel(eventArgs.Report)
                };
                var json = JsonConvert.SerializeObject(interruptedOperation, _serializerSettings);
                operationsChannel.Writer.TryWrite(new Tuple<string, string>(OperationTypes.Interrupted.ToString(), json));
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
                operationsChannel.Writer.TryWrite(new Tuple<string, string>(OperationTypes.Completed.ToString(), json));
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
                operationsChannel.Writer.TryWrite(new Tuple<string, string>(OperationTypes.Start.ToString(), json));
            });
            _orderManagement.OperationStarted += startedEventHandler;

            var changedEventHandler = new EventHandler<OperationChangedEventArgs>((_, eventArgs) =>
            {
                var json = JsonConvert.SerializeObject(Converter.ToModel(eventArgs.Operation), _serializerSettings);
                operationsChannel.Writer.TryWrite(new Tuple<string, string>(OperationTypes.Progress.ToString(), json));
            });
            _orderManagement.OperationProgressChanged += changedEventHandler;

            try
            {
                // Create infinite loop awaiting changes or cancellation
                while (!cancelToken.IsCancellationRequested)
                {
                    var changes = await operationsChannel.Reader.ReadAsync(cancelToken);

                    await response.WriteAsync($"event: {changes.Item1}\n", cancelToken);
                    await response.WriteAsync($"data: {changes.Item2}\r\r", cancelToken);
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
        public ActionResult<OperationModel> GetOperation(Guid guid)
        {
            var operation = _orderManagement.GetOperation(guid);
            if (operation == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.OPERATION_NOT_FOUND });

            return Converter.ToModel(operation);
        }

        [HttpGet]
        [ProducesResponseType(typeof(DocumentModel[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
        [Route("{guid}/documents")]
        [Authorize(Policy = OrderPermissions.CanViewDocuments)]
        public ActionResult<DocumentModel[]> GetDocuments(Guid guid)
        {
            var operation = _orderManagement.GetOperation(guid);
            if (operation == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.DOCUMENT_NOT_FOUND });

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
            var operation = _orderManagement.GetOperation(guid);
            if (operation == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.DOCUMENT_NOT_FOUND });

            var document = operation.Documents.FirstOrDefault(x => x.Identifier == identifier);
            if (document == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.DOCUMENT_NOT_FOUND });

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
        public ActionResult<ProductPartModel[]> GetProductParts(Guid guid)
        {
            var operation = _orderManagement.GetOperation(guid);
            if (operation == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.PRODUCTPARTS_NOT_FOUND });

            return operation.Parts.Select(Converter.ToModel).ToArray();
        }

        [HttpGet]
        [ProducesResponseType(typeof(BeginContext), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
        [Route("{guid}/begin")]
        [Authorize(Policy = OrderPermissions.CanBegin)]
        public ActionResult<BeginContext> GetBeginContext(Guid guid)
        {
            var operation = _orderManagement.GetOperation(guid);
            if (operation == null)
            {
                return NotFound(new MoryxExceptionResponse { Title = Strings.OPERATION_NOT_FOUND });
            }

            var beginContext = _orderManagement.GetBeginContext(operation);
            if (beginContext == null)
            {
                return NotFound(new MoryxExceptionResponse { Title = Strings.OPERATION_NOT_FOUND });
            }

            return beginContext;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ReportContext), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
        [Route("{guid}/report")]
        [Authorize(Policy = OrderPermissions.CanReport)]
        public ActionResult<ReportContext> GetReportContext(Guid guid)
        {
            var operation = _orderManagement.GetOperation(guid);
            if (operation == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.OPERATION_NOT_FOUND });

            var reportContext = _orderManagement.GetReportContext(operation);
            if (reportContext == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.OPERATION_NOT_FOUND });

            return reportContext;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ReportContext), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
        [Route("{guid}/interrupt")]
        [Authorize(Policy = OrderPermissions.CanInterrupt)]
        public ActionResult<ReportContext> GetInterruptContext(Guid guid)
        {
            var operation = _orderManagement.GetOperation(guid);
            if (operation == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.OPERATION_NOT_FOUND });

            var reportContext = _orderManagement.GetInterruptContext(operation);
            if (reportContext == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.OPERATION_NOT_FOUND });

            return reportContext;
        }

        [HttpGet]
        [ProducesResponseType(typeof(AdviceContext), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
        [Route("{guid}/advice")]
        [Authorize(Policy = OrderPermissions.CanAdvice)]
        public ActionResult<AdviceContext> GetAdviceContext(Guid guid)
        {
            var operation = _orderManagement.GetOperation(guid);
            if (operation == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.OPERATION_NOT_FOUND });

            var adviceContext = _orderManagement.GetAdviceContext(operation);
            if (adviceContext == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.OPERATION_NOT_FOUND });

            return adviceContext;
        }

        [HttpGet]
        [Route("{guid}/logs")]
        [ProducesResponseType(typeof(OperationLogMessageModel[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
        [Authorize(Policy = OrderPermissions.CanView)]
        public ActionResult<OperationLogMessageModel[]> GetLogs(Guid guid)
        {
            var operation = _orderManagement.GetOperation(guid);
            if (operation == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.OPERATION_NOT_FOUND });

            return _orderManagement.GetLogs(operation).Select(Converter.ToModel).ToArray();
        }

        [HttpGet]
        [Route("recipes")]
        [ProducesResponseType(typeof(OperationRecipeModel[]), StatusCodes.Status200OK)]
        [Authorize(Policy = OrderPermissions.CanView)]
        public async Task<ActionResult<OperationRecipeModel[]>> GetAssignableRecipes(string identifier, short revision)
        {
            var identity = new ProductIdentity(WebUtility.HtmlEncode(identifier), revision);
            return (await _orderManagement.GetAssignableRecipes(identity)).Select(Converter.ToModel).ToArray();
        }
        #endregion

        #region HttpPost
        [HttpPost]
        [ProducesResponseType(typeof(OperationModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Authorize(Policy = OrderPermissions.CanAdd)]
        public ActionResult<OperationModel> AddOperation(OperationCreationContextModel contextModel, string sourceId = null)
        {
            var context = contextModel.ConvertToContext();

            if (context == null)
                return BadRequest("Context is null");

            if (sourceId == null)
                return Converter.ToModel(_orderManagement.AddOperation(context));
            else
                return Converter.ToModel(_orderManagement.AddOperation(context, new ClientOperationSource(sourceId)));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
        [Route("{guid}/begin")]
        [Authorize(Policy = OrderPermissions.CanBegin)]
        public ActionResult BeginOperation(Guid guid, BeginModel beginModel)
        {
            var operation = _orderManagement.GetOperation(guid);
            if (operation == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.OPERATION_NOT_FOUND });

            if (beginModel.UserId is null)
                _orderManagement.BeginOperation(operation, beginModel.Amount);
            else
                _orderManagement.BeginOperation(operation, beginModel.Amount, _userManagement?.GetUser(beginModel.UserId));

            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
        [Route("{guid}/abort")]
        [Authorize(Policy = OrderPermissions.CanInterrupt)]
        public ActionResult AbortOperation(Guid guid)
        {
            var operation = _orderManagement.GetOperation(guid);
            if (operation == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.OPERATION_NOT_FOUND });

            _orderManagement.AbortOperation(operation);
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
        [Route("{guid}/report")]
        [Authorize(Policy = OrderPermissions.CanReport)]
        public ActionResult ReportOperation(Guid guid, ReportModel report)
        {
            var operation = _orderManagement.GetOperation(guid);
            if (operation == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.OPERATION_NOT_FOUND });

            _orderManagement.ReportOperation(operation, Converter.FromModel(report, _userManagement));
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
        [Route("{guid}/interrupt")]
        [Authorize(Policy = OrderPermissions.CanInterrupt)]
        public ActionResult InterruptOperation(Guid guid, ReportModel report)
        {
            var operation = _orderManagement.GetOperation(guid);
            if (operation == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.OPERATION_NOT_FOUND });

            _orderManagement.InterruptOperation(operation, Converter.FromModel(report, _userManagement));
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
            var operation = _orderManagement.GetOperation(guid);
            if (operation == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.OPERATION_NOT_FOUND });

            var result = await _orderManagement.TryAdvice(operation, Converter.FromModel(advice, operation));
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
        [Route("{guid}/position")]
        [Authorize(Policy = OrderPermissions.CanManage)]
        public ActionResult SetOperationSortOrder(Guid guid, [FromBody] int sortOrder)
        {
            var operation = _orderManagement.GetOperation(guid);
            if (operation == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.OPERATION_NOT_FOUND });

            _orderManagement.SetOperationSortOrder(sortOrder, operation);
            return Ok();
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
        [Route("{guid}/reload")]
        [Authorize(Policy = OrderPermissions.CanManage)]
        public ActionResult Reload(Guid guid)
        {
            var operation = _orderManagement.GetOperation(guid);
            if (operation == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.OPERATION_NOT_FOUND });

            _orderManagement.Reload(operation);
            return Ok();
        }
        #endregion

    }
}

