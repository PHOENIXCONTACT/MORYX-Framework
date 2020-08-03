using System.Data.Entity.Infrastructure;
using Moryx.Container;

namespace Moryx.Model
{
    /// <summary>
    /// Extension to activate database access in the local container
    /// </summary>
    public static class DbContextContainerExtension
    {
        /// <summary>
        /// Register <see cref="IDbContextManager"/> and <see cref="IContextFactory{TContext}"/>
        /// </summary>
        public static void ActivateDbContexts(this IContainer container, IDbContextManager contextManager)
        {
            container.SetInstance(contextManager);

            container.ExecuteInstaller(new ContextFactoryInstaller());
        }

        private class ContextFactoryInstaller : IContainerInstaller
        {
            public void Install(IComponentRegistrator registrator)
            {
                registrator.Register(typeof(ContextFactory<>), new []{ typeof(IContextFactory<>) }, "GenericContextFactory", LifeCycle.Singleton);
            }
        }
    }
}