namespace Marvin.AbstractionLayer.Drivers.Rfid
{
    /// <summary>
    /// Directed rfid tag event
    /// </summary>
    public class DirectedRfidTag : RfidTag
    {
        /// <summary>
        /// Movement direction of the rfid tag
        /// </summary>
        public RfidDirection Direction { get; set; }
    }
}