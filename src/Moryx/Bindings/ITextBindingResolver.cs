// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Bindings
{
    /// <summary>
    /// Interface for all components that can resolve bindings for a specific source string.
    /// </summary>
    public interface ITextBindingResolver
    {
        /// <summary>
        /// Resolve all bindings in the text and return a new object
        /// </summary>
        string Resolve(object source);
    }
}
