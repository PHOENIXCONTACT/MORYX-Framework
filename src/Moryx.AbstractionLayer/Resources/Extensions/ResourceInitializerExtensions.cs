// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Resources
{
    /// <summary>
    /// Provides an API to generate fluent code for resource initializion by method chaining.
    /// </summary>
    public static class ResourceInitializerExtensions
    {
        /// <summary>
        /// Allows to configure a <see cref="Resource"/> with method chaining
        /// </summary>
        /// <param name="configurable">The resource to be configured</param>
        /// <param name="onConfigure">Action to configure the given `configurable`</param>
        /// <returns>The provided `configurable`</returns>
        public static TResult Configure<TResult>(this TResult configurable, Action<TResult> onConfigure)
        {
            onConfigure(configurable);
            return configurable;
        }

        /// <summary>
        /// Adds a <paramref name="child"/> resource to the <paramref name="parent"/> and returns the
        /// <paramref name="parent"/> to allow further operations on it.
        /// </summary>
        /// <typeparam name="TResource">Type of the parent <see cref="Resource"/></typeparam>
        /// <param name="parent">Parent resource</param>
        /// <param name="child">Child resource, that gets added to <paramref name="parent"/></param>
        /// <returns>The provided <paramref name="parent"/></returns>
        public static TResource AddChild<TResource>(this TResource parent, Resource child) where TResource : Resource
        {
            parent.Children.Add(child);
            return parent;
        }

        /// <summary>
        /// Adds a <paramref name="child"/> resource to the <paramref name="parent"/> and returns the
        /// <paramref name="parent"/> to allow further operations on it.
        /// Can be used to perform a custom <paramref name="action"/> within the method chain.
        /// </summary>
        /// <typeparam name="TResource">Type of the parent <see cref="Resource"/></typeparam>
        /// <typeparam name="TChildResource">Type of the child resource</typeparam>
        /// <param name="parent">Parent resource</param>
        /// <param name="child">Child resource, that gets added to <paramref name="parent"/></param>
        /// <param name="action">Custom action which provides <paramref name="parent"/> and <paramref name="child"/> as parameters</param>
        /// <returns>The provided <paramref name="parent"/></returns>
        public static TResource AddChild<TResource, TChildResource>(this TResource parent, TChildResource child, Action<TResource, TChildResource> action)
            where TResource : Resource
            where TChildResource : Resource
        {
            parent.Children.Add(child);
            action(parent, child);
            return parent;
        }

        /// <summary>
        /// Instantiates a <see cref="Resource"/> on a resource <paramref name="graph"/> by providing a <paramref name="name"/>
        /// and optionally a <paramref name="description"/>.
        /// </summary>
        /// <typeparam name="TResource">Type of the <see cref="Resource"/></typeparam>
        /// <param name="graph">Resource graph, on which the resource gets instantiated</param>
        /// <param name="name">Name of the resource</param>
        /// <param name="description">Description of the resource</param>
        /// <returns>The new resource</returns>
        public static TResource Instantiate<TResource>(this IResourceGraph graph, string name, string description = "") where TResource : Resource
        {
            return graph.Instantiate<TResource>()
                .Configure(r =>
                {
                    r.Name = name;
                    r.Description = description;
                });
        }
    }
}

