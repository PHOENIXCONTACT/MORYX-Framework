namespace Marvin.Workflows
{
    /// <summary>
    /// Strategy to fetch referenced sub workplans within the engine on the fly
    /// </summary>
    public interface IWorkplanSource
    {
        /// <summary>
        /// Load a workplan instance into the editing session
        /// </summary>
        /// <param name="id">Unique id of the workplan</param>
        /// <returns>Workplan instance</returns>
        IWorkplan Load(long id);
    }
}