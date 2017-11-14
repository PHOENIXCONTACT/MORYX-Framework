namespace Marvin.Workflows
{
    /// <summary>
    /// Additional facade interface for components that store and provide workplans
    /// </summary>
    public interface IWorkplans
    {
        /// <summary>
        /// Saves workplan and return database id
        /// </summary>
        /// <param name="workplan">Workplan instance</param>
        /// <returns>Database id of workplan</returns>
        long SaveWorkplan(Workplan workplan);

        /// <summary>
        /// Loads the workplan
        /// </summary>
        /// <param name="workplanId">Workplan id</param>
        /// <returns>Instance of workplan</returns>
        Workplan LoadWorkplan(long workplanId);

        /// <summary>
        /// Deletes workplan if in new state
        /// </summary>
        /// <param name="workplanId">Workplan id</param>
        void DeleteWorkplan(long workplanId);
    }
}