namespace Marvin.AbstractionLayer.Drivers.DigitalIO
{
    /// <summary>
    /// API for outputs
    /// </summary>
    public interface IDigitalOutputs
    {
        /// <summary>
        /// Read or write the value at his numerical index
        /// </summary>
        bool this[int index] { get; set; }

        /// <summary>
        /// Read or write the value at this named index
        /// </summary>
        bool this[string name] { get; set; }
    }
}