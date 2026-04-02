// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Net;
using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moryx.AbstractionLayer.Resources;
using Moryx.Operators.Attendances;
using Moryx.Operators.Endpoints.Models;
using Moryx.Operators.Exceptions;
using Moryx.Operators.Skills;
using Moryx.Runtime.Modules;

namespace Moryx.Operators.Endpoints;

/// <summary>
/// Definition of a REST API on the <see cref="IOperatorManagement"/> facade.
/// TODO: Remove unused resourceManagement paramter in next major
/// </summary>
[ApiController]
[Route("api/moryx/operators/")]
[Produces("application/json")]
public class OperatorManagementController(
    IOperatorManagement operatorManagement,
    IAttendanceManagement attendanceManagement,
    IResourceManagement resourceManagement,
    ISkillManagement skillManagement) : ControllerBase
{
    private const string SignInStatusChangedEventType = "signInChanged";
    private readonly IOperatorManagement _operatorManagement = operatorManagement;
    private readonly IAttendanceManagement _attendanceManagement = attendanceManagement;
    private readonly ISkillManagement _skillManagement = skillManagement;

    #region IOperatorManagement

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = OperatorPermissions.CanManage)]
    public ActionResult<string> Add(OperatorModel model)
        => Response(() => _operatorManagement.AddOperator(model.ToType()));

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("{identifier}")]
    [Authorize(Policy = OperatorPermissions.CanManage)]
    public ActionResult<string> Update([FromRoute] string identifier, [FromBody] OperatorModel model)
    {
        model.Identifier = identifier;
        return Response(() => _operatorManagement.UpdateOperator(model.ToType()));
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = OperatorPermissions.CanManage)]
    public ActionResult<string> DeleteOperator(string operatorIdentifier)
        => Response(() => _operatorManagement.DeleteOperator(WebUtility.HtmlEncode(operatorIdentifier)));

    #endregion

    #region IAttandanceManagement

    [HttpGet]
    [Authorize(Policy = OperatorPermissions.CanView)]
    public ActionResult<AssignableOperator[]> GetAll()
        => Response(() => _attendanceManagement.Operators.ToArray());

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("{identifier}")]
    [Authorize(Policy = OperatorPermissions.CanManage)]
    public ActionResult<ExtendedOperatorModel> Get(string identifier)
        => Response(() => RetrieveOperator(identifier).ToModel());

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("get-operators-by-resource/{resourceId}")]
    [Authorize(Policy = OperatorPermissions.CanManage)]
    public ActionResult<IEnumerable<ExtendedOperatorModel>> GetOperatorsByResource(long resourceId)
    {
        return Response(() =>
        {
            var resource = RetrieveResource(resourceId);
            var operators = _attendanceManagement.Operators;

            return operators
            .Where(@operator => resource.RequiredSkills.ProvidedBy(_skillManagement.GetAcquiredCapabilities(@operator)))
            .Select(assignableOperator => assignableOperator.ToModel());
        });
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("resources/{operatorIdentifier}")]
    [Authorize(Policy = OperatorPermissions.CanManage)]
    public ActionResult<IEnumerable<AttendableResourceModel>> GetResources(string? operatorIdentifier = null)
    {
        return Response(() =>
        {
            var attendableResources =
                _attendanceManagement is IAttendanceManagementExtended extended
                ? extended.Assignables
                : resourceManagement.GetAssignableResources();
            // return all the resources
            if (string.IsNullOrEmpty(operatorIdentifier))
            {
                return attendableResources.Select(Converter.ToModel);
            }
            var @operator = RetrieveOperator(operatorIdentifier);
            return attendableResources
                .Where(attendableResource => attendableResource.RequiredSkills.ProvidedBy(_skillManagement.GetAcquiredCapabilities(@operator)))
                .Select(Converter.ToModel);
        });
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("resources")]
    [Authorize(Policy = OperatorPermissions.CanManage)]
    public ActionResult<IEnumerable<AttendableResourceModel>> GetResources() => GetResources(null);

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("default")]
    [Authorize(Policy = OperatorPermissions.CanManage)]
    public ActionResult<ExtendedOperatorModel?> GetDefaultOperator()
        => Response(() => _attendanceManagement.DefaultOperator?.ToModel());

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("signin")]
    [Authorize(Policy = OperatorPermissions.CanManage)]
    public ActionResult SignIn(string operatorIdentifier, long resourceId)
    {
        return Response(() =>
        {
            var @operator = RetrieveOperator(@operatorIdentifier);
            var resource = RetrieveResource(resourceId);
            _attendanceManagement.SignIn(@operator, resource);
            NotifyResource(resource);
        });
    }

    private void NotifyResource(IOperatorAssignable resource)
    {
        var attendance =
                _attendanceManagement is IAttendanceManagementExtended extended
                ? extended.GetAttendingOperators(resource)
                : _attendanceManagement.Operators
                    .Where(o => o.AssignedResources.Any(r => r.Id == resource.Id))
                    .Select(o => new AttendanceChangedArgs(o, _skillManagement.GetSkills(o).ToArray()))
                    .ToArray();

        resource.AttendanceChanged(attendance);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("signout")]
    [Authorize(Policy = OperatorPermissions.CanManage)]
    public ActionResult SignOut(string operatorIdentifier, long resourceId)
    {
        return Response((Action)(() =>
        {
            var @operator = RetrieveOperator(operatorIdentifier);
            IOperatorAssignable resource = RetrieveResource(resourceId);
            _attendanceManagement.SignOut(@operator, resource);
            NotifyResource(resource);
        }));
    }

    /// <summary>
    /// Provides events related to operators
    /// <list type="termdef">
    /// <item>
    /// <term>signInChanged</term>
    /// <description><see cref="SignInStatusChangedModel"/></description>
    /// </item>
    /// <item>
    /// <term>Creation|Update|Deletion</term>
    /// <description><see cref="OperatorModel"/></description>
    /// </item>
    /// </list>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpGet("stream")]
    public IResult Stream(CancellationToken token)
    {
        var stream = GetChangeStream(token);
        return TypedResults.ServerSentEvents(stream);
    }

    private async IAsyncEnumerable<SseItem<object>> GetChangeStream([EnumeratorCancellation] CancellationToken token)
    {
        var channel = Channel.CreateUnbounded<SseItem<object>>();
        void OnStatusChanged(object? sender, SignInStatusChangedArgs e)
        {
            channel.Writer.TryWrite(new(Converter.ToModel(e), SignInStatusChangedEventType));
        }

        void OnOperatorChanged(object? sender, OperatorChangedEventArgs e)
        {
            var sse = new SseItem<object>(
                    Converter.ToModel(e.Operator),
                    e.Change.ToString());
            channel.Writer.TryWrite(sse);
        }

        _operatorManagement.OperatorChanged += OnOperatorChanged;
        if (_attendanceManagement is IAttendanceManagementExtended extended)
        {
            extended.SignInStatusChanged += OnStatusChanged;
        }

        await foreach (var item in channel.Reader.ReadAllAsync(token))
        {
            yield return item;
        }
        if (_attendanceManagement is IAttendanceManagementExtended extended2)
        {
            extended2.SignInStatusChanged -= OnStatusChanged;
        }

        _operatorManagement.OperatorChanged -= OnOperatorChanged;
    }

    #endregion

    private new ActionResult<TResult> Response<TResult>(Func<TResult> func, Func<TResult, ActionResult<TResult>>? onSuccess = null)
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

    private new ActionResult Response(Action action)
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
        OperatorNotFoundException _ => NotFound(ex.Message),
        AlreadyExistsException _ => Conflict(ex.Message),
        ArgumentNullException _ => BadRequest(ex.Message),
        ArgumentException _ => BadRequest(ex.Message),
        KeyNotFoundException _ => StatusCode(500, ex.Message),
        HealthStateException _ => StatusCode(500, ex.Message),
        _ => StatusCode(500, ex.Message),
    };

    private AssignableOperator RetrieveOperator(string operatorIdentifier)
        => _attendanceManagement.GetOperator(WebUtility.HtmlEncode(operatorIdentifier)) ?? throw new OperatorNotFoundException(operatorIdentifier);

    private IOperatorAssignable RetrieveResource(long resourceId)
    {
        if (_attendanceManagement is IAttendanceManagementExtended extended)
        {
            return extended.GetAssignable(resourceId) ?? throw new ResourceNotFoundException(resourceId);
        }
        else
        {
            return resourceManagement.GetAssignableResource(resourceId) ?? throw new ResourceNotFoundException(resourceId);
        }
    }
}

