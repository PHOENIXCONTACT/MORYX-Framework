namespace Marvin.Logging
{
    /// <summary>
    /// This interface allows framework components to access a module logger and log entries in their name
    /// </summary>
    public interface ILoggingComponent
    {
        /// <summary>
        /// Logger of this component
        /// </summary>
        IModuleLogger Logger { get; set; }
    }
}
