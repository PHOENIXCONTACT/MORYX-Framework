// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Modules;

/// <summary>
/// Interface for all modules offering an API Facade
/// </summary>
/// <typeparam name="TFacade">Type of facade offered by this module</typeparam>
public interface IFacadeContainer<out TFacade>
{
    /// <summary>
    /// Facade controlled by this module
    /// </summary>
    /// <remarks>
    /// The hard-coded name of this property is also used in Moryx.Runtime.Kernel\ModuleManagement\Components\ModuleDependencyManager.cs
    /// </remarks>
    TFacade Facade { get; }
}