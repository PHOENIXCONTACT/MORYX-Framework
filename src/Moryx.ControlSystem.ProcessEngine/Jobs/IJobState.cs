using Moryx.ControlSystem.Jobs;

namespace Moryx.ControlSystem.ProcessEngine.Jobs
{
    /// <summary>
    /// Public representation of the job state
    /// </summary>
    internal interface IJobState
    {
        /// <summary>
        /// Key of the job state
        /// </summary>
        int Key { get; }

        /// <summary>
        /// Classification of the job
        /// </summary>
        JobClassification Classification { get; }
    }
}