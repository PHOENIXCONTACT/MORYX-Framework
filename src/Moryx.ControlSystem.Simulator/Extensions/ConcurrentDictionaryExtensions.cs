// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace Moryx.Simulation.Simulator.Extensions;

internal static class ConcurrentDictionaryExtensions
{
    public static bool WaitUntilEmpty<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TimeSpan timeout, TimeSpan pollInterval)
    {
        using var cts = new CancellationTokenSource(timeout);
        var token = cts.Token;

        try
        {
            while (!token.IsCancellationRequested)
            {
                if (dictionary.IsEmpty)
                {
                    return true;
                }

                // Wait asynchronously to avoid busy waiting
                Task.Delay(pollInterval, token).Wait(token);
            }
        }
        catch (OperationCanceledException)
        {
            // Timeout occurred
        }

        return dictionary.IsEmpty;
    }
}
