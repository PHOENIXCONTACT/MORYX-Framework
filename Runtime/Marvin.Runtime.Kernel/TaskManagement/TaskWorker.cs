using System;
using System.Collections.Concurrent;
using System.Threading;
using Marvin.Logging;
using Marvin.Modules;
using Marvin.Runtime.Tasks;

namespace Marvin.Runtime.Kernel.TaskManagement
{
    internal class TaskWorker : IInitializable, IDisposable
    {
        public IModuleLogger Logger { get; set; }
        public string Name { get; set; }

        public bool IsBusy { get; private set; }

        private Thread _worker;
        private readonly CancellationTokenSource _tokenSource;
        private readonly BlockingCollection<ITask> _taskQueue = new BlockingCollection<ITask>(); 

        public TaskWorker(int number, CancellationTokenSource tokenSource)
        {
            _tokenSource = tokenSource;
            Name = string.Format("TaskWorker{0}", number);
        }

        #region Life cycle

        /// <summary>
        /// Initialize this component and prepare it for incoming taks. This must only involve preparation and must not start 
        /// any active functionality and/or periodic execution of logic.
        /// </summary>
        public void Initialize()
        {
            _worker = new Thread(Run) { IsBackground = true, Name = Name };
            _worker.Start();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _worker.Join(1000);
            _worker.Abort();
        }

        #endregion

        #region Task assignment

        /// <summary>
        /// Assign a task to this worker
        /// </summary>
        public void Assign(ITask task)
        {
            IsBusy = true;
            _taskQueue.Add(task);
        }

        /// <summary>
        /// Event raised when the run call returned
        /// </summary>
        public EventHandler<ITask> ExecutionCompleted;

        #endregion

        #region Task execution

        /// <summary>
        /// Run the assigned task
        /// </summary>
        private void Run(object unused)
        {
            var token = _tokenSource.Token;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var task = _taskQueue.Take(_tokenSource.Token);
                    RunTask(task);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Run the task save
        /// </summary>
        private void RunTask(ITask task)
        {
            try
            {
                task.Run(_tokenSource.Token);
                ExecutionCompleted(this, task);
            }
            catch (Exception ex)
            {
                Logger.LogException(LogLevel.Error, ex, "Execution of task {0} failed!", task.Name);
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

    }
}