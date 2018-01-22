namespace Marvin.AbstractionLayer.Drivers.Marking
{
    /// <summary>
    /// Exception if the marking driver does not support segments
    /// </summary>
    public class SegmentsNotSupportedException : MarvinException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentsNotSupportedException"/> class.
        /// </summary>
        public SegmentsNotSupportedException() : base("The driver does not support segments")
        {
            
        } 
    }
}