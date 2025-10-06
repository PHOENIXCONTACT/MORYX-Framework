// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moryx.AbstractionLayer.Products.Endpoints;
using Moryx.Asp.Extensions;
using Moryx.Serialization;
using Moryx.Workplans.Web.Properties;


namespace Moryx.Workplans.Endpoint
{
    [Route("api/moryx/workplans/")]
    [ApiController, Produces("application/json")]
    public class WorkplanEditingController : ControllerBase
    {
        private readonly IWorkplans _workplans;
        private readonly IWorkplanEditing _workplanEditing;

        public WorkplanEditingController(IWorkplans workplans, IWorkplanEditing workplanEditing)
        {
            _workplans = workplans;
            _workplanEditing = workplanEditing;
        }

        [HttpGet("steps")]
        public ActionResult<WorkplanStepRecipe[]> AvailableSteps()
        {
            var workplans = _workplans.LoadAllWorkplans();

            var recipeSteps = _workplanEditing.AvailableSteps
              .Where(step => ModelConverter.ToClassification(step) != WorkplanNodeClassification.Subworkplan)
              .Select(step => WorkplanStepRecipe.FromStepType(step));

            var subWorkplans = _workplanEditing.AvailableSteps
              .Where(step => ModelConverter.ToClassification(step) == WorkplanNodeClassification.Subworkplan)
              .SelectMany(step =>
              {
                  var stepRecipe = WorkplanStepRecipe.FromStepType(step);
                  return workplans.Select(workplan => new WorkplanStepRecipe
                  {
                      SubworkplanId = workplan.Id,
                      Type = stepRecipe.Type,
                      Classification = WorkplanNodeClassification.Subworkplan,
                      Name = workplan.Name
                  });
              });

            return recipeSteps.Concat(subWorkplans).ToArray();
        }

        [HttpPost("sessions")]
        [Authorize(Policy = WorkplanPermissions.CanEdit)]
        public ActionResult<WorkplanSessionModel> EditWorkplan([FromBody] OpenSessionRequest openSession)
        {
            var workplan = openSession.WorkplanId > 0
                ? _workplans.LoadWorkplan(openSession.WorkplanId)
                : CreateNew();
            if (workplan == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.SESSION_NOT_FOUND });

            var session = _workplanEditing.EditWorkplan(workplan, openSession.Duplicate);
            return ModelConverter.ConvertSession(session);
        }

        private Workplan CreateNew()
        {
            var workplan = new Workplan { Name = "New workplan", State = WorkplanState.New };
            workplan.Add(new Connector { Name = "Start", Classification = NodeClassification.Start, Position = new Point(200, 100) });
            workplan.Add(new Connector { Name = "End", Classification = NodeClassification.End, Position = new Point(100, 500) });
            workplan.Add(new Connector { Name = "Failed", Classification = NodeClassification.Failed, Position = new Point(300, 500) });
            return workplan;
        }

