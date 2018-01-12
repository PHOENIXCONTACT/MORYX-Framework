using System;
using Effort.Provider;

namespace Marvin.Model.InMemory
{
    /// <summary>
    /// Base implementation of <see cref="IUnitOfWorkFactory"/> for InMemory databases with Effort
    /// </summary>
    public abstract class InMemoryUnitOfWorkFactoryBase<TContext> : UnitOfWorkFactoryBase<TContext>
        where TContext : MarvinDbContext
    {
        private readonly string _instanceId;

        /// <inheritdoc />
        protected InMemoryUnitOfWorkFactoryBase(string instanceId)
        {
            if (string.IsNullOrEmpty(instanceId))
                throw new ArgumentException("InstanceId should be not null or empty", nameof(instanceId));

            EffortProviderConfiguration.RegisterProvider();

            _instanceId = instanceId;
        }

        /// <inheritdoc />
        protected sealed override IModelConfigurator CreateConfigurator()
        {
            return null; // Configurator is not supported
        }

        /// <inheritdoc />
        protected override MarvinDbContext CreateContext(Type contextType, ContextMode contextMode)
        {
            var connection = string.IsNullOrEmpty(_instanceId)
                ? Effort.DbConnectionFactory.CreatePersistent(Guid.NewGuid().ToString())
                : Effort.DbConnectionFactory.CreatePersistent(_instanceId);

            return (MarvinDbContext)Activator.CreateInstance(contextType, connection, contextMode);
        }
    }
}