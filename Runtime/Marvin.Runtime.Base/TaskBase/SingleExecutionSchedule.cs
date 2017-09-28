using System;
using Marvin.Runtime.Tasks;

namespace Marvin.Runtime.Base.TaskBase
{
    /// <summary>
    /// TaskSchedule implementation for single execution
    /// </summary>
    public class SingleExecutionSchedule : ITaskSchedule
    {
        /// <summary>
        /// Schedule an instant single execution
        /// </summary>
        public SingleExecutionSchedule() : this(DateTime.Now)
        {
        }

        /// <summary>
        /// Schedule a single execution at a given time
        /// </summary>
        public SingleExecutionSchedule(DateTime start) : this(start, start)
        {
        }

        /// <summary>
        /// Schedule a single execution within a certain timeslot
        /// </summary>
        public SingleExecutionSchedule(DateTime start, DateTime dueDate)
        {
            Executable = true;
            StartDate = start;
            DueDate = dueDate > start ? dueDate : start;
        }

        /// <summary>
        /// Flag if this scheduled task is still executable or should be disposed
        /// </summary>
        public bool Executable { get; private set; }

        /// <summary>
        /// Get the next time this task shall be run
        /// </summary>
        public DateTime StartDate { get; private set; }

        /// <summary>
        /// Get the due date for the next execution
        /// </summary>
        public DateTime DueDate { get; private set; }

        /// <summary>
        /// Scheduled task was run. New execution times shall be calculated
        /// </summary>
        public void TaskRun(DateTime executionTime)
        {
            Executable = false;
        }
    }
}