        [HttpGet("sessions/{sessionId}")]
        [Authorize(Policy = WorkplanPermissions.CanView)]
        public ActionResult<WorkplanSessionModel> OpenSession([FromRoute] string sessionId)
        {
            var session = _workplanEditing.OpenSession(sessionId);
            if (session == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.SESSION_NOT_FOUND });
            return ModelConverter.ConvertSession(session);
        }

        [HttpGet("sessions/{sessionId}/autolayout")]
        [Authorize(Policy = WorkplanPermissions.CanView)]
        public ActionResult<WorkplanSessionModel> AutoLayout([FromRoute] string sessionId)
        {
            var session = _workplanEditing.OpenSession(sessionId);
            if (session == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.SESSION_NOT_FOUND });

            _workplanEditing.AutoLayout(sessionId);
            return ModelConverter.ConvertSession(session);
        }

        [HttpPut("sessions/{sessionId}")]
        [Authorize(Policy = WorkplanPermissions.CanEdit)]
        public ActionResult<WorkplanSessionModel> UpdateSession(
            [FromRoute] string sessionId,
            [FromBody] WorkplanSessionModel sessionModel
        )
        {
            var session = _workplanEditing.OpenSession(sessionId);
            UpdateSession(sessionModel, session);
            return ModelConverter.ConvertSession(session);
        }

        private static void UpdateSession(WorkplanSessionModel sessionModel, WorkplanSession session)
        {
            session.Workplan.Name = sessionModel.Name;
            session.Workplan.State = sessionModel.State;
            foreach (var node in sessionModel.Nodes)
            {
                var workplanNode = (IWorkplanNode)session.Workplan.Connectors.FirstOrDefault(c => c.Id == node.Id)
                    ?? session.Workplan.Steps.FirstOrDefault(s => s.Id == node.Id);

                if (workplanNode == null)
                    continue;

                workplanNode.Name = node.DisplayName ?? node.Name;
                workplanNode.Position = new Point(node.PositionLeft, node.PositionTop);

                if (node.Properties == null)
                    continue;

                EntryConvert.UpdateInstance(workplanNode, node.Properties);
            }
        }

        [HttpPost("sessions/{sessionId}/save")]
        [Authorize(Policy = WorkplanPermissions.CanEdit)]
        public ActionResult<WorkplanSessionModel> SaveSession(
            [FromRoute] string sessionId,
            [FromBody] WorkplanSessionModel sessionModel)
        {
            var session = _workplanEditing.OpenSession(sessionId);
            UpdateSession(sessionModel, session);
            _workplans.SaveWorkplan(session.Workplan);
            return ModelConverter.ConvertSession(session);
        }

        [HttpDelete("sessions/{sessionId}")]
        [Authorize(Policy = WorkplanPermissions.CanView)]
        public ActionResult CloseSession([FromRoute] string sessionId)
        {
            _workplanEditing.CloseSession(sessionId);
            return Ok();
        }

        [HttpPost("sessions/{sessionId}/nodes")]
        [Authorize(Policy = WorkplanPermissions.CanEdit)]
        public ActionResult<WorkplanNodeModel> AddStep(
            [FromRoute] string sessionId,
            [FromBody] WorkplanStepRecipe recipe
        )
        {
            var stepType = _workplanEditing.AvailableSteps.FirstOrDefault(s => s.FullName == recipe.Type);
            IWorkplanStep step;

            if (recipe.Classification == WorkplanNodeClassification.Subworkplan)
            {
                var workplan = _workplans.LoadWorkplan(recipe.SubworkplanId);
                step = (IWorkplanStep)Activator.CreateInstance(stepType, workplan);
            }
            else if (recipe.Constructor == null)
                step = (IWorkplanStep)Activator.CreateInstance(stepType);
            else
                step = (IWorkplanStep)EntryConvert.CreateInstance(stepType, recipe.Constructor);

            step.Position = new Point(recipe.PositionLeft, recipe.PositionTop);

            try
            {
                _workplanEditing.AddStep(sessionId, step);
            }
            catch (Exception e)
            {
                return BadRequest(new MoryxExceptionResponse { Title = e.Message });
            }

            return ModelConverter.ConvertWorkplanStep(step, new List<IWorkplanStep>());
        }

        [HttpPut("sessions/{sessionId}/nodes/{nodeId}")]
        [Authorize(Policy = WorkplanPermissions.CanEdit)]
        public ActionResult<WorkplanNodeModel> UpdateStep(
            [FromRoute] string sessionId,
            [FromRoute] long nodeId,
            [FromBody] WorkplanNodeModel stepModel)
        {
            var session = _workplanEditing.OpenSession(sessionId);
            var step = session.Workplan.Steps.FirstOrDefault(s => s.Id == nodeId);
            if (step == null)
                return stepModel;

            if (stepModel.Properties != null)
                EntryConvert.UpdateInstance(step, stepModel.Properties);

            step.Name = stepModel.DisplayName;
            return ModelConverter.ConvertWorkplanStep(step, session.Workplan.Steps.ToList());
        }

        [HttpDelete("sessions/{sessionId}/nodes/{nodeId}")]
        [Authorize(Policy = WorkplanPermissions.CanEdit)]
        public ActionResult<WorkplanSessionModel> RemoveNode(
            [FromRoute] string sessionId,
            [FromRoute] long nodeId
        )
        {
            var session = _workplanEditing.OpenSession(sessionId);
            _workplanEditing.RemoveStep(sessionId, nodeId);
            return ModelConverter.ConvertSession(session);
        }

        [HttpPost("sessions/{sessionId}/nodes/{targetNodeId}/{targetIndex}")]
        [Authorize(Policy = WorkplanPermissions.CanEdit)]
        public ActionResult<WorkplanSessionModel> ConnectStep(
            [FromRoute] string sessionId,
            [FromRoute] long targetNodeId,
            [FromRoute] int targetIndex,
            [FromBody] NodeConnector source)
        {
            var session = _workplanEditing.OpenSession(sessionId);
            var sourceNode = (IWorkplanNode)session.Workplan.Connectors.FirstOrDefault(c => c.Id == source.NodeId)
                ?? session.Workplan.Steps.FirstOrDefault(s => s.Id == source.NodeId);
            var targetNode = (IWorkplanNode)session.Workplan.Connectors.FirstOrDefault(c => c.Id == targetNodeId)
                ?? session.Workplan.Steps.FirstOrDefault(s => s.Id == targetNodeId);
            _workplanEditing.Connect(sessionId, sourceNode, source.Index, targetNode, targetIndex);
            return ModelConverter.ConvertSession(session);
        }

        [HttpDelete("sessions/{sessionId}/nodes/{targetNodeId}/{targetIndex}")]
        [Authorize(Policy = WorkplanPermissions.CanEdit)]
        public ActionResult<WorkplanSessionModel> DisconnectStep(
            [FromRoute] string sessionId,
            [FromRoute] long targetNodeId,
            [FromRoute] int targetIndex,
            [FromBody] NodeConnector source)
        {
            var session = _workplanEditing.OpenSession(sessionId);
            var sourceNode = (IWorkplanNode)session.Workplan.Connectors.FirstOrDefault(c => c.Id == source.NodeId)
            ?? session.Workplan.Steps.FirstOrDefault(s => s.Id == source.NodeId);
            var targetNode = (IWorkplanNode)session.Workplan.Connectors.FirstOrDefault(c => c.Id == targetNodeId)
            ?? session.Workplan.Steps.FirstOrDefault(s => s.Id == targetNodeId);
            _workplanEditing.Disconnect(sessionId, sourceNode, source.Index, targetNode, targetIndex);
            return ModelConverter.ConvertSession(session);
        }
    }
}

