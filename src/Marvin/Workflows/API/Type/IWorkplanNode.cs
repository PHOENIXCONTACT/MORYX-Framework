namespace Marvin.Workflows
{
    /// <summary>
    /// Common interface for <see cref="IWorkplanStep"/> and <see cref="IConnector"/>
    /// </summary>
    public interface IWorkplanNode
    {
        /// <summary>
        /// Workplan unique element id of this connector
        /// </summary>
        long Id { get; set; }

        /// <summary>
        /// Transition name
        /// </summary>
        string Name { get; }
    }
}