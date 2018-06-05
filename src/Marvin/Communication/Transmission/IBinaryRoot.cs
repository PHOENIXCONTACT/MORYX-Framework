namespace Marvin.Communication
{
    /// <summary>
    /// Interface for root classes of message definitions
    /// </summary>
    public interface IBinaryRoot<THeader> : IByteSerializable, IQuickCast
        where THeader : IBinaryHeader
    {
        /// <summary>
        /// Generate header for this message
        /// </summary>
        THeader Header { get; set; }
    }
}