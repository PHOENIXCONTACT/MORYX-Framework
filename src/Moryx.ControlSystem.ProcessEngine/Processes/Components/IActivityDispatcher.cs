namespace Moryx.ControlSystem.ProcessEngine.Processes
{
    /// <summary>
    /// Additional interface for the activity dispatcher to access the reported sessions
    /// </summary>
    internal interface IActivityDispatcher
    {
        /// <summary>
        /// Export sessions
        /// </summary>
        ResourceAndSession[] ExportSessions();
    }
}