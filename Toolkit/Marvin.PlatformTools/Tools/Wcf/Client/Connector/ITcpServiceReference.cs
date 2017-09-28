using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// Client Side Definition of Marvin.Base.Proxies.Wcf.IBaseWebSvc
    /// inteface service contract.
    /// </summary>
    public interface ITcpServiceReference
    {
        /// <summary>
        /// Subscribes the specified client identifier to the server.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        void Subscribe(string clientId);

        /// <summary>
        /// Subscribes the specified client identifier to the server.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        Task SubscribeAsync(string clientId);

        /// <summary>
        /// Resubscribes the specified client identifier to the server.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        void ReSubscribe(string clientId);

        /// <summary>
        /// Resubscribes the specified client identifier to the server.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        Task ReSubscribeAsync(string clientId);

        /// <summary>
        /// Heartbeat call to the server side.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        void Heartbeat(string clientId);

        /// <summary>
        /// Heartbeat call to the server side.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        Task HeartbeatAsync(string clientId);

        /// <summary>
        /// Unsubscribe client from server.
        /// </summary>
        void UnSubscribe();
    }
}
