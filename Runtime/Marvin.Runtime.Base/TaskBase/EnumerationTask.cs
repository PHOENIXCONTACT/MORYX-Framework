using System;
using System.Collections.Generic;
using System.Threading;

namespace Marvin.Runtime.Base.TaskBase
{
    /// <summary>
    /// Run tasks on enumerables and can pause and resume them after each entry.
    /// </summary>
    /// <typeparam name="TEnumerable">Type of the enumeration entries.</typeparam>
    /// <typeparam name="TContext">User defined context which is needed for a task.</typeparam>
    public abstract class EnumerationTask<TEnumerable, TContext> : TaskBase
        where TContext : class, IDisposable
    {
        private bool _shallRun = true;

        private TContext _context;
        private IEnumerator<TEnumerable> _enumerator;
        private int _totalIterations;
        private int _currentIteration;

        /// <summary>
        /// Run or resume this task
        /// </summary>
        public sealed override void Run(CancellationToken token)
        {
            // If it was not paused we have to prepare for fresh run
            if (_context == null || _enumerator == null)
            {
                _context = Init();
                _totalIterations = CalculateIterations();
                _enumerator = GetEnumerable().GetEnumerator();
            }

            while (_shallRun && !token.IsCancellationRequested && _enumerator.MoveNext())
            {
                _currentIteration ++;
                Process(_context, _enumerator.Current);
            }

            // Only paused
            if (_currentIteration <= _totalIterations) 
                return;

            _context.Dispose();
            _context = null;

            _enumerator.Dispose();
            _enumerator = null;
        }

        /// <summary>
        /// Prepare task execution
        /// </summary>
        protected abstract TContext Init();

        /// <summary>
        /// Calculate total number of iterations for this run
        /// </summary>
        protected abstract int CalculateIterations();

        /// <summary>
        /// Get enumerable to run over
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<TEnumerable> GetEnumerable();

        /// <summary>
        /// Process this entry
        /// </summary>
        protected abstract void Process(TContext context, TEnumerable item);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            _currentIteration = _totalIterations;
            _shallRun = false;

            base.Dispose();
        }

        /// <summary>
        /// Estimate current execution progress in percent
        /// </summary>
        public sealed override float EstimateProgress()
        {
            return ((float)_currentIteration) / _totalIterations;
        }
    }
}