// Copyright (c) 2022, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moryx.Workplans;
using System.Collections.Generic;

namespace Moryx.AbstractionLayer.Products.Endpoints
{
    /// <summary>
    /// Definition of a REST API on the <see cref="IWorkplansVersions"/> facade.
    /// </summary>
    [ApiController]
    [Route("api/moryx/workplans/")]
    [Produces("application/json")]
    public class WorkplanController : ControllerBase
    {
        private readonly IWorkplans _workplansVersions;
        public WorkplanController(IWorkplans workplansVersions)
            => _workplansVersions = workplansVersions;

        [HttpGet]
        public WorkplanModel[] GetAllWorkplans()
        {
            var workplans = new List<WorkplanModel>();
            foreach (var w in _workplansVersions.LoadAllWorkplans())
                workplans.Add(ProductConverter.ConvertWorkplan(w));
            return workplans.ToArray();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<long> SaveWorkplan(WorkplanModel model)
        {
            if (model == null)
                return BadRequest($"Model was null");
            var workplan = ProductConverter.ConvertWorkplanBack(model);
            return _workplansVersions.SaveWorkplan(workplan);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{id}/versions")]
        public ActionResult<WorkplanModel[]> GetVersions(long id)
        {
            if (id == 0)
                return BadRequest($"Workplan id was 0");
            var versions = _workplansVersions.LoadVersions(id);
            if (versions == null)
                return NotFound();
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
        public ActionResult<WorkplanModel> GetWorkplan(long id)
        {
            if (id == 0)
                return BadRequest($"Workplan id was 0");
            var workplan = _workplansVersions.LoadWorkplan(id);
            if (workplan == null)
                return NotFound();
            return ProductConverter.ConvertWorkplan(workplan);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{id}")]
        public ActionResult DeleteWorkplan(long id)
        {
            if (_workplansVersions.LoadWorkplan(id) == null)
                return NotFound();
            _workplansVersions.DeleteWorkplan(id);
            return Ok();
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{id}")]
        public ActionResult<long> UpdateWorkplan(WorkplanModel model)
        {
            if(model == null)
                return BadRequest($"Workplan id was 0");
            if(_workplansVersions.LoadWorkplan(model.Id) == null)
                return BadRequest($"Workplan with id {model.Id} does not exist");
            return _workplansVersions.SaveWorkplan(ProductConverter.ConvertWorkplanBack(model));
        }

    }
}
