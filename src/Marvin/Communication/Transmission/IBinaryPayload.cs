namespace Marvin.Communication
{
    /// <summary>
    /// Interface for all classes used as DTO for binary communication
    /// </summary>
    public interface IBinaryPayload : IByteSerializable
    {
        /// <summary>
        /// Serialize this object into an existing byte array
        /// </summary>
        byte[] ToBytes(byte[] bytes, ref int index);

        /// <summary>
        /// Read object from byte stream and move the pointer
        /// Returns itself for method chaining
        /// </summary>
        IBinaryPayload FromBytes(byte[] bytes, ref int index);
    }
}