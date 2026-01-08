// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Container
{
    /// <summary>
    /// Interface for all components that allow access to direct children
    /// </summary>
    public interface INamedChildContainer<out T>
        where T : class
    {
        /// <summary>
        /// Get the child with this name for a specific component
        /// </summary>
        /// <param name="name">Name of the child or empty for child with the same name</param>
        /// <param name="target">Target component the child is assigned to</param>
        T GetChild(string name, Type target);
    }

    /// <summary>
    /// Interface for components that are part of their parents container
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    public interface IContainerChild<TParent> where TParent : INamedChildContainer<object>
    {
        /// <summary>
        /// Parent container of this child
        /// </summary>
        TParent Parent { get; set; }
    }
}
