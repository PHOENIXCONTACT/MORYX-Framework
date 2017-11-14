namespace Marvin.Workflows
{
    /// <summary>
    /// Enum that represents the different actions of the user
    /// </summary>
    public enum UserOperation
    {
        /// <summary>
        /// Add a step to the workplan
        /// </summary>
        AddStep,
        /// <summary>
        /// Update a step
        /// </summary>
        UpdateStep,
        /// <summary>
        /// Remove a step
        /// </summary>
        RemoveStep,

        /// <summary>
        /// Add a connector
        /// </summary>
        AddConnector,
        /// <summary>
        /// Remove a connector
        /// </summary>
        RemoveConnector,

        /// <summary>
        /// Connect two steps or a step and a connector
        /// </summary>
        Connect,
        /// <summary>
        /// Remove a connection
        /// </summary>
        Disconnect,

        /// <summary>
        /// User undid a previous operation
        /// </summary>
        Undo,
        /// <summary>
        /// User redid an undone operation
        /// </summary>
        Redo
    }
}