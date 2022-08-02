// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;

namespace Moryx.Runtime.Modules
{
    /// <summary>
    /// Interface to access all facades within MORYX
    /// </summary>
    public interface IFacadeCollector
    {
        /// <summary>
        /// All facades of the running application
        /// </summary>
        IReadOnlyList<object> Facades { get; }
    }
}