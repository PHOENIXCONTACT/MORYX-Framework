using Marvin.Logging;

namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// Host factory to create dependency injection enabled web services
    /// </summary>
    public interface IWcfHostFactory
    {
        /// <summary>
        /// Create host instance for a wcf service
        /// </summary>
        IConfiguredServiceHost CreateHost<T>(HostConfig config, ITypedHostFactory hostFactory, IModuleLogger logger);
    }
}
