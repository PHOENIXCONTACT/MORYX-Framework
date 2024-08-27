// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;

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