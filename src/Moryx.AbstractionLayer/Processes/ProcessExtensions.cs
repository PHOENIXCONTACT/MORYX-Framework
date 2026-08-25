// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;

namespace Moryx.AbstractionLayer.Processes;

/// <summary>
/// Extensions for <see cref="IProcess"/>
/// </summary>
public static class ProcessExtensions
{
    /// <param name="process">Extended instance of <see cref="IProcess"/></param>
    extension(IProcess process)
    {
        #region Product Types

        /// <summary>
        /// Returns the <see cref="ProductType"/> or null if <paramref name="process"/> is not a 
        /// <see cref="ProductionProcess"/> or does not hold a <see cref="ProductionProcess.ProductInstance"/>        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// var productType = process.GetProductType();
        /// ]]>
        /// </code>
        /// </example>
        public ProductType GetProductType()
        {
            if (process.Recipe is IProductRecipe prodcutRecipe)
            {
                return prodcutRecipe.Target;
            }

            return default;
        }

        /// <summary>
        /// Returns the <see cref="ProductType"/> of type <typeparamref name="TType"/> or null if 
        /// <paramref name="process"/> is not a <see cref="ProductionProcess"/>, does not hold a 
        /// <see cref="ProductionProcess.ProductInstance"/>, or its product type does not implement 
        /// <typeparamref name="TType"/>
        /// </summary>
        /// <typeparam name="TType">The expected type of the product type</typeparam>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// var myType = process.GetProductType<MyProductType>();
        /// ]]>
        /// </code>
        /// </example>
        public TType GetProductType<TType>() where TType : ProductType => process.GetProductType() as TType;

        #endregion

        #region Product Instances

        /// <summary>
        /// Returns the <see cref="ProductInstance"/> or null if <see cref="ProductionProcess.ProductInstance"/> does 
        /// not implement the specified type.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// var productInstance = process.GetProductInstance();
        /// ]]>
        /// </code>
        /// </example>
        public ProductInstance GetProductInstance()
        {
            if (process is not ProductionProcess productionProcess)
            {
                return null;
            }

            return productionProcess.ProductInstance;
        }

        /// <summary>
        /// Returns the <see cref="ProductInstance"/> of type <typeparamref name="TInstance"/> or null if 
        /// <see cref="ProductionProcess.ProductInstance"/> does not implement the specified type.
        /// </summary>
        /// <typeparam name="TInstance">The expected type of the product instance</typeparam>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// var myInstance = process.GetProductInstance<MyProductInstance>();
        /// ]]>
        /// </code>
        /// </example>
        public TInstance GetProductInstance<TInstance>() where TInstance : ProductInstance
        {
            if (process is not ProductionProcess productionProcess)
            {
                return null;
            }

            if (productionProcess.ProductInstance is not TInstance instance)
            {
                return null;
            }

            return instance;
        }

        /// <summary>
        /// Modifies the <see cref="ProductInstance"/> of type <typeparamref name="TInstance"/>
        /// on the <see cref="IProcess"/> using the given <paramref name="setter"/>.
        /// </summary>
        /// <typeparam name="TInstance">The expected type of the product instance</typeparam>
        /// <param name="setter">The action to be executed on the product instance</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// process.Modify<MyProductInstance>((var instance) => instance.MyProperty = 1);
        /// ]]>
        /// </code>
        /// </example>
        /// <exception cref="InvalidCastException">Thrown if the given <paramref name="process"/> does
        /// not hold a product instance of type <typeparamref name="TInstance"/></exception>
        /// <exception cref="InvalidOperationException">Thrown if the given <paramref name="process"/>
        /// is no <see cref="ProductionProcess"/></exception>
        public TInstance Modify<TInstance>(Action<TInstance> setter) where TInstance : ProductInstance
        {
            if (process is not ProductionProcess productionProcess)
            {
                throw new InvalidOperationException($"Cannot modify an {nameof(ProductInstance)} on a process of type {process.GetType()}");
            }

            if (productionProcess.ProductInstance is not TInstance instance)
            {
                throw new InvalidCastException($"Cannot cast {nameof(ProductionProcess.ProductInstance)} of type "
                    + $"{productionProcess?.ProductInstance?.GetType()} to {typeof(TInstance)}");
            }

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
        /// process.TryModify<MyProductInstance>((var instance) => instance.MyProperty = 1);
        /// ]]>
        /// </code>
        /// </example>
        public bool TryModify<TInstance>(Action<TInstance> setter) where TInstance : ProductInstance
        {
            if (process is not ProductionProcess productionProcess)
            {
                return false;
            }

            if (productionProcess.ProductInstance is not TInstance instance)
            {
                return false;
            }

            setter.Invoke(instance);
            return true;
        }

        #endregion

        #region Activities

        /// <summary>
        /// Get one prepared activity that will be dispatched as soon as a ready to work was send.
        /// Mention that, in case of parallel path in a workplan, a process could have multiple prepared activities!
        /// See also: <seealso cref="NextActivities"/>
        /// </summary>
        /// <returns>Last activity of the process that is prepared</returns>
        public Activity NextActivity()
        {
            return process.GetActivity(ActivitySelectionType.LastOrDefault, activity => activity.Tracing?.Started == null);
        }

        /// <summary>
        /// Get all prepared activities that will be dispatched as soon as a ready to work was send.
        /// </summary>
        public IEnumerable<Activity> NextActivities()
        {
            return process.GetActivities(activity => activity.Tracing?.Started == null);
        }

        /// <summary>
        /// Get one of the current running activities of the process.
        /// Mention that, in case of parallel path in a workplan, a process could have multiple running activities!
        /// See also: <seealso cref="CurrentActivities"/>
        /// </summary>
        /// <returns>Last activity of the process that is running</returns>
        public Activity CurrentActivity()
        {
            return process.GetActivity(ActivitySelectionType.LastOrDefault, activity => activity.Tracing?.Started != null && activity.Result == null);
        }

        /// <summary>
        /// Get all current running activities of the process.
        /// </summary>
        public IEnumerable<Activity> CurrentActivities()
        {
            return process.GetActivities(activity => activity.Tracing?.Started != null && activity.Result == null);
        }

        /// <summary>
        /// Get last completed activity
        /// </summary>
        public Activity LastActivity()
        {
            return process.GetActivity(ActivitySelectionType.LastOrDefault, a => a.Result != null);
        }

        /// <summary>
        /// Get last activity of a certain type
        /// </summary>
        public Activity LastActivity(string typeName)
        {
            return process.GetActivity(ActivitySelectionType.LastOrDefault, a => a.GetType().Name == typeName);
        }

        /// <summary>
        /// Gets the last activity of a certain type. Derived types are also considered.
        /// Use <see cref="LastActivity{TActivity}(IProcess, bool)" /> if the exact type is needed.
        /// </summary>
        /// <typeparam name="TActivity">Type of the activity</typeparam>
        public Activity LastActivity<TActivity>() where TActivity : IActivity
        {
            return process.LastActivity<TActivity>(false);
        }

        /// <summary>
        /// Gets the last activity of a certain type.
        /// If exact parameter is set to <c>true</c> only the exact type will be considered.
        /// </summary>
        /// <typeparam name="TActivity">Type of the activity</typeparam>
        /// <param name="exact">If <c>true</c> only the exact type will be considered.</param>
        public Activity LastActivity<TActivity>(bool exact) where TActivity : IActivity
        {
            return process.GetActivity(ActivitySelectionType.LastOrDefault, a => !exact && a is TActivity || exact && a.GetType() == typeof(TActivity));
        }

        #endregion
    }
}
