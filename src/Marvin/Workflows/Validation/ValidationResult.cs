namespace Marvin.Workflows.Validation
{
    /// <summary>
    /// Result of the workflow validation
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Indicator wether validation was a success
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Errors found during validation
        /// </summary>
        public ValidationError[] Errors { get; set; }
    }

    /// <summary>
    /// An error that was detected during validation
    /// </summary>
    public abstract class ValidationError
    {
        /// <summary>
        /// Create error instance with position id
        /// </summary>
        /// <param name="positionId"></param>
        protected ValidationError(long positionId)
        {
            PositionId = positionId;
        }

        /// <summary>
        /// Position where the error was detected. May be a place or transition
        /// </summary>
        public long PositionId { get; set; }

        /// <summary>
        /// Print error in readable format
        /// </summary>
        // TODO: Does it make sense to pass workplan to error
        public abstract string Print(IWorkplan workplan);
    }
}