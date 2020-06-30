// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Products
{
    /// <summary>
    /// Interface for prototype parameters
    /// </summary>
    public interface IPrototypeParameters : IImportParameters
    {
        /// <summary>
        /// Identifier of the new product
        /// </summary>
        string Identifier { get; set; }
        /// <summary>
        /// Revision of the new product
        /// </summary>
        short Revision { get; set; }
        /// <summary>
        /// Name of the new product
        /// </summary>
        string Name { get; set; }
    }
}
