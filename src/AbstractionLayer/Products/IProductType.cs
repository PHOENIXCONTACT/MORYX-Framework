// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.AbstractionLayer.Identity;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Interface for all typed products
    /// </summary>
    public interface IProductType : IPersistentObject, IIdentifiableObject
    {
        /// <summary>
        /// Display name of this product
        /// </summary>
        string Name { get; }

        /// <summary>
        /// State of the product
        /// </summary>
        ProductTypeState State { get; }


        /// <summary>
        /// Create article instance of this type
        /// </summary>
        ProductInstance CreateInstance();
    }
}
