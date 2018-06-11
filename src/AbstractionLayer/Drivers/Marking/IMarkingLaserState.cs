namespace Marvin.AbstractionLayer.Drivers.Marking
{
    /// <summary>
    /// Specialization of the <see cref="IDriverState"/> 
    /// </summary>
    /// <seealso cref="IDriverState" />
    public interface IMarkingLaserState : IDriverState
    {
        /// <summary>
        /// Determines the special status of the laser. 
        /// The laser can be ready to work (move axis or set marking file) but not ready for marking
        /// Some errors can occure at the laser. If this property is <c>true</c> the laser is ready for marking requests
        /// </summary>
        bool IsReadyForMarking { get; }
    }
}
