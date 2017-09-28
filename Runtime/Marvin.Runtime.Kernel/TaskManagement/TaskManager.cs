using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Marvin.Configuration;
using Marvin.Container;
using Marvin.Logging;
using Marvin.Modules;
using Marvin.Runtime.Tasks;

namespace Marvin.Runtime.Kernel.TaskManagement
{
    /// <summary>
    /// Manager for handling incoming tasks and provide methods to start them.
    /// </summary>
    [InitializableKernelComponent(typeof(ITaskManager))]
    public class TaskManager : ITaskManager, IInitializable, IDisposable, ILoggingHost
    {
        /// <summary>
        /// Configuration manager instance. Injected by castel.
        /// </summary>
        public IConfigManager ConfigManager { get; set; }
        /// <summary>
        /// Logger management instance. Injected by castle.
        /// </summary>
        public ILoggerManagement Logging { get; set; }

        private TaskManagerConfig _config;

        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private TaskScheduler _scheduler;
        private readonly IList<TaskWorker> _workers = new List<TaskWorker>();


        /// <summary>
        /// Initialize this component and prepare it for incoming tasks. This must only involve preparation and must not start 
        /// any active functionality and/or periodic execution of logic.
        /// </summary>
        public void Initialize()
        {
            _config = ConfigManager.GetConfiguration<TaskManagerConfig>();
            Logging.ActivateLogging(this);

            var workerType = typeof(TaskWorker);
            for (var i = 0; i < _config.ThreadCount; i++)
            {
                var worker = new TaskWorker(i, _tokenSource) { Logger = Logger.GetChild(string.Empty, workerType) };
                worker.ExecutionCompleted += WorkerCompletedTask;
                worker.Initialize();
                _workers.Add(worker);
            }

            _scheduler = new TaskScheduler
            {
                Config = _config,
                Logger = Logger.GetChild(string.Empty, typeof(TaskScheduler)),
                Run = RunTask,
            };
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _tokenSource.Cancel();
            _scheduler.Dispose();

            lock (_workers)
            {
                foreach (var worker in _workers)
                {
                    worker.Dispose();
                }
            }
            Logging.DeactivateLogging(this);
        }

        /// <summary>
        /// Number of available parallel threads
        /// </summary>
        public int WorkerThreads { get { return _config.ThreadCount; } }

        /// <summary>
        /// Full task list
        /// </summary>
        public IEnumerable<ScheduledTask> Tasks
        {
            get
            {
                return _scheduler.FullSchedule.Select(fs => new ScheduledTask
                {
                    Id = fs.Id,
                    Name = fs.Task.Name,
                    Running = fs.AssignedWorker != null,
                    Progress = fs.Task.EstimateProgress()
                });
            }
        }

        /// <summary>
        /// Schedule this task for execution
        /// </summary>
        /// <returns>Task id that can be used to remove the task</returns>
        public int Schedule(ITask task, ITaskSchedule schedule)
        {
            return _scheduler.Schedule(task, schedule);
        }

        private TaskWorker RunTask(ITask taskEntry)
        {
            lock (_workers)
            {
                // Find free worker and assign task
                var worker = _workers.FirstOrDefault(w => !w.IsBusy);
                if (worker == null)
                {
                    Logger.LogEntry(LogLevel.Warning, "No available worker for Task {0}", taskEntry.Name);
                    return null;
                }

                worker.Assign(taskEntry);
                Logger.LogEntry(LogLevel.Info, "Started running task {0} on {1}", taskEntry.Name, worker.Name);
                return worker;
            }
        }

        private void WorkerCompletedTask(object sender, ITask task)
        {
            lock (_workers)
            {
                var worker = (TaskWorker) sender;
                Logger.LogEntry(LogLevel.Info, "Completed task {0} on {1}", task.Name, worker.Name);
                _scheduler.Completed(task, DateTime.Now);
            }
        }

        /// <summary>
        /// Name of this host. Used for logger name structure
        /// </summary>
        public string Name { get { return "Kernel"; } }

        /// <summary>
        /// Logger instance
        /// </summary>
        public IModuleLogger Logger { get; set; }
    }
}