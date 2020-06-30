// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// The thread context to be used by console applications or Windows services for the <see cref="BaseWcfClientFactory"/>
    /// </summary>
    public class SimpleThreadContext : IThreadContext
    {
        /// 
        public void Invoke(Action action)
        {
            action();
        }
    }
}
