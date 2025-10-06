// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Products.Management
{
    internal interface IConfiguredTypesProvider
    {
        /// <summary>
        /// Returns all currently configuret recipe types
        /// </summary>
        public IReadOnlyList<Type> RecipeTypes { get; }

        /// <summary>
        /// Returns all currently configuret recipe types
        /// </summary>
        public IReadOnlyList<Type> ProductTypes { get; }
    }
}