namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// Host factory to create dependency injection enabled web services
    /// </summary>
    public interface IConfiguredHostFactory
    {
        /// <summary>
        /// Create host instance for a wcf service
        /// </summary>
        IConfiguredServiceHost CreateHost<T>(HostConfig config);
    }
}