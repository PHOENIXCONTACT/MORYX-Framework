using System.ServiceModel;

namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// Host factory interface for container independency
    /// </summary>
    public interface ITypedHostFactory
    {
        /// <summary>
        /// Create a wcf service host for a specific service
        /// </summary>
        /// <typeparam name="T">Service contract interface</typeparam>
        /// <returns>Host instance for endpoint configuration</returns>
        ServiceHost CreateServiceHost<T>();
    }
}
