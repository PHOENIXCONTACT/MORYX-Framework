using System;
using Marvin.Runtime.Tasks;

namespace Marvin.Runtime.Base.TaskBase
{
    /// <summary>
    /// Task schedule for interval execution
    /// </summary>
    public class IntervalExecutionSchedule : ITaskSchedule
    {
        private readonly TimeSpan _interval;

        /// <summary>
        /// Create interval execution with immediate start
        /// </summary>
        public IntervalExecutionSchedule(TimeSpan interval) 
            : this(interval, DateTime.Now)
        {
        }

        /// <summary>
        /// Create interval execution with configured start
        /// </summary>
        public IntervalExecutionSchedule(TimeSpan interval, DateTime startTime) 
            : this(interval, startTime, startTime)
        {
        }

        /// <summary>
        /// Create interval execution with configured timeslot
        /// </summary>
        public IntervalExecutionSchedule(TimeSpan interval, DateTime startTime, DateTime dueDate)
        {
            _interval = interval;
            StartDate = startTime;
            DueDate = dueDate > startTime ? dueDate : startTime;
        }

        /// <summary>
        /// Flag if this scheduled task is still executable or should be disposed
        /// </summary>
        public bool Executable { get { return true; } }

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
            // Task was run in time slot so we remain the original schedule
            if (StartDate < executionTime && executionTime < DueDate)
            {
                StartDate += _interval;
                DueDate += _interval;
            }
            // Move timeslot according to last execution
            else
            {
                var timeShift = TimeSpan.FromSeconds((DueDate - StartDate).TotalSeconds / 2);
                StartDate = executionTime + _interval - timeShift;
                DueDate = executionTime + _interval + timeShift;
            }
        }
    }
}