// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Processes;
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
        /// Modifies the <see cref="ProductInstance"/> of type <typeparamref name="TInstance"/>
        /// on the <see cref="IProcess"/> using the given <paramref name="setter"/>.
        /// </summary>
        /// <typeparam name="TInstance">The expected type of the product instance</typeparam>
        /// <param name="setter">The action to be executed on the product instance</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// process.ModifyProductInstance<MyProductInstance>((var instance) => instance.MyProperty = 1)
        /// ]]>
        /// </code>
        /// </example>
        /// <exception cref="InvalidCastException">Thrown if the given <paramref name="process"/> does
        /// not hold a product instance of type <typeparamref name="TInstance"/></exception>
        /// <exception cref="InvalidOperationException">Thrown if the given <paramref name="process"/>
        /// is no <see cref="ProductionProcess"/></exception>
        public TInstance ModifyProductInstance<TInstance>(Action<TInstance> setter)
            where TInstance : ProductInstance
        {
            if (process is not ProductionProcess productionProcess)
                throw new InvalidOperationException($"Cannot modify an {nameof(ProductInstance)} on a process of type {process.GetType()}");
            if (productionProcess.ProductInstance is not TInstance instance)
                throw new InvalidCastException($"Cannot cast {nameof(ProductionProcess.ProductInstance)} of type " +
                                               $"{productionProcess?.ProductInstance?.GetType()} to {typeof(TInstance)}");
            setter.Invoke(instance);
            return instance;
        }

        /// <summary>
        /// Tries to modifies the <see cref="ProductInstance"/> of type <typeparamref name="TInstance"/>
        /// on the <see cref="IProcess"/> using the given <paramref name="setter"/>. Returns false, if the
        /// operation could not be executed.
        /// </summary>
        /// <typeparam name="TInstance">The expected type of the product instance</typeparam>
        /// <param name="setter">The action to be executed on the product instance</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// process.TryModifyingProductInstance<MyProductInstance>((var instance) => instance.MyProperty = 1)
        /// ]]>
        /// </code>
        /// </example>
        public bool TryModifyProductInstance<TInstance>(Action<TInstance> setter)
            where TInstance : ProductInstance
        {
            if (process is not ProductionProcess productionProcess)
                return false;
            if (productionProcess.ProductInstance is not TInstance instance)
                return false;
            setter.Invoke(instance);
            return true;
        }

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