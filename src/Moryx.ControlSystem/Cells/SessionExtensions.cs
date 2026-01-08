// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Products;
using Moryx.ControlSystem.Recipes;
using Moryx.ControlSystem.Processes;

namespace Moryx.ControlSystem.Cells;

/// <summary>
/// Extension methods on the <see cref="Session"/> to get product related information, activity details or just provide shortcuts based on the actual session type
/// </summary>
public static class SessionExtensions
{
    /// <param name="session">The sesion to get the <see cref="ProductInstance"/> from</param>
    extension(Session session)
    {
        /// <summary>
        /// Extension method to get the <see cref="ProductInstance"/> from the <see cref="Process"/> of the <paramref name="session"/>
        /// </summary>
        /// <typeparam name="TProductInstance">Type of the <see cref="ProductInstance"/> that is expected.</typeparam>
        /// <returns>
        /// The <see cref="ProductInstance"/> in the session, if the <paramref name="session"/> belongs to a
        /// <see cref="ProductionProcess"/> and the <see cref="ProductionProcess"/> holds a <typeparamref name="TProductInstance"/>;
        /// Otherwise returns null
        /// </returns>
        public TProductInstance GetProductInstance<TProductInstance>() where TProductInstance : ProductInstance
        {
            if (session.Process is not ProductionProcess process) return null;

            return process.ProductInstance as TProductInstance;
        }

        /// <summary>
        /// Modifies the <see cref="ProductInstance"/> of type <typeparamref name="TInstance"/>
        /// on the <see cref="Process"/> of the <paramref name="session"/> using the given
        /// <paramref name="setter"/>.
        /// </summary>
        /// <typeparam name="TInstance">The expected type of the product instance</typeparam>
        /// <param name="setter">The action to be executed on the product instance</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// session.ModifyProductInstance<MyProductInstance>((var instance) => instance.MyProperty = 1)
        /// ]]>
        /// </code>
        /// </example>
        /// <exception cref="InvalidCastException">Thrown if the <see cref="Process"/> of the
        /// <paramref name="session"/> does not hold a product instance of type <typeparamref name="TInstance"/>
        /// </exception>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="Process"/> of the
        /// <paramref name="session"/> is no <see cref="ProductionProcess"/></exception>
        public TInstance ModifyProductInstance<TInstance>(Action<TInstance> setter)
            where TInstance : ProductInstance => session.Process.ModifyProductInstance(setter);

        /// <summary>
        /// Tries to modifies the <see cref="ProductInstance"/> of type <typeparamref name="TInstance"/>
        /// on the <see cref="Process"/> of the <paramref name="session"/> using the given
        /// <paramref name="setter"/>. Returns false, if the
        /// operation could not be executed.
        /// </summary>
        /// <typeparam name="TInstance">The expected type of the product instance</typeparam>
        /// <param name="setter">The action to be executed on the product instance</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// session.TryModifyingProductInstance<MyProductInstance>((var instance) => instance.MyProperty = 1)
        /// ]]>
        /// </code>
        /// </example>
        public bool TryModifyProductInstance<TInstance>(Action<TInstance> setter)
            where TInstance : ProductInstance => session.Process.TryModifyProductInstance(setter);

        /// <summary>
        /// Extension method to get the <see cref="Activity"/> from the <paramref name="session"/>
        /// </summary>
        /// <typeparam name="TActivityType">Type of the <see cref="Activity"/> that is expected.</typeparam>
        /// <returns>
        /// The <see cref="Activity"/> in the session, if the <paramref name="session"/> currently handles an
        /// Activity of type <typeparamref name="TActivityType"/>; Otherwise returns null
        /// </returns>
        public TActivityType GetActivity<TActivityType>() where TActivityType : Activity
        {
            if (session is ActivityCompleted completed)
                return completed.CompletedActivity as TActivityType;
            if (session is ActivityStart start)
                return start.Activity as TActivityType;

            return null;
        }

        /// <summary>
        /// Extension method to get the last <see cref="Activity"/> from the <paramref name="session"/>
        /// </summary>
        /// <typeparam name="TActivityType">Type of the <see cref="Activity"/> that is expected.</typeparam>
        /// <returns>
        /// The last <see cref="Activity"/> in the session, if the <paramref name="session"/> currently handles an
        /// Activity of type <typeparamref name="TActivityType"/>; Otherwise returns null
        /// </returns>
        public TActivityType GetLastActivity<TActivityType>() where TActivityType : Activity
            => session.Process.LastActivity<TActivityType>() as TActivityType;

        /// <summary>
        /// Extension method to get the <see cref="ProductType"/> from the <paramref name="session"/>
        /// </summary>
        /// <typeparam name="TProductType">Type of the <see cref="ProductType"/> that is expected.</typeparam>
        /// <returns>
        /// The target <see cref="ProductType"/> in the session, if it belongs to a <see cref="ProductionProcess"/>
        /// or holds an <see cref="SetupRecipe"/> with a <typeparamref name="TProductType"/>; Otherwise returns null.
        /// </returns>
        public TProductType GetProductType<TProductType>() where TProductType : ProductType
        {
            if (session.Process.Recipe is SetupRecipe setupRecipe)
                return setupRecipe.TargetRecipe.Target as TProductType;

            if (session.Process.Recipe is IProductRecipe prodcutRecipe)
                return prodcutRecipe.Target as TProductType;

            return default;
        }
    }
}