// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer
{
    /// <summary>
    /// This is a protected base class that can not not be extended outside this assembly
    /// </summary>
    public abstract class ParametersBase : IParameters
    {
        private IProcess _process;
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

        private static ProcessBindingResolverFactory _resolverFactory;
        /// <summary>
        /// Singleton resolver factory for process parameter binding
        /// </summary>
        protected static ProcessBindingResolverFactory ResolverFactory => _resolverFactory ?? (_resolverFactory = new ProcessBindingResolverFactory());

        /// <summary>
        /// Resolve the binding parameters
        /// </summary>
        /// <param name="process">Process to bind to</param>
        /// <returns>Parameters with resolved bindings</returns>
        protected abstract ParametersBase ResolveBinding(IProcess process);
    }
}
