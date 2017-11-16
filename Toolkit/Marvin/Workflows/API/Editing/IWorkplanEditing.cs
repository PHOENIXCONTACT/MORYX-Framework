using System;

namespace Marvin.Workflows
{
    /// <summary>
    /// Interface for all workplan import/export methods
    /// </summary>
    public interface IWorkplanEditing
    {
        /// <summary>
        /// Declare all types of workplan steps that may be used
        /// </summary>
        /// <param name="stepTypes">Available steps</param>
        void SetAvailableTypes(params Type[] stepTypes);

        /// <summary>
        /// Export the session object
        /// </summary>
        WorkplanEditingSession ExportSession();

        /// <summary>
        /// Add a step to the workplan
        /// </summary>
        SessionModification AddStep(WorkplanStepRecipe recipe);

        /// <summary>
        /// Add special connector to the workplan
        /// </summary>
        SessionModification AddConnector(ConnectorModel connector);

        /// <summary>
        /// Update the properties of a step
        /// </summary>
        SessionModification UpdateStep(WorkplanStepModel stepModel);

        /// <summary>
        /// Remove step from workplan
        /// </summary>
        SessionModification RemoveStep(long stepId);

        /// <summary>
        /// Remove special connector and all references to it from the model
        /// </summary>
        SessionModification RemoveConnector(long connectorId);

        /// <summary>
        /// Connect two steps
        /// </summary>
        SessionModification Connect(ConnectionPoint source, ConnectionPoint target);

        /// <summary>
        /// Disconnect two steps
        /// </summary>
        SessionModification Disconnect(ConnectionPoint source, ConnectionPoint target);

        /// <summary>
        /// Undo the last operation
        /// </summary>
        /// <returns></returns>
        SessionModification Undo();

        /// <summary>
        /// Redo a previously undone operation
        /// </summary>
        /// <returns></returns>
        SessionModification Redo();

        /// <summary>
        /// Finish editing and return the modified workplan instance
        /// </summary>
        /// <returns>Modified workplan instance</returns>
        Workplan Finish();

        /// <summary>
        /// Finish editing and return the modified workplan instance after transfering
        /// values from the client session.
        /// </summary>
        /// <returns>Modified workplan instance</returns>
        Workplan Finish(WorkplanEditingSession session);
    }
}