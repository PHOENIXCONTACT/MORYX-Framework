namespace Marvin.AbstractionLayer.Drivers
{
    /// <summary>
    /// Generic base delegate for driver APIs with callback
    /// </summary>
    public delegate void DriverResponse<in TResult>(IDriver sender, TResult result) where TResult : TransmissionResult;

    /// <summary>
    /// Base result object for transissions
    /// </summary>
    public abstract class TransmissionResult
    {
        /// <summary>
        /// Create success message
        /// </summary>
        protected TransmissionResult()
        {
            IsSuccess = true;
        }

        /// <summary>
        /// Create failure message
        /// </summary>
        /// <param name="error">Error that caused the failure</param>
        protected TransmissionResult(TransmissionError error)
        {
            IsSuccess = false;
            Error = error;
        }

        /// <summary>
        /// Flag if this transmission was a success
        /// </summary>
        public bool IsSuccess { get; private set; }

        /// <summary>
        /// Error that occured
        /// </summary>
        public TransmissionError Error { get; set; }
    }

    /// <summary>
    /// Error wrapper class
    /// </summary>
    public class TransmissionError
    {
        /// <summary>
        /// Create error from error message
        /// </summary>
        public TransmissionError(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Human readable error message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0}", Message);
        }
    }
}