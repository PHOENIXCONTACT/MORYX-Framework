namespace Marvin.AbstractionLayer.Drivers.Marking
{
    /// <summary>
    /// Response of the marking process
    /// </summary>
    public class MarkingResponse : TransmissionResult
    {
        /// <summary>
        /// Successfull marking
        /// </summary>
        public MarkingResponse()
        {
            
        }

        /// <summary>
        /// Faulty marking
        /// </summary>
        /// <param name="errorMessage">Occured error</param>
        public MarkingResponse(string errorMessage) : base(new TransmissionError(errorMessage))
        {
            
        }
    }
}