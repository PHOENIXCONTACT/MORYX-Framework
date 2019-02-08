namespace Marvin.Serialization
{
    /// <summary>
    /// Possible value types
    /// </summary>
    public enum EntryValueType
    {
        /// <summary>
        /// Byte
        /// </summary>
        Byte,
        /// <summary>
        /// Value type Bool.
        /// </summary>
        Boolean,
        /// <summary>
        /// Int16
        /// </summary>
        Int16,
        /// <summary>
        /// Int16
        /// </summary>
        UInt16,
        /// <summary>
        /// Int32
        /// </summary>
        Int32,
        /// <summary>
        /// Int32
        /// </summary>
        UInt32,
        /// <summary>
        /// Int64
        /// </summary>
        Int64,
        /// <summary>
        /// Int64
        /// </summary>
        UInt64,
        /// <summary>
        /// Decimal value 32 bit
        /// </summary>
        Single,
        /// <summary>
        /// Decimal value 64 bit
        /// </summary>
        Double,
        /// <summary>
        /// Value type string.
        /// </summary>
        String,
        /// <summary>
        /// Value type Enum.
        /// </summary>
        Enum,
        /// <summary>
        /// Value type Class.
        /// </summary>
        Class,
        /// <summary>
        /// Value type Collection.
        /// </summary>
        Collection,
        /// <summary>
        /// Retrieving the value caused an exception
        /// </summary>
        Exception,
        /// <summary>
        /// Value type stream
        /// </summary>
        Stream
    }
}