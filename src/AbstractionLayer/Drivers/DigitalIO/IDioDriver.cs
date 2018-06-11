namespace Marvin.AbstractionLayer.Drivers.DigitalIO
{
    /// <summary>
    /// Interface for drivers of digital IO devices
    /// </summary>
    public interface IDioDriver : IDriver
    {
        /// <summary>
        /// Access the input
        /// </summary>
        IDigitalInputs In { get; }

        /// <summary>
        /// Access the output
        /// </summary>
        IDigitalOutputs Out { get; }
        
        /// <summary>
        /// Access to groups
        /// </summary>
        IDioGroup Group { get; }
    }
}