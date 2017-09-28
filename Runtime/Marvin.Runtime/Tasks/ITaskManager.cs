using System.Collections.Generic;

namespace Marvin.Runtime.Tasks
{
    /// <summary>
    /// Kernel component for periodic or longrunning tasks
    /// </summary>
    public interface ITaskManager
    {
        /// <summary>
        /// Number of available parallel threads
        /// </summary>
        int WorkerThreads { get; }

        /// <summary>
        /// Full task list
        /// </summary>
        IEnumerable<ScheduledTask> Tasks { get; } 

        /// <summary>
        /// Schedule this task for execution
        /// </summary>
        /// <returns>Task id that can be used to remove the task</returns>
        int Schedule(ITask task, ITaskSchedule schedule);
    }

    /// <summary>
    /// POCO class representing a scheduled task
    /// </summary>
    public class ScheduledTask
    {
        /// <summary>
        /// Unique Id of this task
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the task
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Flag if this task is currently running
        /// </summary>
        public bool Running { get; set; }

        /// <summary>
        /// Progress in percent
        /// </summary>
        public float Progress { get; set; }
    }
}