// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.AbstractionLayer
{
    /// <summary>
    /// Outdated base class for parameters use <see cref="Parameters"/> instead
    /// </summary>
    [Obsolete("Use '" + nameof(Parameters) + "' instead!")]
    public abstract class ParametersBase : IParameters
    {
        private static ProcessBindingResolverFactory _resolverFactory;
        private IProcess _process;

        /// <summary>
        /// Singleton resolver factory for process parameter binding
        /// </summary>
        protected static ProcessBindingResolverFactory ResolverFactory => _resolverFactory ?? (_resolverFactory = new ProcessBindingResolverFactory());

        /// <see cref="IParameters"/>
        public IParameters Bind(IProcess process)
        {
            // We are already bound to this process
            if (_process == process)
                return this;

            var resolved = ResolveBinding(process);
            resolved._process = process;
            return resolved;
        }

        /// <summary>
        /// Resolve the binding parameters
        /// </summary>
        /// <param name="process">Process to bind to</param>
        /// <returns>Parameters with resolved bindings</returns>
        protected abstract ParametersBase ResolveBinding(IProcess process);
    }
}
