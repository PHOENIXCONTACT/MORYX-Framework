// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Tools;
using System.Reflection;

namespace Moryx.Container;

/// <summary>
/// Extension to replace load components on <see cref="IContainer"/>
/// </summary>
public static class ContainerLoadComponentsExtension
{
    /// <summary>
    /// Load all types from an assembly
    /// </summary>
    public static void LoadFromAssembly(this IContainer container, Assembly assembly)
    {
        container.LoadFromAssembly(assembly, t => true);
    }

    /// <summary>
    /// Load types from assembly filtered by dependency attribute
    /// </summary>
    public static void LoadFromAssembly(this IContainer container, Assembly assembly, DependencyRegistrationAttribute att)
    {
        container.LoadFromAssembly(assembly, type => TypeRequired(att, type));
    }

    /// <summary>
    /// Load filtered types from assembly
    /// </summary>
    public static void LoadFromAssembly(this IContainer container, Assembly assembly, Predicate<Type> predicate)
    {
        // Install all components
        foreach (var type in assembly.GetTypes())
        {
            // Register all we want
            if (ShallInstall(type) && predicate(type))
                container.Register(type);
        }
    }

    /// <summary>
    /// Method to determine if this component shall be installed
    /// </summary>
    private static bool ShallInstall(Type foundType)
    {
        var regAtt = foundType.GetCustomAttribute<ComponentAttribute>();
        var facAtt = foundType.GetCustomAttribute<PluginFactoryAttribute>();

        return (regAtt != null || facAtt != null);
    }

    private static bool TypeRequired(DependencyRegistrationAttribute att, Type foundType)
    {
        if (att.InstallerMode == InstallerMode.All)
            return true;

        // Check if interface was specified as required
        if (foundType.IsInterface && att.RequiredTypes.Contains(foundType))
            return true;

        // Check if class exports required type
        var services = ContainerRegistrationExtensions.GetComponentServices(foundType);
        if (foundType.IsClass && att.RequiredTypes.Intersect(services).Any())
            return true;

        return false;
    }

    /// <summary>
    /// Load all implementations of type from currently known types
    /// KnownTypes: Types in default framework folders and deeper.
    /// </summary>
    public static void LoadComponents<T>(this IContainer container) where T : class
    {
        LoadComponents<T>(container, null);
    }

    /// <summary>
    /// Loads all implementations of type from the currently known types
    /// KnownTypes: Types in default framework folders and deeper.
    /// </summary>
    public static IContainer LoadComponents<T>(this IContainer container, Predicate<Type> condition) where T : class
    {
        foreach (var type in ReflectionTool.GetPublicClasses<T>(LoadComponentCandidate))
        {
            if (condition?.Invoke(type) ?? true)
            {
                container.Register(type);

                RegisterAdditionalDependencies(container, type);
            }
        }

        return container;
    }

    private static bool LoadComponentCandidate(Type type)
    {
        if (type.Assembly.GetCustomAttribute<ComponentLoaderIgnoreAttribute>() != null)
            return false;

        if (type.GetCustomAttribute<ComponentAttribute>(true) == null)
            return false;

        return true;
    }

    private static void RegisterAdditionalDependencies(IContainer container, Type implementation)
    {
        var att = implementation.GetCustomAttribute<DependencyRegistrationAttribute>();
        if (att == null)
            return;

        container.LoadFromAssembly(implementation.Assembly, att);

        if (att.Initializer == null)
            return;

        if (!typeof(ISubInitializer).IsAssignableFrom(att.Initializer))
            throw new InvalidCastException($"SubInitializer {att.Initializer.Name} of component {implementation.Name} does not implement interface ISubInitializer");

        container.Register(att.Initializer, [typeof(ISubInitializer), att.Initializer], null, LifeCycle.Singleton);
    }
}