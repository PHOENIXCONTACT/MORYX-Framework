// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using Marvin.Modules;

namespace Marvin.Container
{
    /// <summary>
    /// Register a kernel component
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class KernelComponentAttribute : GlobalComponentAttribute
    {
        /// <summary>
        /// Register a kernel component
        /// </summary>
        public KernelComponentAttribute(params Type[] services)
            : base(LifeCycle.Singleton, services)
        {
        }
    }

    /// <summary>
    /// Register a kernel component
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class InitializableKernelComponentAttribute : KernelComponentAttribute
    {
        /// <summary>
        /// Register a kernel component
        /// </summary>
        public InitializableKernelComponentAttribute(params Type[] services)
            : base(new[] { typeof(IInitializable) }.Union(services).ToArray())
        {
        }
    }

    /// <summary>
    /// Register a kernel plugin
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class KernelPluginAttribute : GlobalComponentAttribute
    {
        /// <summary>
        /// Register a kernel component
        /// </summary>
        public KernelPluginAttribute(params Type[] services)
            : base(LifeCycle.Singleton, services)
        {
        }
    }

}
