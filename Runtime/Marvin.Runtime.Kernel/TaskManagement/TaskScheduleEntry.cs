using System;
using Marvin.Runtime.Tasks;

namespace Marvin.Runtime.Kernel.TaskManagement
{
    internal class TaskScheduleEntry
    {
        public int Id { get; set; }

        public ITask Task { get; set; }

        public ITaskSchedule Schedule { get; set; }

        public TaskWorker AssignedWorker { get; set; }

        public DateTime LastStart { get; set; }

        public DateTime LastCompletion { get; set; }
    }
}