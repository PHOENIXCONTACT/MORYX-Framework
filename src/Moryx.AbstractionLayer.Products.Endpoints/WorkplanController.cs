// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moryx.AbstractionLayer.Products.Endpoints.Models;
using Moryx.AbstractionLayer.Products.Endpoints.Properties;
using Moryx.AspNetCore;
using Moryx.Workplans;

namespace Moryx.AbstractionLayer.Products.Endpoints
{
    /// <summary>
    /// Definition of a REST API on the <see cref="IWorkplans"/> facade.
    /// </summary>
    [ApiController]
    [Route("api/moryx/workplans/")]
    [Produces("application/json")]
    public class WorkplanController : ControllerBase
    {
        private readonly IWorkplans _workplans;
        public WorkplanController(IWorkplans workplans)
        {
            _workplans = workplans;
        }

        [HttpGet]
        [Authorize(Policy = WorkplanPermissions.CanView)]
        public async Task<ActionResult<WorkplanModel[]>> GetAllWorkplans()
        {
            var workplans = new List<WorkplanModel>();
            foreach (var w in await _workplans.LoadAllWorkplansAsync())
                workplans.Add(ProductConverter.ConvertWorkplan(w));
            return workplans.ToArray();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = WorkplanPermissions.CanEdit)]
        public async Task<ActionResult<long>> SaveWorkplan(WorkplanModel model)
        {
            if (model == null)
                return BadRequest($"Model was null");
            var workplan = ProductConverter.ConvertWorkplanBack(model);
            return await _workplans.SaveWorkplanAsync(workplan);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{id}/versions")]
        [Authorize(Policy = WorkplanPermissions.CanView)]
        public async Task<ActionResult<WorkplanModel[]>> GetVersions(long id)
        {
            if (id == 0)
                return BadRequest($"Workplan id was 0");
            var versions = await _workplans.LoadVersionsAsync(id);
            if (versions == null)
                return NotFound(new MoryxExceptionResponse { Title = string.Format(Strings.WorkplanController_WorkplanNotFound, id) });
            var model = new List<WorkplanModel>();
            foreach (var v in versions)
            {
                model.Add(ProductConverter.ConvertWorkplan(v));
            }
            return Ok(model.ToArray());
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{id}")]
        [Authorize(Policy = WorkplanPermissions.CanView)]
        public async Task<ActionResult<WorkplanModel>> GetWorkplan(long id)
        {
            if (id == 0)
                return BadRequest($"Workplan id was 0");
            var workplan = await _workplans.LoadWorkplanAsync(id);
            if (workplan == null)
                return NotFound(new MoryxExceptionResponse { Title = string.Format(Strings.WorkplanController_WorkplanNotFound, id) });
            return ProductConverter.ConvertWorkplan(workplan);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{id}")]
        [Authorize(Policy = WorkplanPermissions.CanDelete)]
        public async Task<ActionResult> DeleteWorkplan(long id)
        {
            var workplan = await _workplans.LoadWorkplanAsync(id);
            if (workplan == null)
                return NotFound();
            await _workplans.DeleteWorkplanAsync(id);
            return Ok();
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{id}")]
        [Authorize(Policy = WorkplanPermissions.CanEdit)]
        public async Task<ActionResult<long>> UpdateWorkplan(WorkplanModel model)
        {
            if (model == null)
                return BadRequest($"Workplan id was 0");

            var workplan = await _workplans.LoadWorkplanAsync(model.Id);

            if (workplan== null)
                return BadRequest($"Workplan with id {model.Id} does not exist");

            return await _workplans.SaveWorkplanAsync(ProductConverter.ConvertWorkplanBack(model));
        }

    }
}
