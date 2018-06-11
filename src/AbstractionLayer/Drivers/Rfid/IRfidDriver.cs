using System;

namespace Marvin.AbstractionLayer.Drivers.Rfid
{
    /// <summary>
    /// Driver API of the sich driver
    /// </summary>
    public interface IRfidDriver : IDriver
    {
        /// <summary>
        /// Event raised when tags are read by the antenna
        /// </summary>
        event EventHandler<RfidTag[]> TagsRead;
    }
}