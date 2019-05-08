using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Marvin.Logging;

namespace Marvin.Threading
{
    /// <summary>
    /// The <see cref="EventDecoupler{TEventArgs}"/> provides an event listener that enqueues incoming calls
    /// and forwards them on a dedicated worker thread
    /// </summary>
    /// <typeparam name="TEventArgs">Arguments for the <see cref="EventHandler{TEventArgs}"/> delegate</typeparam>
    internal class EventDecoupler<TEventArgs> : IDisposable
    {
        private bool _running = true;

        private Thread _eventQueueWorker;

        private readonly Queue<Tuple<object, TEventArgs>> _eventQueue = new Queue<Tuple<object, TEventArgs>>();

        /// <summary>
        /// Logger for invocation exceptions on the background thread
        /// </summary>
        private IModuleLogger Logger { get; }

        /// <summary>
        /// Target callback for the event
        /// </summary>
        private EventHandler<TEventArgs> EventTarget { get; }

        /// <summary>
        /// Create a new <see cref="EventDecoupler{TEventArgs}"/> to decouple a single listener from an event
        /// </summary>
        public EventDecoupler(IModuleLogger logger, EventHandler<TEventArgs> eventTarget)
        {
            Logger = logger;
            EventTarget = eventTarget;

            _eventQueueWorker = new Thread(ProcessEventQueue) { IsBackground = true };
            _eventQueueWorker.Start();
        }

        public void Dispose()
        {
            lock (_eventQueue)
            {
                if (!_running)
                    return;
                _running = false;

                Monitor.Pulse(_eventQueue);
            }

            // Give the thread a second to join
            if(!_eventQueueWorker.Join(1000))
                _eventQueueWorker.Abort();
            _eventQueueWorker = null;
            _eventQueue.Clear();
        }

        /// <summary>
        /// Target for the worker thread to process the event queue
        /// </summary>
        private void ProcessEventQueue()
        {
            lock (_eventQueue)
            {
                // Wait until there is the first entry in the queue
                if (_eventQueue.Count == 0)
                    Monitor.Wait(_eventQueue);

                while (_running)
                {
                    var nextEvent = _eventQueue.Dequeue();

                    Monitor.Exit(_eventQueue);

                    try
                    {
                        EventTarget?.Invoke(nextEvent.Item1, nextEvent.Item2);
                    }
                    catch (Exception e)
                    {
                        Logger?.LogException(LogLevel.Error, e, "Exception while forwarding event");
                    }

                    Monitor.Enter(_eventQueue);

                    if (_eventQueue.Count == 0)
                        Monitor.Wait(_eventQueue);
                }
            }
        }

        public void EventListener(object sender, TEventArgs eventArgs)
        {
            lock (_eventQueue)
            {
                if (!_running)
                    return;

                _eventQueue.Enqueue(new Tuple<object, TEventArgs>(sender, eventArgs));

                Monitor.Pulse(_eventQueue);
            }
        }
    }
}