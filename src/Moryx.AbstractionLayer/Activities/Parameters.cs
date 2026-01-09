// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Processes;
using Moryx.Tools;

namespace Moryx.AbstractionLayer.Activities;

/// <summary>
/// Base class for parameters
/// </summary>
public abstract class Parameters : IParameters
{
    /// <summary>
    /// Method to create a new instance of this parameters
    /// </summary>
    private Func<Parameters> _instanceDelegate;

    private static ProcessBindingResolverFactory _resolverFactory;
    private Process _process;

    /// <summary>
    /// Singleton resolver factory for process parameter binding
    /// </summary>
    protected static ProcessBindingResolverFactory ResolverFactory => _resolverFactory ??= new ProcessBindingResolverFactory();

    /// <see cref="IParameters"/>
    public IParameters Bind(Process process)
    {
        // We are already bound to this process
        if (_process == process)
            return this;

        if (_instanceDelegate == null)
            _instanceDelegate = ReflectionTool.ConstructorDelegate<Parameters>(GetType());

        // Create new instance of this type
        var instance = _instanceDelegate();

        // Populate values
        Populate(process, instance);

        instance._process = process;
        return instance;
    }

    /// <summary>
    /// Populates the given instance with parameters
    /// </summary>
    /// <param name="instance">New instance of this type</param>
    /// <param name="process">Process to bind to</param>
    protected abstract void Populate(Process process, Parameters instance);
}