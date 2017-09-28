using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Marvin.Logging;
using Marvin.Runtime.Tasks;

namespace Marvin.Runtime.Kernel.TaskManagement
{
    internal class TaskScheduler : IDisposable
    {
        public TaskManagerConfig Config { get; set; }
        public IModuleLogger Logger { get; set; }

        private int _taskId;
        // All entries
        private readonly IList<TaskScheduleEntry> _schedule = new List<TaskScheduleEntry>();
        // Due entries that could not receive an empty worker
        private readonly Queue<TaskScheduleEntry> _pending = new Queue<TaskScheduleEntry>();

        private readonly Timer _timer;
        private DateTime _nextExecution;

        public TaskScheduler()
        {
            _timer = new Timer(CheckSchedules);
        }

        /// <summary>
        /// Schedule this task for execution
        /// </summary>
        /// <returns>Task id that can be used to remove the task</returns>
        public int Schedule(ITask task, ITaskSchedule schedule)
        {
            // Create and entry schedule entry
            var taskId = ++_taskId;
            var scheduleEntry = new TaskScheduleEntry
            {
                Id = taskId,
                Task = task,
                Schedule = schedule
            };
            task.Disposing += TaskDisposing;
            lock (_schedule)
                _schedule.Add(scheduleEntry);

            // Check if the timer can hit the current due date
            if (!TimeShiftRequired(schedule.StartDate))
                return taskId;

            if (schedule.StartDate >= DateTime.Now)
            {
                // Shift next check
                _nextExecution = schedule.StartDate;
                _timer.Change(_nextExecution - DateTime.Now, Timeout.InfiniteTimeSpan);
            }
            else
            {
                // Immidiate execution
                CheckSchedules(null);
            }
            return taskId;
        }

        /// <summary>
        /// Invoked when a given task is being disposed
        /// </summary>
        private void TaskDisposing(object sender, EventArgs eventArgs)
        {
            lock (_schedule)
            {
                var task = (ITask)sender;
                task.Disposing -= TaskDisposing;
                var taskSchedule = _schedule.FirstOrDefault(e => e.Task == task);
                _schedule.Remove(taskSchedule);
            }
        }

        /// <summary>
        /// Check all task schedules
        /// </summary>
        /// <param name="state"></param>
        private void CheckSchedules(object state)
        {
            // Stop timer
            _timer.Change(Timeout.Infinite, Timeout.Infinite);

            // Clear the queue, pending entries will be readded as the startdate is still pending and their due date smaller
            _pending.Clear();

            // Copy task list in lock for save access
            TaskScheduleEntry[] currentSchedule;
            lock (_schedule)
                currentSchedule = _schedule.ToArray();

            // Find startable task and next task for timer
            var startable = currentSchedule.OrderBy(e => e.Schedule.DueDate).Where(e => e.Schedule.StartDate <= DateTime.Now).ToArray();
            var next = currentSchedule.OrderBy(e => e.Schedule.StartDate).FirstOrDefault(e => e.Schedule.StartDate > DateTime.Now);

            var newlyRunning = 0;
            var busyWorkers = _schedule.Count(se => se.AssignedWorker != null);
            for (var index = 0; index < startable.Length; index++)
            {
                var scheduleEntry = startable[index];
                if (busyWorkers + index < Config.ThreadCount)
                {
                    // Execute if we have threads left
                    scheduleEntry.AssignedWorker = Run(scheduleEntry.Task);
                    newlyRunning++;
                }
                else
                    // Enqueue if we ran out of threads
                    _pending.Enqueue(scheduleEntry);
                index++;
            }
            Logger.LogEntry(LogLevel.Info, "Scheduling result: {0} allready running, {1} newly running, {2} pending, {3} total",
                                           busyWorkers, newlyRunning, _pending.Count, _schedule.Count);

            if (next == null)
            {
                _nextExecution = new DateTime();
            }
            else
            {
                _nextExecution = next.Schedule.StartDate;
                _timer.Change(_nextExecution - DateTime.Now, Timeout.InfiniteTimeSpan);
            }
        }

        /// <summary>
        /// Get all scheduled entries
        /// </summary>
        public IEnumerable<TaskScheduleEntry> FullSchedule
        {
            get { return _schedule; }
        }

        /// <summary>
        /// Delegate invoked when a task reached its due date and shall be executed
        /// </summary>
        public Func<ITask, TaskWorker> Run { get; set; }

        /// <summary>
        /// Task execution was completed
        /// </summary>
        public void Completed(ITask task, DateTime executionTime)
        {
            TaskScheduleEntry scheduleEntry;
            lock (_schedule)
                scheduleEntry = _schedule.First(s => s.Task == task);

            scheduleEntry.Schedule.TaskRun(executionTime);
            scheduleEntry.AssignedWorker = null;

            // Check if we need to run the task again or update next timer execution
            if (!scheduleEntry.Schedule.Executable)
                _schedule.Remove(scheduleEntry);
            else if (TimeShiftRequired(scheduleEntry.Schedule.StartDate))
                _timer.Change(scheduleEntry.Schedule.StartDate - DateTime.Now, Timeout.InfiniteTimeSpan);

            // Get next task from pending queue
            if (_pending.Any())
            {
                var nextEntry = _pending.Dequeue();
                nextEntry.AssignedWorker = Run(nextEntry.Task);
            }
        }

        /// <summary>
        /// Check if the timer needs to be changed for this operation
        /// </summary>
        /// <param name="startDate"></param>
        /// <returns></returns>
        private bool TimeShiftRequired(DateTime startDate)
        {
            return _nextExecution < DateTime.Now || _nextExecution > startDate;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _timer.Dispose();
            _schedule.Clear();
        }
    }
}