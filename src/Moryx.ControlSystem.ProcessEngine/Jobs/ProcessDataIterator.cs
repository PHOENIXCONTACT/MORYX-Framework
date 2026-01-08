// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;

namespace Moryx.ControlSystem.ProcessEngine.Jobs;

/// <summary>
/// Iterator for <see cref="JobDataBase.AllProcesses"/> and <see cref="JobDataBase.RunningProcesses"/> that is not affected by collection changes
/// </summary>
internal class ProcessDataIterator<T> : IEnumerator<T>
    where T : class
{
    private int _currentIndex = -1;

    private T _currentProcess;

    private readonly IReadOnlyList<T> _processes;

    public ProcessDataIterator(IReadOnlyList<T> processes)
    {
        _processes = processes;
    }

    public bool MoveNext()
    {
        if (++_currentIndex < _processes.Count)
        {
            _currentProcess = _processes[_currentIndex];
            return true;
        }
        else
        {
            _currentProcess = null;
            return false;
        }
    }

    public void Reset() => _currentIndex = -1;

    public void Dispose()
    {
    }

    public T Current => _currentProcess;

    object IEnumerator.Current => _currentProcess;
}