using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moryx.Workflows;
using System.Collections.Generic;

namespace Moryx.AbstractionLayer.Products.Endpoints
{
    /// Definition of a REST API on the <see cref="IWorkplansVersions"/> facade.
    /// </summary>
    [ApiController]
    [Route("api/moryx/workplansversions/")]
    public class WorkplanVersionsController: ControllerBase
    {
        private readonly IWorkplansVersions _workplansVersions;
        public WorkplanVersionsController(IWorkplansVersions workplansVersions)
            => _workplansVersions = workplansVersions;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("[action]")]
        public ActionResult<IReadOnlyList<Workplan>> GetVersions(long id)
        {
            if (id == 0)
                return BadRequest($"Workplan id was 0");
            var versions = _workplansVersions.LoadVersions(id);
            if (versions == null)
                return NotFound();
            return Ok(versions);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("[action]")]
        public ActionResult<Workplan> GetWorkplan(long id)
        {
            if (id == 0)
                return BadRequest($"Workplan id was 0");
            var workplan = _workplansVersions.LoadWorkplan(id);
            if (workplan == null)
                return NotFound();
            return Ok(workplan);
        }

        [HttpGet]
        [Route("[action]")]
        public IReadOnlyList<Workplan> GetAllWorkplans()
        {
            return _workplansVersions.LoadAllWorkplans();
        }

        [HttpPost]
        [Route("[action]")]
        public void DeleteWorkplan(long workplanId)
        {
            _workplansVersions.DeleteWorkplan(workplanId);
        }
    }
}
