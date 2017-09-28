namespace Marvin.Communication
{
    /// <summary>
    /// Interface for all objects that can be directly serialized
    /// </summary>
    public interface IByteSerializable
    {
        /// <summary>
        /// Convert object into byte array
        /// </summary>
        byte[] ToBytes();

        /// <summary>
        /// Read object from bytes
        /// </summary>
        void FromBytes(byte[] bytes);
    }
}