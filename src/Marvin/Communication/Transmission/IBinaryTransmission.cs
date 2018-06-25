using System;
using System.Threading.Tasks;

namespace Marvin.Communication
{
    /// <summary>
    /// Interface for duplex binary transmission
    /// </summary>
    public interface IBinaryTransmission
    {
        /// <summary>
        /// Sends the specified data.
        /// </summary>
        void Send(BinaryMessage message);

        /// <summary>
        /// Sends the specified data asynchornous. 
        /// Task is done if data is successfully written to the underlying stream.
        /// </summary>
        Task SendAsync(BinaryMessage message);

        /// <summary>
        /// Event that will be triggered when new data was received
        /// Beware that some devices like the AsyncTCPClient immediatly start 
        /// processing buffered bytes while adding this handler.
        /// </summary>
        event EventHandler<BinaryMessage> Received;
    }
}