﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
using Moryx.Configuration;
using Moryx.Model.InMemory;
using Moryx.Model;
using Moryx.Runtime.Kernel;
using Moryx.Runtime.Modules;
using Moryx.TestTools.UnitTest;
using Moryx.Threading;
using System;
using System.Linq;
using Moryx.Tools;
using System.Collections.Generic;

namespace Moryx.TestTools.IntegrationTest
{
    /// <summary>
    /// A test environment for MORYX modules to test the module lifecycle as well as its 
    /// facade and component orchestration. The environment must be filled with mocked 
    /// dependencies.
    /// </summary>
    /// <typeparam name="T">Type of the facade to be tested.</typeparam>
    public class MoryxTestEnvironment
    {
        private readonly Type _moduleType;

        public IServiceProvider Services { get; private set; }

        /// <summary>
        /// Creates an <see cref="IServiceProvider"/> for integration tests of moryx. We prepare the 
        /// service collection to hold all kernel components (a mocked IConfigManager providing only the <paramref name="config"/>, 
        /// <see cref="NotSoParallelOps"/>, an <see cref="InMemoryDbContextManager"/>, a <see cref="NullLoggerFactory"/> and the 
        /// <see cref="ModuleManager"/>). Additionally all provided mocks are registered as moryx modules.
        /// </summary>
        /// <param name="serverModuleType">Type of the ModuleController of the module to be tested</param>
        /// <param name="dependencyMocks">An enumeration of mocks for all dependencies of the module to be tested. 
        /// We recommend using the <see cref="CreateModuleMock{T}"/> method to properly create the mocks.</param>
        /// <param name="config">The config for the module to be tested.</param>
        /// <exception cref="ArgumentException">Throw if <paramref name="serverModuleType"/> is not a server module</exception>
        public MoryxTestEnvironment(Type serverModuleType, IEnumerable<Mock> dependencyMocks, ConfigBase config)
        {
            _moduleType = serverModuleType;

            if (!serverModuleType.IsAssignableTo(typeof(IServerModule)))
                throw new ArgumentException("Provided parameter is no server module", nameof(serverModuleType));

            var dependencyTypes = serverModuleType.GetProperties()
                .Where(p => p.GetCustomAttribute<RequiredModuleApiAttribute>() is not null)
                .Select(p => p.PropertyType);

            var services = new ServiceCollection();
            foreach (var type in dependencyTypes)
            {
                var mock = dependencyMocks.SingleOrDefault(m => type.IsAssignableFrom(m.Object.GetType())) ??
                    throw new ArgumentException($"Missing {nameof(Mock)} for dependency of type {type} of facade type {serverModuleType}", nameof(dependencyMocks));
                services.AddSingleton(type, mock.Object);
                services.AddSingleton(typeof(IServerModule), mock.Object);
            }

            services.AddMoryxKernel();
            var configManagerMock = new Mock<IConfigManager>();
            configManagerMock.Setup(c => c.GetConfiguration(config.GetType(), It.IsAny<string>(), false)).Returns(config);
            services.AddSingleton(configManagerMock.Object);

            var parallelOpsDescriptor = services.Single(d => d.ServiceType == typeof(IParallelOperations));
            services.Remove(parallelOpsDescriptor);
            services.AddTransient<IParallelOperations, NotSoParallelOps>();
            services.AddSingleton<IDbContextManager>(new InMemoryDbContextManager(Guid.NewGuid().ToString()));
            services.AddSingleton<ILoggerFactory>(new NullLoggerFactory());
            services.AddSingleton(new Mock<ILogger<ModuleManager>>().Object);
            services.AddMoryxModules();

            Services = services.BuildServiceProvider();
            _ = Services.GetRequiredService<IModuleManager>();
        }

        /// <summary>
        /// Creates a mock of a server module with a facade interface of type <typeparamref name="T"/>.
        /// The mock can be used in setting up a service collection for test purposes.
        /// </summary>
        /// <typeparam name="T">Type of the facade interface</typeparam>
        /// <returns>The mock of the <typeparamref name="FacadeType"/></returns>
        public static Mock<FacadeType> CreateModuleMock<FacadeType>() where FacadeType : class
        {
            var mock = new Mock<FacadeType>();
            var moduleMock = mock.As<IServerModule>();
            moduleMock.SetupGet(m => m.State).Returns(ServerModuleState.Running);
            var containerMock = moduleMock.As<IFacadeContainer<FacadeType>>();
            containerMock.SetupGet(x => x.Facade).Returns(mock.Object);
            return mock;
        }

        /// <summary>
        /// Initializes and starts the module with the facade interface of type
        /// <typeparamref name="T"/>.
        /// </summary>
        /// <returns>The started module.</returns>
        public IServerModule StartTestModule()
        {
            var module = (IServerModule)Services.GetService(_moduleType);

            module.Initialize();
            module.Container.Register(typeof(NotSoParallelOps), [typeof(IParallelOperations)], nameof(NotSoParallelOps), Container.LifeCycle.Singleton);

            var strategies = module.GetType().GetProperty(nameof(ServerModuleBase<ConfigBase>.Strategies)).GetValue(module) as Dictionary<Type, string>;
            if (strategies is not null && !strategies.Any(s => s.Value == nameof(NotSoParallelOps)))
                strategies.Add(typeof(IParallelOperations), nameof(NotSoParallelOps));

            module.Start();
            return module;
        }

        /// <summary>
        /// Stops the module with the facade interface of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>The stopped module.</returns>
        public IServerModule StopTestModule()
        {
            var module = (IServerModule)Services.GetService(_moduleType);
            module.Stop();

            return module;
        }

        /// <summary>
        /// Returns the service for the facade of type <typeparamref name="T"/> to be tested.
        /// </summary>
        public TModule GetTestModule<TModule>() => Services.GetRequiredService<TModule>();
    }
}