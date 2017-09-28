using System;
using System.Linq;
using Marvin.Runtime.Tasks;

namespace Marvin.Runtime.Base.TaskBase
{
    /// <summary>
    /// Combines an ammount of scheduled tasks to run them all on the same date.
    /// </summary>
    public class CombinedSchedule : ITaskSchedule
    {
        private readonly ITaskSchedule[] _schedules;
        /// <summary>
        /// Constructor for CombinedSchedule.
        /// </summary>
        /// <param name="schedules">Amount of tasks which should be combined.</param>
        public CombinedSchedule(params ITaskSchedule[] schedules)
        {
            _schedules = schedules;
        }

        /// <summary>
        /// Flag if this scheduled task is still executable or should be disposed
        /// </summary>
        public bool Executable
        {
            get { return _schedules.Any(s => s.Executable); }
        }

        /// <summary>
        /// Get the next time this task shall be run
        /// </summary>
        public DateTime StartDate
        {
            get { return _schedules.Min(s => s.StartDate); }
        }

        /// <summary>
        /// Get the due date for the next execution
        /// </summary>
        public DateTime DueDate
        {
            get { return _schedules.Min(s => s.DueDate); }
        }

        /// <summary>
        /// Scheduled task was run. New execution times shall be calculated
        /// </summary>
        public void TaskRun(DateTime executionTime)
        {
            foreach (var taskSchedule in _schedules)
            {
                taskSchedule.TaskRun(executionTime);
            }
        }
    }
}