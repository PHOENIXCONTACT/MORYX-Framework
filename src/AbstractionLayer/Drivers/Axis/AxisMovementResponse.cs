namespace Marvin.AbstractionLayer.Drivers.Axis
{
    /// <summary>
    /// Response of the marking file setup
    /// </summary>
    public class AxisMovementResponse : TransmissionResult
    {
        /// <summary>
        /// Successfull movement of the axis
        /// </summary>
        public AxisMovementResponse() : base()
        {
            
        }

        /// <summary>
        /// Faulty movement of the axis
        /// </summary>
        /// <param name="errorMessage">Occured error</param>
        public AxisMovementResponse(string errorMessage) : base(new TransmissionError(errorMessage))
        {
            
        }
    }
}