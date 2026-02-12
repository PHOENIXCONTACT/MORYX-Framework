// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Moryx.Threading;

namespace Moryx.Collections;

/// <summary>
/// Hash store with auto-clear functionality
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="parallelOperations"></param>
public class WriteOnlyHashStore<T>(IParallelOperations parallelOperations) : IDisposable
{
    readonly List<T> _cache = [];
    readonly Lock _lock = new();
    private int _timerId;

    /// <summary>
    /// Initialize the store with the given <paramref name="cleanUpInterval"/>
    /// </summary>
    /// <param name="cleanUpInterval"></param>
    public void Initialize(int cleanUpInterval)
    {
        _timerId = parallelOperations.ScheduleExecution(Clean, 0, cleanUpInterval);
    }

    private void Clean()
    {
        lock (_lock)
        {
            _cache.Clear();
        }
    }

    /// <summary>
    /// Write the given <paramref name="value"/> to the store
    /// </summary>
    /// <param name="value"></param>
    public void Write(T value)
    {
        lock (_lock)
        {
            _cache.Add(value);
        }
    }

    /// <summary>
    /// Checks if the store contains the given <paramref name="value"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Contains(T value)
    {
        lock (_lock)
        {
            return _cache.Contains(value);
        }
    }

    /// <summary>
    /// Manually disposese
    /// </summary>
    /// <returns></returns>
    public void Dispose()
    {
        parallelOperations.StopExecution(_timerId);
        GC.SuppressFinalize(this);
    }
}
