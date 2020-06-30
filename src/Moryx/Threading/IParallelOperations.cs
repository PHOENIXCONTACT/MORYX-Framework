// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.Threading
{
    /// <summary>
    /// Interface for operations that should be executed parallely without redundant try catch
    /// </summary>
    public interface IParallelOperations : IDisposable
    {
        /// <summary>
        /// Execute operation on ThreadPool thread
        /// </summary>
        void ExecuteParallel(Action operation);
        /// <summary>
        /// Execute operation on ThreadPool thread
        /// </summary>
        void ExecuteParallel(Action operation, bool criticalOperation);
        /// <summary>
        /// Execute operation on ThreadPool thread
        /// </summary>
        void ExecuteParallel<T>(Action<T> operation, T userState) where T : class;
        /// <summary>
        /// Execute operation on ThreadPool thread
        /// </summary>
        void ExecuteParallel<T>(Action<T> operation, T userState, bool criticalOperation) where T : class;

        /// <summary>
        /// Execute operation periodically but non-stacking
        /// </summary>
        /// <returns>Timer id to stop periodic execution</returns>
        int ScheduleExecution(Action operation, int delayMs, int periodMs);
        /// <summary>
        /// Execute operation periodically but non-stacking
        /// </summary>
        /// <returns>Timer id to stop periodic execution</returns>
        int ScheduleExecution(Action operation, int delayMs, int periodMs, bool criticalOperation);
        /// <summary>
        /// Execute operation periodically but non-stacking
        /// </summary>
        /// <returns>Timer id to stop periodic execution</returns>
        int ScheduleExecution<T>(Action<T> operation, T userState, int delayMs, int periodMs) where T : class;
        /// <summary>
        /// Execute operation periodically but non-stacking
        /// </summary>
        /// <returns>Timer id to stop periodic execution</returns>
        int ScheduleExecution<T>(Action<T> operation, T userState, int delayMs, int periodMs, bool criticalOperation) where T : class;

        /// <summary>
        /// Stop the execution of the timer with this id
        /// </summary>
        /// <param name="timerId"></param>
        void StopExecution(int timerId);

        /// <summary>
        /// Decouple the event listener delegate from the event invocation thread
        /// </summary>
        /// <param name="target">Target delegate</param>
        /// <returns>The decoupling event listener</returns>
        EventHandler DecoupleListener(EventHandler<EventArgs> target);

        /// <summary>
        /// Decouple the event listener delegate from the event invocation thread
        /// </summary>
        /// <typeparam name="TEventArgs">Type of event args</typeparam>
        /// <param name="target">Target delegate</param>
        /// <returns>The decoupling event listener</returns>
        EventHandler<TEventArgs> DecoupleListener<TEventArgs>(EventHandler<TEventArgs> target);

        /// <summary>
        /// Remove the decouple listener from the event
        /// </summary>
        /// <param name="target">Target delegate that is removed as a listener</param>
        /// <returns>The decoupling event listener</returns>
        EventHandler RemoveListener(EventHandler<EventArgs> target);

        /// <summary>
        /// Remove the decouple listener from the event
        /// </summary>
        /// <typeparam name="TEventArgs">Type of event args</typeparam>
        /// <param name="target">Target delegate that is removed as a listener</param>
        /// <returns>The decoupling event listener</returns>
        EventHandler<TEventArgs> RemoveListener<TEventArgs>(EventHandler<TEventArgs> target);
    }
}
