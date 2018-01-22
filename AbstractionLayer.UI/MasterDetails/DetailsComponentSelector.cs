using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel;
using Marvin.Container;

namespace Marvin.AbstractionLayer.UI
{
    /// <summary>
    /// Component selector for specialized plugins defined by runtime data
    /// </summary>
    public abstract class DetailsComponentSelector<TDetailsType, TController> : DefaultTypedFactoryComponentSelector
        where TDetailsType : class
        where TController : IInteractionController
    {
        /// <summary>
        /// Controller to access the web server
        /// </summary>
        protected TController Controller { get; }

        /// <summary>
        /// Registrations of view models for different types
        /// </summary>
        protected Dictionary<string, string> Registrations { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailsComponentSelector{TDetailsType,TController}"/> class.
        /// </summary>
        protected DetailsComponentSelector(IContainer container, TController controller)
        {
            Controller = controller;

            Registrations = (from type in container.GetRegisteredImplementations(typeof(TDetailsType))
                let att = type.GetCustomAttribute<DetailsRegistrationAttribute>()
                where att != null
                select new
                {
                    GroupName = att.TypeName,
                    PluginType = type.FullName
                }).ToDictionary(e => e.GroupName, e => e.PluginType);
        }

        ///
        protected override string GetComponentName(MethodInfo method, object[] arguments)
        {
            var groupType = arguments.FirstOrDefault() as string;
            return groupType != null && Registrations.ContainsKey(groupType)
                ? Registrations[groupType]
                : Registrations[DetailsConstants.DefaultType];
        }

        ///
        protected override Func<IKernelInternal, IReleasePolicy, object> BuildFactoryComponent(MethodInfo method, string componentName, Type componentType, IDictionary additionalArguments)
        {
            return delegate (IKernelInternal @internal, IReleasePolicy policy)
            {
                var component = base.BuildFactoryComponent(method, componentName, componentType, additionalArguments)(@internal, policy);

                var baseViewModel = component as IDetailsViewModel;
                if (baseViewModel != null)
                    baseViewModel.Initialize(Controller, componentName);

                return component;
            };
        }
    }
}