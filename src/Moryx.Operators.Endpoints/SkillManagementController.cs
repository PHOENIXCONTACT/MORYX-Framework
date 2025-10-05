// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moryx.AbstractionLayer.Resources;
using Moryx.Operators.Exceptions;
using Moryx.Operators.Skills;
using Moryx.Runtime.Modules;

namespace Moryx.Operators.Endpoints;

/// <summary>
/// Definition of a REST API on the <see cref="ISkillManagement"/> facade.
/// </summary>
[ApiController]
[Route("api/moryx/skills/")]
[Produces("application/json")]
public class SkillManagementController(ISkillManagement skillManagement, IOperatorManagement operatorManagement) : ControllerBase
{
    private readonly ISkillManagement _skillManagement = skillManagement;
    private readonly IOperatorManagement _operatorManagement = operatorManagement;

    #region Types

    [HttpGet("types")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public ActionResult<IEnumerable<SkillTypeModel>> GetTypes() =>
        Response(() => _skillManagement.SkillTypes.Select(t => t.ToModel(false)));

    [HttpGet("types/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [Authorize(Policy = SkillPermissions.CanManage)]
    public ActionResult<SkillTypeModel> GetType(long id) =>
        Response(() => _skillManagement.GetSkillType(id)?.ToModel(true) ?? throw new KeyNotFoundException($"No skill with id {id} could be found."));

    [HttpPost("types")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [Authorize(Policy = SkillPermissions.CanManage)]
    public ActionResult<SkillTypeModel> Create(SkillTypeCreationContextModel model) =>
        Response(() => _skillManagement.CreateSkillType(model.ToContext()).ToModel(true), result => Created("", result));

    [HttpPut("types")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [Authorize(Policy = SkillPermissions.CanManage)]
    public ActionResult Update(SkillTypeModel model) =>
        Response(() => _skillManagement.UpdateSkillType(model.ToType()), () => NoContent());

    [HttpDelete("types/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [Authorize(Policy = SkillPermissions.CanManage)]
    public ActionResult DeleteType(long id) =>
        Response(() => _skillManagement.DeleteSkillType(id), () => NoContent());

    [HttpGet("types/prototype")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public ActionResult<SkillTypeModel> GetTypePrototype() => 
        Response(() => new SkillType("Enter Skill Name", new SimpleSkill()).ToModel(true));

    #endregion

    #region Skills

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [Authorize(Policy = SkillPermissions.CanView)]
    public ActionResult<IEnumerable<SkillModel>> GetSkills() =>
        Response(() => _skillManagement.Skills.Select(s => s.ToModel()));

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [Authorize(Policy = SkillPermissions.CanManage)]
    public ActionResult<SkillModel> GetSkill(long id) =>
        Response(() => _skillManagement.GetSkill(id)?.ToModel() ?? throw new KeyNotFoundException($"No skill with id {id} could be found."));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [Authorize(Policy = SkillPermissions.CanManage)]
    public ActionResult<SkillModel> Create(SkillCreationContextModel model) =>
        Response(() => _skillManagement.CreateSkill(model.ToContext(_operatorManagement, _skillManagement)).ToModel(), result => Created("api/moryx/skills/"+result.Id, result));

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [Authorize(Policy = SkillPermissions.CanManage)]
    public ActionResult DeleteSkill(long id) =>
        Response(() => _skillManagement.DeleteSkill(id), () => NoContent());

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

    private new ActionResult Response(Action func, Func<ActionResult>? onSuccess = null)
    {
        try
        {
            func();
            if (onSuccess != null)
            {
                return onSuccess();
            }
            else
            {
                return Ok();
            }
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
        AlreadyExistsException _ => BadRequest(ex.Message),
        ArgumentNullException _ => BadRequest(ex.Message),
        ArgumentException _ => BadRequest(ex.Message),
        KeyNotFoundException _ => StatusCode(500, ex.Message),
        HealthStateException _ => StatusCode(500, ex.Message),
        _ => StatusCode(500, ex.Message),
    };
}

