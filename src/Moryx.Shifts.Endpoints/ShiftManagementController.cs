// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moryx.AbstractionLayer.Resources;
using Moryx.Operators;
using Moryx.Runtime.Modules;
using Moryx.Shifts.Endpoints.Models;
using Moryx.Shifts.Extensions;
using System.ComponentModel.DataAnnotations;
using Moryx.Shifts.Endpoints.Localizations;

namespace Moryx.Shifts.Endpoints
{
    /// <summary>
    /// Definition of a REST API on the <see cref="IShiftManagement"/> facade.
    /// </summary>
    [ApiController]
    [Route("api/moryx/shifts/")]
    [Produces("application/json")]
    public class ShiftManagementController : ControllerBase
    {
        private readonly IShiftManagement _shiftManagement;
        private readonly IResourceManagement _resourceManagement;
        private readonly IOperatorManagement _operatorManagement;

        public ShiftManagementController(IShiftManagement shiftManagement, IResourceManagement resourceManagement, IOperatorManagement operatorManagement)
        {
            _shiftManagement = shiftManagement;
            _resourceManagement = resourceManagement;
            _operatorManagement = operatorManagement;
        }

        #region Shifts

        [HttpGet]
        public ActionResult<IReadOnlyList<ShiftModel>> GetShifts()
        {
            try
            {
                var shifts = _shiftManagement.Shifts.Select(s => s.ToModel());
                return Ok(shifts);
            }
            catch (HealthStateException)
            {
                return InvalidModuleState();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("filter")]
        public ActionResult<IReadOnlyList<ShiftModel>> GetShifts([FromQuery, DataType(DataType.Time)] DateOnly? earliestDate, 
            [FromQuery, DataType(DataType.Time)] DateOnly? latestDate)
        {
            try
            {
                var shifts =
                    _shiftManagement.GetShifts(earliestDate, latestDate).Select(s => s.ToModel());
                return Ok(shifts);
            }
            catch (HealthStateException)
            {
                return InvalidModuleState();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult<ShiftModel> CreateShift(ShiftCreationContextModel context)
        {
            try
            {
                var type = _shiftManagement.GetShiftType(context.TypeId);
                if (type is null)
                    return UnprocessableEntity($"No {nameof(ShiftType)} could be found for provided {nameof(context.TypeId)}");

                var result = _shiftManagement.CreateShift(context.FromModel(type)).ToModel();
                return Ok(result);
            }
            catch (HealthStateException)
            {
                return InvalidModuleState();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public ActionResult UpdateShift(ShiftModel model)
        {
            try
            {
                var type = _shiftManagement.GetShiftType(model.TypeId);
                if (type is null)
                    return UnprocessableEntity($"No {nameof(ShiftType)} could be found for provided {nameof(model.TypeId)}");

                _shiftManagement.UpdateShift(model.FromModel(type));
                return NoContent();
            }
            catch (HealthStateException)
            {
                return InvalidModuleState();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteShift(long id)
        {
            try
            {
                _shiftManagement.DeleteShift(id);
                return NoContent();
            }
            catch (HealthStateException)
            {
                return InvalidModuleState();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        #endregion

        #region ShiftTypes

        [HttpGet("types")]
        public ActionResult<IReadOnlyList<ShiftTypeModel>> GetShiftTypes()
        {
            try
            {
                var types = _shiftManagement.ShiftTypes.Select(t => t.ToModel());
                return base.Ok(types);
            }
            catch (HealthStateException)
            {
                return InvalidModuleState();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("types")]
        public ActionResult<ShiftTypeModel> CreateShiftType(ShiftTypeCreationContextModel context)
        {
            try
            {
                if (context.Name is null)
                    return UnprocessableEntity($"{nameof(context.Name)} must not be null");

                var result = _shiftManagement.CreateShiftType(context.FromModel()).ToModel();
                return base.Ok(result);
            }
            catch (HealthStateException)
            {
                return InvalidModuleState();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("types")]
        public ActionResult UpdateShiftType(ShiftTypeModel shiftType)
        {
            try
            {
                _shiftManagement.UpdateShiftType(shiftType.FromModel());
                return NoContent();
            }
            catch (HealthStateException)
            {
                return InvalidModuleState();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("types/{id}")]
        public ActionResult DeleteShiftType(long id)
        {
            try
            {
                _shiftManagement.DeleteShiftType(id);
                return NoContent();
            }
            catch (HealthStateException)
            {
                return InvalidModuleState();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        #endregion

        #region ShiftAssignements

        [HttpGet("assignements")]
        public ActionResult<IReadOnlyList<ShiftAssignementModel>> GetShiftAssignements()
        {
            try
            {
                var assignements = _shiftManagement.ShiftAssignements.Select(a => a.ToModel());
                return Ok(assignements);
            }
            catch (HealthStateException)
            {
                return InvalidModuleState();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("assignements/filter")]
        public ActionResult<IReadOnlyList<ShiftAssignementModel>> GetShiftAssignements([FromQuery, DataType(DataType.Time)] DateOnly? earliestDate, 
            [FromQuery, DataType(DataType.Time)] DateOnly? latestDate)
        {
            try
            {
                var assignements =
                    _shiftManagement.GetShiftAssignements(earliestDate, latestDate).Select(a => a.ToModel());
                return Ok(assignements);
            }
            catch (HealthStateException)
            {
                return InvalidModuleState();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("assignements")]
        public ActionResult<ShiftAssignementModel> CreateShiftAssignement(ShiftAssignementCreationContextModel context)
        {
            try
            {
                var shift = _shiftManagement.GetShift(context.ShiftId);
                if (shift is null)
                    return UnprocessableEntity(string.Format(Strings.ID_NOT_FOUND, nameof(Shift), nameof(context.ShiftId)));

                var resource = _resourceManagement.GetResource<IResource>(context.ResourceId);
                if (resource is null)
                    return UnprocessableEntity(string.Format(Strings.ID_NOT_FOUND, nameof(IResource), nameof(context.ResourceId)));

                if (context.OperatorIdentifier is null)
                    return UnprocessableEntity(string.Format(Strings.NOT_NULL, nameof(context.OperatorIdentifier)));

                var @operator = _operatorManagement.Operators.FirstOrDefault(x => x.Identifier == context.OperatorIdentifier);
                if (@operator is null)
                    return UnprocessableEntity(string.Format(Strings.ID_NOT_FOUND, nameof(Operator), nameof(context.OperatorIdentifier)));

                var result = _shiftManagement.CreateShiftAssignement(context.FromModel(shift, resource, @operator)).ToModel();
                return Ok(result);
            }
            catch (HealthStateException)
            {
                return InvalidModuleState();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("assignements")]
        public ActionResult UpdateShiftAssignement(ShiftAssignementModel shiftAssignement)
        {
            try
            {
                var shift = _shiftManagement.GetShift(shiftAssignement.ShiftId);
                if (shift is null)
                    return UnprocessableEntity(string.Format(Strings.ID_NOT_FOUND, nameof(Shift), nameof(shiftAssignement.ShiftId)));

                var resource = _resourceManagement.GetResource<IResource>(shiftAssignement.ResourceId);
                if (resource is null)
                    return UnprocessableEntity(string.Format(Strings.ID_NOT_FOUND, nameof(IResource), nameof(shiftAssignement.ResourceId)));

                if (shiftAssignement.OperatorIdentifier is null)
                    return UnprocessableEntity(string.Format(Strings.NOT_NULL, nameof(shiftAssignement.OperatorIdentifier)));

                var @operator = _operatorManagement.Operators.FirstOrDefault(x => x.Identifier == shiftAssignement.OperatorIdentifier);
                if (@operator is null)
                    return UnprocessableEntity(string.Format(Strings.ID_NOT_FOUND, nameof(Operator), nameof(shiftAssignement.OperatorIdentifier)));

                _shiftManagement.UpdateShiftAssignement(shiftAssignement.FromModel(shift, resource, @operator));
                return NoContent();
            }
            catch (HealthStateException)
            {
                return InvalidModuleState();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete("assignements/{id}")]
        public ActionResult DeleteShiftAssignement(long id)
        {
            try
            {
                _shiftManagement.DeleteShiftAssignement(id);
                return NoContent();
            }
            catch (HealthStateException)
            {
                return InvalidModuleState();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        #endregion

        private ActionResult InvalidModuleState() => StatusCode(StatusCodes.Status500InternalServerError,
                "The application was not ready to handle the request. Please check the server status and try again.");
    }
}
