namespace Marvin.Communication
{
    /// <summary>
    /// Interaface for binary messages
    /// </summary>
    public class BinaryMessage
    {
        /// <summary>
        /// Empty payload to optimize memory usage and avoid creating unnecessary objects
        /// </summary>
        public static byte[] EmptyBytes = new byte[0];

        /// <summary>
        /// Message payload
        /// </summary>
        public byte[] Payload { get; set; }
    }

    /// <summary>
    /// Interaface for binary messages with generic header
    /// </summary>
    public class BinaryMessage<THeader> : BinaryMessage
        where THeader : IBinaryHeader, new()
    {
        /// <summary>
        /// Header of this message
        /// </summary>
        public THeader Header { get; set; }
    }
}
