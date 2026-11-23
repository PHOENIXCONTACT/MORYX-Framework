// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Products;
using Moryx.ControlSystem.Recipes;

namespace Moryx.ControlSystem.Processes;

/// <summary>
/// Extensions on an <see cref="IProcess"/>
/// </summary>
public static class IProcessExtensions
{
    /// <param name="process">The process holding the product instance</param>
    extension(IProcess process)
    {
        /// <summary>
        /// Returns <see cref="IOrderBasedRecipe.OrderNumber"/> on the <see cref="IProcess"/> using the given <paramref name="process"/>.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// process.GetOrderNumber()
        /// ]]>
        /// </code>
        /// </example>
        public string GetOrderNumber()
        {
            return (process.Recipe as IOrderBasedRecipe)?.OrderNumber;
        }

        /// <summary>
        /// Returns <see cref="IOrderBasedRecipe.OperationNumber"/> on the <see cref="IProcess"/> using the given <paramref name="process"/>.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// process.GetOperationNumber()
        /// ]]>
        /// </code>
        /// </example>
        public string GetOperationNumber()
        {
            return (process.Recipe as IOrderBasedRecipe)?.OperationNumber;
        }
    }
}
