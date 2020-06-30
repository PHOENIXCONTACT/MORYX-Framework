// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// The thread context to be used for Invoke().
    /// </summary>
    public interface IThreadContext
    {
        /// <summary>
        /// For WPF applications, executes the specified Action synchronously on the thread the Dispatcher is associated with.
        /// For other applications, executes the specified Action synchronously.
        /// </summary>
        /// <param name="action">A delegate to invoke</param>
        void Invoke(Action action);
    }
}
