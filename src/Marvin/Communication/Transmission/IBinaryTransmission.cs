using System;

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
        /// <param name="data">The data.</param>
        void Send(BinaryMessage data);

        /// <summary>
        /// Event that will be triggered when new data was received
        /// 
        /// Beware that some devices like the AsyncTCPClient immediatly start 
        /// processing buffered bytes while adding this handler.
        /// </summary>
        event EventHandler<BinaryMessage> Received;
    }
}