// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Properties;

namespace Moryx.AbstractionLayer.Products
{
    /// <summary>
    /// Exception thrown when a product for a certain id was not found
    /// </summary>
    public class ProductNotFoundException : Exception
    {
        /// <summary>
        /// Initialize exception with database id
        /// </summary>
        /// <param name="id">Id that was not found</param>
        public ProductNotFoundException(long id)
            : base(string.Format(Strings.ProductNotFoundException_Message, id))
        {
        }
    }
}