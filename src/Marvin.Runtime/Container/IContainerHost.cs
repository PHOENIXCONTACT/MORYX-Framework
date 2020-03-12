// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Marvin.Container;

namespace Marvin.Runtime.Container
{
    /// <summary>
    /// Base contract for all components hosting their own container
    /// </summary>
    public interface IContainerHost
    {
        /// <summary>
        /// Strategy configuration of this host
        /// </summary>
        IDictionary<Type, string> Strategies { get; }

        /// <summary>
        /// Container hosted by this component
        /// </summary>
        IContainer Container { get; }
    }
}
