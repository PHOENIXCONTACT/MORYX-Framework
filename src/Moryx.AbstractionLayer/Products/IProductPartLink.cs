// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Products
{
    /// <summary>
    /// Extension to instantiate instance collection from product type parts collection
    /// </summary>
    public static class PartLinkExtension
    {
        /// <summary>
        /// Instantiate product instance collection
        /// </summary>
        public static List<TInstance> Instantiate<TInstance>(this IEnumerable<ProductPartLink> parts)
            where TInstance : ProductInstance
        {
            return parts.Select(p => (TInstance)p.Instantiate()).ToList();
        }
    }
}
