// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Tools;

namespace Moryx.AbstractionLayer
{
    /// <summary>
    /// Base class for parameters
    /// </summary>
#pragma warning disable 618 //TODO: Remove if ParametersBase was removed
    public abstract class Parameters : ParametersBase
    {
        /// <summary>
        /// Method to create a new instance of this parameters
        /// </summary>
        private Func<Parameters> _instanceDelegate;

        /// <summary>
        /// Resolve the binding parameters
        /// </summary>
        /// <param name="process">Process to bind to</param>
        /// <returns>Parameters with resolved bindings</returns>
        protected sealed override ParametersBase ResolveBinding(IProcess process)
        {
            if (_instanceDelegate == null)
                _instanceDelegate = ReflectionTool.ConstructorDelegate<Parameters>(GetType());

            var instance = _instanceDelegate();
            Populate(process, instance);
            return instance;
        }

        /// <summary>
        /// Populates the given instance with parameters
        /// </summary>
        /// <param name="instance">New instance of this type</param>
        /// <param name="process">Process to bind to</param>
        protected abstract void Populate(IProcess process, Parameters instance);
    }
#pragma warning restore 618
}