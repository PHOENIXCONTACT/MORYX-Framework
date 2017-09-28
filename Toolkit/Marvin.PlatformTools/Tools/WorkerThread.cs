using System;
using System.Threading;
using Marvin.Logging;

namespace Marvin.Tools
{
    /// <summary>
    /// WorkerThread is a wrapper for a background <see cref="System.Threading.Thread"/>
    /// containing a standard initialization and a main loop with error handling.
    /// In contrast to <see cref="System.Threading.Thread"/>, he WorkerThread can be started again after it is stopped.
    /// </summary>
    public class WorkerThread : IDisposable
    {
        #region Fields and Properties

        private readonly string _name;
        private readonly Action _worker;
        private readonly int _sleepTime;
        private readonly IModuleLogger _logger;

        private Thread _thread;

        private bool _disposed;

        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        /// <summary>
        /// Checks wether the contained Thread is alive.
        /// </summary>
        public bool IsAlive
        {
            get { return (_thread != null) && _thread.IsAlive; }
        }

        /// <summary>
        /// The <see cref="CancellationToken"/> associated with this WorkerThread.
        /// </summary>
        public CancellationToken Token
        {
            get { return _tokenSource.Token; }
        }

        #endregion

        /// <summary>
        /// Creates a new WorkerThread
        /// </summary>
        /// <param name="name">The name of the Thread</param>
        /// <param name="worker">The delegate to be called for each execution loop. 
        /// The delegate does not need to loop or sleep.</param>
        /// <param name="sleepTime">The time in milliseconds to sleep after each loop cycle.</param>
        /// <param name="logger">The logger to use for log messages. If null, the WorkerThread creates one on its own.</param>
        /// <exception cref="ArgumentNullException">If the argument <code>worker</code> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the argument <code>sleepTime</code> is &lt;= 0.</exception>
        public WorkerThread(String name, Action worker, int sleepTime, IModuleLogger logger)
        {
            if (worker == null)
                throw new ArgumentNullException("worker");

            if (sleepTime <= 0)
                throw new ArgumentOutOfRangeException("sleepTime", sleepTime, "Sleeptime must be > 0");

            _name = name;
            _worker = worker;
            _sleepTime = sleepTime;
            _logger = logger;
        }

        /// <summary>
        /// Stopps this WorkerThread and disables its Start() method.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            Stop();
        }

        /// <summary>
        /// Creates and starts the contained Thread. The thread can be started only once.
        /// </summary>
        /// <exception cref="InvalidOperationException">If this instance of WorkerThread is already disposed.</exception>
        /// <exception cref="InvalidOperationException">If this WorkerThread is already (or still) running or already finished.</exception>
        public void Start()
        {
            if (_disposed)
            {
                throw new InvalidOperationException(String.Format("WorkerThread '{0}' is already disposed.", _name));
            }

            if (_thread != null && _thread.IsAlive)
            {
                throw new InvalidOperationException(String.Format("WorkerThread '{0}' is already running.", _name));
            }

            var token = _tokenSource.Token;

            if (token.IsCancellationRequested)
            {
                throw new InvalidOperationException(String.Format("WorkerThread '{0}' is already finished.", _name));
            }

            _thread = new Thread(WorkerThreadLoop);

            if (! String.IsNullOrEmpty(_name))
            {
                _thread.Name = _name;
            }

            _thread.IsBackground = true;

            _thread.Start();
        }

        /// <summary>
        /// Loop for the worker thread
        /// </summary>
        private void WorkerThreadLoop()
        {
            _logger.LogEntry(LogLevel.Info, "Starting thread '{0}'", _name);

            var token = _tokenSource.Token;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    _worker();
                }
                catch (Exception e)
                {
                    _logger.LogException(LogLevel.Error, e, "Caught unexpected exception");
                }

                try
                {
                    Thread.Sleep(_sleepTime);
                }
                catch (ThreadInterruptedException)
                {
                    // Do nothing
                }
            }

            _logger.LogEntry(LogLevel.Info, "Thread '{0}' finished.", _name);
        }

        /// <summary>
        /// Stopps this WorkerThread. The Thread is not immediately stopped if its delegate is currently working. 
        /// If it is sleeping, the sleep is interrupted. An additional cycle will not be executed.
        /// </summary>
        public void Stop()
        {
            _logger.LogEntry(LogLevel.Info, "Stopping thread '{0}'", _name);

            _tokenSource.Cancel();

            if (_thread != null)
            {
                _thread.Interrupt();
            }
        }

        /// <summary>
        /// Calls Join() on the contained System.Threading.Thread. If there is no Thread yet, it returns immediately.
        /// </summary>
        /// <param name="timeout">The timeout to wait until the call returns even if the Thread has not yet died.</param>
        public void Join(int timeout)
        {
            if (_thread != null)
                _thread.Join(timeout);
        }
    }
}