// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;

namespace Moryx.Configuration;

/// <summary>
/// Stackframe to keep track of a recursive calls to the value provider to provide context.
/// </summary>
/// <param name="parent">Object the property belongs to </param>
/// <param name="property">Can be null, if the parent is an enumerable. The context store will contain an "Index" key in that case</param>
/// <param name="contextStore">Can be used to store data to a stackframe that can be read from higher stackframes or can be used communicate between value providers</param>
public record ExecutorLevel(object parent, PropertyInfo property, Dictionary<string, object> contextStore);

/// <summary>
/// specialized value provider that receives a stack containing information about lower levels when the value provider is working recursively.
/// </summary>
/// <remarks>This is an experimental interface and might be removed with MORYX 12</remarks>
public interface IContextAwareValueProvider : IValueProvider
{
    /// <summary>
    /// Provides value to a property with a stack of the surrounding objects
    /// </summary>
    ValueProviderResult Handle(Stack<ExecutorLevel> levels);
}
