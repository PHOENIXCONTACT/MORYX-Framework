using System.Diagnostics;

namespace Moryx.ControlSystem.Jobs
{
    /// <summary>
    /// Position of 
    /// </summary>
    [DebuggerDisplay("JobPosition <Type: {" + nameof(PositionType) + "}, Reference: {" + nameof(ReferenceId) + "}>")]
    public struct JobPosition
    {
        /// <summary>
        /// Create a new job position
        /// </summary>
        public JobPosition(JobPositionType positionType, long? reference)
        {
            PositionType = positionType;
            ReferenceId = reference;
        }

        /// <summary>
        /// Different positions for a job
        /// </summary>
        public JobPositionType PositionType { get; }

        /// <summary>
        /// Reference object the <see cref="PositionType"/> refers to
        /// </summary>
        public long? ReferenceId { get; }

        /// <summary>
        /// Default position
        /// </summary>
        public static JobPosition Append = new JobPosition(JobPositionType.Append, null);

        /// <summary>
        /// Position new jobs around the ones already part of the list
        /// </summary>
        public static JobPosition Expand = new JobPosition(JobPositionType.AroundExisting, null);
    }
}