// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Marvin.Threading;

namespace Marvin.Workflows
{
    /// <summary>
    /// Maps different transition types onto dedicated methods
    /// </summary>
    internal class TransitionMapper : ITransitionMapper
    {
        /// <summary>
        /// Parallel operations to execute on new thread
        /// </summary>
        private readonly IParallelOperations _parallelOperations;

        /// <summary>
        /// All registered maps
        /// </summary>
        private readonly List<IAttemptInvokation> _delegateMaps = new List<IAttemptInvokation>();

        /// <summary>
        /// Create mapper that invokes synchronus
        /// </summary>
        public TransitionMapper()
        {
        }

        /// <summary>
        /// Create mapper that uses parallel operations
        /// </summary>
        public TransitionMapper(IParallelOperations parallelOperations)
        {
            _parallelOperations = parallelOperations;
        }

        /// <summary>
        /// Event handler for <see cref="IWorkflowEngine.TransitionTriggered"/>
        /// </summary>
        public void TransitionTriggered(object sender, ITransition transition)
        {
            foreach (var invokation in _delegateMaps)
            {
                if (invokation.TryInvoke(transition))
                    break;
            }
        }

        /// <summary>
        /// Register another <see cref="IAttemptInvokation"/> strategy
        /// </summary>
        public ITransitionMapper Map(IAttemptInvokation invokation)
        {
            _delegateMaps.Add(invokation);
            return this;
        }

        /// <summary>
        /// Map transition of a certain type to this methid
        /// </summary>
        public ITransitionMapper Map<T>(Action<T> transitionHandler)
            where T : class, ITransition
        {
            if (_parallelOperations == null)
                Map(new AttemptInvokation<T>(transitionHandler));
            else
                Map(new AsyncAttemptInvokation<T>(_parallelOperations, transitionHandler));
            
            return this;
        }

        /// <summary>
        /// Delegate that trys to invoke the callback if type matches
        /// </summary>
        private struct AttemptInvokation<T> : IAttemptInvokation
            where T : class , ITransition
        {
            private readonly Action<T> _callback;

            public AttemptInvokation(Action<T> callback)
            {
                _callback = callback;
            }

            public bool TryInvoke(ITransition transition)
            {
                var casted = transition as T;
                if (casted == null)
                    return false;

                _callback(casted);
                return true;
            }
        }

        /// <summary>
        /// Delegate that trys to match the type and invokes the handler on a new thread
        /// </summary>
        private struct AsyncAttemptInvokation<T> : IAttemptInvokation
            where T : class , ITransition
        {
            private readonly Action<T> _callback;
            private readonly IParallelOperations _parallelOperations;

            public AsyncAttemptInvokation(IParallelOperations parallelOperations, Action<T> callback)
            {
                _parallelOperations = parallelOperations;
                _callback = callback;
            }

            public bool TryInvoke(ITransition transition)
            {
                var casted = transition as T;
                if (casted == null)
                    return false;

                _parallelOperations.ExecuteParallel(_callback, casted);
                return true;
            }
        }

        /// <summary>
        /// Clears the list of delegates to remove all references to the parent
        /// </summary>
        public void Dispose()
        {
            _delegateMaps.Clear();
        }
    }
}
