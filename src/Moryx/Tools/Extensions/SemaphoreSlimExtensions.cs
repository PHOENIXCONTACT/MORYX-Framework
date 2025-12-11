// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Tools;

/// <summary>
/// Extensions for the <see cref="SemaphoreSlim"/>
/// </summary>
public static class SemaphoreSlimExtensions
{
    /// <summary>
    /// Extensions for the <see cref="SemaphoreSlim"/>
    /// </summary>
    /// <param name="semaphore">The <see cref="SemaphoreSlim"/> that controls access to the critical section.</param>
    extension(SemaphoreSlim semaphore)
    {
        /// <summary>
        /// Executes the specified asynchronous operation within a semaphore lock.
        /// The method waits asynchronously to enter the semaphore, runs the provided
        /// function, and guarantees that the semaphore is released afterwards.
        /// </summary>
        /// <param name="criticalFunc">The asynchronous operation to execute inside the lock.</param>
        /// <returns>
        /// A <see cref="Task"/> that represents the asynchronous operation.
        /// The task completes when the function has finished executing and the semaphore
        /// has been released.
        /// </returns>
        /// <remarks>
        /// This helper abstracts the common pattern of calling <c>WaitAsync</c>,
        /// executing a critical function, and ensuring <c>Release</c> is called in a
        /// <c>finally</c> block. It improves readability and reduces boilerplate
        /// when protecting asynchronous critical sections.
        /// </remarks>
        public async Task ExecuteAsync(Func<Task> criticalFunc, CancellationToken cancellationToken = default)
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                await criticalFunc();
            }
            finally
            {
                semaphore.Release();
            }
        }

        /// <summary>
        /// Executes the specified asynchronous operation within a semaphore lock.
        /// The method waits asynchronously to enter the semaphore, runs the provided
        /// function, and guarantees that the semaphore is released afterwards.
        /// </summary>
        /// <param name="criticalFunc">The asynchronous operation to execute inside the lock.</param>
        /// <typeparam name="T">The result type of the asynchronous operation.</typeparam>
        /// <returns>
        /// A <see cref="Task{T}"/> that represents the asynchronous operation.
        /// The task completes when the function has finished executing and the semaphore
        /// has been released.
        /// </returns>
        /// <remarks>
        /// This helper abstracts the common pattern of calling <c>WaitAsync</c>,
        /// executing a critical function, and ensuring <c>Release</c> is called in a
        /// <c>finally</c> block. It improves readability and reduces boilerplate
        /// when protecting asynchronous critical sections.
        /// </remarks>
        public async Task<T> ExecuteAsync<T>(Func<Task<T>> criticalFunc, CancellationToken cancellationToken = default)
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                return await criticalFunc();
            }
            finally
            {
                semaphore.Release();
            }
        }

        /// <summary>
        /// Executes the specified operation within a semaphore lock.
        /// The method waits to enter the semaphore, runs the provided
        /// function, and guarantees that the semaphore is released afterwards.
        /// </summary>
        /// <param name="criticalFunc">The operation to execute inside the lock.</param>
        /// <remarks>
        /// This helper abstracts the common pattern of calling <c>Wait</c>,
        /// executing a critical function, and ensuring <c>Release</c> is called in a
        /// <c>finally</c> block. It improves readability and reduces boilerplate
        /// when protecting asynchronous critical sections.
        /// </remarks>
        public void Execute(Action criticalFunc)
        {
            semaphore.Wait();
            try
            {
                criticalFunc();
            }
            finally
            {
                semaphore.Release();
            }
        }

        /// <summary>
        /// Executes the specified operation within a semaphore lock.
        /// The method waits to enter the semaphore, runs the provided
        /// function, and guarantees that the semaphore is released afterwards.
        /// </summary>
        /// <param name="criticalFunc">The operation to execute inside the lock.</param>
        /// <typeparam name="T">The result type of the operation.</typeparam>
        /// <returns>
        /// A <see cref="T"/> that represents the operation.
        /// The task completes when the function has finished executing and the semaphore
        /// has been released.
        /// </returns>
        /// <remarks>
        /// This helper abstracts the common pattern of calling <c>Wait</c>,
        /// executing a critical function, and ensuring <c>Release</c> is called in a
        /// <c>finally</c> block. It improves readability and reduces boilerplate
        /// when protecting asynchronous critical sections.
        /// </remarks>
        public T Execute<T>(Func<T> criticalFunc)
        {
            semaphore.Wait();
            try
            {
                return criticalFunc();
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
