using System;

namespace Marvin.Runtime.Tasks
{
    /// <summary>
    /// Interface for task scheduling
    /// </summary>
    public interface ITaskSchedule
    {
        /// <summary>
        /// Flag if this scheduled task is still executable or should be disposed
        /// </summary>
        bool Executable { get; }

        /// <summary>
        /// Get the next time this task shall be run
        /// </summary>
        DateTime StartDate { get; }

        /// <summary>
        /// Get the due date for the next execution
        /// </summary>
        DateTime DueDate { get; }

        /// <summary>
        /// Scheduled task was run. New execution times shall be calculated
        /// </summary>
        void TaskRun(DateTime executionTime);
    }
}