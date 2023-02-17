// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Bindings
{
    /// <summary>
    /// Factory to create <see cref="IBindingResolver"/> instances by a BaseKey
    /// found in the binding string.
    /// </summary>
    public interface IBindingResolverFactory
    {
        /// <summary>
        /// Create a property resolver for a binding string
        /// </summary>
        IBindingResolver Create(string bindingString);
    }
}
