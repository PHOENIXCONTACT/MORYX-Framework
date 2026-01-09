// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Threading;

namespace Moryx.Workplans;

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
    private readonly List<IAttemptInvocation> _delegateMaps = [];

    /// <summary>
    /// Create mapper that invokes synchronous
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
    /// Event handler for <see cref="IWorkplanEngine.TransitionTriggered"/>
    /// </summary>
    public void TransitionTriggered(object sender, ITransition transition)
    {
        foreach (var invocation in _delegateMaps)
        {
            if (invocation.TryInvoke(transition))
                break;
        }
    }

    /// <summary>
    /// Register another <see cref="IAttemptInvocation"/> strategy
    /// </summary>
    public ITransitionMapper Map(IAttemptInvocation invocation)
    {
        _delegateMaps.Add(invocation);
        return this;
    }

    /// <summary>
    /// Map transition of a certain type to this method
    /// </summary>
    public ITransitionMapper Map<T>(Action<T> transitionHandler)
        where T : class, ITransition
    {
        if (_parallelOperations == null)
            Map(new AttemptInvocation<T>(transitionHandler));
        else
            Map(new AsyncAttemptInvocation<T>(_parallelOperations, transitionHandler));

        return this;
    }

    /// <summary>
    /// Delegate that tries to invoke the callback if type matches
    /// </summary>
    private readonly struct AttemptInvocation<T> : IAttemptInvocation
        where T : class, ITransition
    {
        private readonly Action<T> _callback;

        public AttemptInvocation(Action<T> callback)
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
    /// Delegate that tries to match the type and invokes the handler on a new thread
    /// </summary>
    private readonly struct AsyncAttemptInvocation<T> : IAttemptInvocation
        where T : class, ITransition
    {
        private readonly Action<T> _callback;
        private readonly IParallelOperations _parallelOperations;

        public AsyncAttemptInvocation(IParallelOperations parallelOperations, Action<T> callback)
        {
            _parallelOperations = parallelOperations;
            _callback = callback;
        }

        public bool TryInvoke(ITransition transition)
        {
            if (!(transition is T casted))
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