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
