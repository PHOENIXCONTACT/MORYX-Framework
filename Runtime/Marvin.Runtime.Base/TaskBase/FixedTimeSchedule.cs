using System;
using Marvin.Runtime.Tasks;

namespace Marvin.Runtime.Base.TaskBase
{
    /// <summary>
    /// Schedule for Cron executions
    /// </summary>
    public class FixedTimeSchedule : ITaskSchedule
    {
        private readonly TimeSpan _shift;

        /// <summary>
        /// Create cron schedule that will execute the task at this time every hour
        /// </summary>
        public FixedTimeSchedule(int minute)
        {
            var n = DateTime.Now;
            StartDate = n.Minute > minute ? new DateTime(n.Year, n.Month, n.Day, n.Hour + 1, minute, 0)
                                          : new DateTime(n.Year, n.Month, n.Day, n.Hour, minute, 0);
            _shift = TimeSpan.FromHours(1);
        }

        /// <summary>
        /// Create a CronSchedule that will execute the task at this time every day
        /// </summary>
        public FixedTimeSchedule(int hour, int minute) : this(hour, minute, 0)
        {
        }

        /// <summary>
        /// Create a CronSchedule that will execute the task at this time every day
        /// </summary>
        public FixedTimeSchedule(int hour, int minute, int seconds)
        {
            var n = DateTime.Now;
            StartDate = n.Hour > hour && n.Minute > minute ? new DateTime(n.Year, n.Month, n.Day + 1, hour, minute, seconds)
                                                           : new DateTime(n.Year, n.Month, n.Day, hour, minute, seconds);
            _shift = TimeSpan.FromDays(1);
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
        public DateTime DueDate { get { return StartDate.AddSeconds(10); } }

        /// <summary>
        /// Scheduled task was run. New execution times shall be calculated
        /// </summary>
        public void TaskRun(DateTime executionTime)
        {
            StartDate = StartDate.Add(_shift);
        }
    }
}