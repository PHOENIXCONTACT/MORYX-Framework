namespace Marvin.Communication
{
    /// <summary>
    /// Base class for binary headers
    /// </summary>
    public interface IBinaryHeader : IByteSerializable
    {
        /// <summary>
        /// Length of the payload
        /// </summary>
        int PayloadLength { get; set; }
    }
}
