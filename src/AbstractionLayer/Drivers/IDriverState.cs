namespace Marvin.AbstractionLayer.Drivers
{
    /// <summary>
    /// Base API for driver states
    /// </summary>
    public interface IDriverState
    {
        /// <summary>
        /// Gets the classification of the state.
        /// </summary>
        StateClassification Classification { get; }
    }
}