// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Moryx.AbstractionLayer.Products
{
    // TODO: AL6 Remove extensions
    /// <summary>
    /// Extensions for the <see cref="IProductManagement"/> facade
    /// </summary>
    public static class ProductFacadeExtensions
    {
        /// <summary>
        /// Bridge extension for LoadTypes using filter expression
        /// </summary>
        public static IReadOnlyList<TType> LoadTypes<TType>(this IProductManagement facade, Expression<Func<TType, bool>> selector)
        {
            return facade.LoadTypes(selector);
        }

        /// <summary>
        /// Bridge extension for DeleteProduct using an id
        /// </summary>
        public static bool DeleteProduct(this IProductManagement facade, long id)
        {
            return facade.DeleteProduct(id);
        }
    }
}