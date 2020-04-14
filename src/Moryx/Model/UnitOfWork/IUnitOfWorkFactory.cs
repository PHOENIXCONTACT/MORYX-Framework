// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Container;

namespace Moryx.Model
{
    /// <summary>
    /// Factory to open db context wrapped in UnitOfWork
    /// </summary>
    public interface IUnitOfWorkFactory : INamedChildContainer<IUnitOfWorkFactory>
    {
        /// <summary>
        /// Create new context in default mode ProxyLazyLoadingTracking
        /// </summary>
        IUnitOfWork Create();

        /// <summary>
        /// Specify the context mode
        /// </summary>
        IUnitOfWork Create(ContextMode mode);
    }

    /// <summary>
    /// Available ContextMode Constants
    /// </summary>
    internal struct ContextModeFlags
    {
        /// <summary>
        /// No features set
        /// </summary>
        public const int AllOff = 0x0;

        /// <summary>
        /// Poxy enabled
        /// </summary>
        public const int Proxy = 0x1;

        /// <summary>
        /// Lazy loading enabled
        /// </summary>
        public const int Lazy = 0x2;

        /// <summary>
        /// Auto detect changes
        /// </summary>
        public const int ChangeTracking = 0x4;
    }

    /// <summary>
    /// Configuration for the context creation. It will alter the values for entity tracking, proxy creation
    /// and lazy loading. Each mode has its advantages and disadvantages
    /// Flags: Tracking LazyLoading Proxy
    /// </summary>
    [Flags]
    public enum ContextMode
    {
        /// <summary>
        /// Don't use any context enhancements - simple database access. This mode is used best for read-only
        /// operations and LinQ queries. This will not track changes made to entities and does not provide
        /// deferred loading of entities.
        /// </summary>
        AllOff = ContextModeFlags.AllOff,

        /// <summary>
        /// Wrap each requested entity into a proxy class for enhanced EF features. Using this
        /// stand alone does not provide any benefit and should only be done as an intermediate step for performance
        /// enhancement.
        /// </summary>
        ProxyOnly = ContextModeFlags.Proxy,

        /// <summary>
        /// Enables proxies and the lazy loading feature for virtual navigation properties
        /// </summary>
        LazyLoading = ContextModeFlags.Proxy | ContextModeFlags.Lazy,

        /// <summary>
        /// Keep track of entities and changes made to them. This works with (Dynamic Change Tracking) proxies.
        /// This mode is ideal for reading operations split into several minor reads. Start with loading the root entity and load navigation properties on-demand
        /// using the repository Load-method.
        /// </summary>
        ChangeTracking = ContextModeFlags.Proxy | ContextModeFlags.ChangeTracking,

        /// <summary>
        /// Use the full available feature set of proxies, lazy loading and tracking. This is the default mode and also
        /// the easiest. Used best for duplex database access involving read, insert and update.
        /// </summary>
        AllOn = ContextModeFlags.Proxy | ContextModeFlags.Lazy | ContextModeFlags.ChangeTracking
    }
}
