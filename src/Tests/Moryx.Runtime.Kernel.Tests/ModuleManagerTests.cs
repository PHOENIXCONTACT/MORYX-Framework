// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moryx.Configuration;
using Moryx.Logging;
using Moryx.Runtime.Kernel.Tests.Dummies;
using Moryx.Runtime.Kernel.Tests.ModuleMocks;
using Moryx.Runtime.Modules;
using Moryx.Tools;
using NUnit.Framework;

namespace Moryx.Runtime.Kernel.Tests;

[TestFixture]
public class ModuleManagerTests
{
    private Mock<IConfigManager> _mockConfigManager;
    private Mock<IModuleLogger> _mockLogger;
    private ModuleManagerConfig _moduleManagerConfig;

    [SetUp]
    public void Setup()
    {
        _mockConfigManager = new Mock<IConfigManager>();
        _moduleManagerConfig = new ModuleManagerConfig { ManagedModules = [] };
        _mockConfigManager.Setup(mock => mock.GetConfiguration(typeof(ModuleManagerConfig), typeof(ModuleManagerConfig).FullName, false))
            .Returns(_moduleManagerConfig);
        _mockConfigManager.Setup(mock => mock.GetConfiguration(typeof(RuntimeConfigManagerTestConfig2), typeof(RuntimeConfigManagerTestConfig2).FullName, false))
            .Returns(new RuntimeConfigManagerTestConfig2());

        _mockLogger = new Mock<IModuleLogger>();
        _mockLogger.Setup(ml => ml.GetChild(It.IsAny<string>(), It.IsAny<Type>())).Returns(_mockLogger.Object);
    }

    private ModuleManager CreateObjectUnderTest(IServerModule[] modules)
    {
        return new ModuleManager(modules, _mockConfigManager.Object, new NullLogger<ModuleManager>());
    }

    private LifeCycleBoundFacadeTestModule CreateLifeCycleBoundFacadeTestModuleUnderTest()
    {
        return new LifeCycleBoundFacadeTestModule(new ModuleContainerFactory(), _mockConfigManager.Object, new NullLoggerFactory());
    }

    [Test]
    public async Task FacadeCollectionInjection()
    {
        // Arrange
        var dependent = new ModuleC();
        var moduleManager = CreateObjectUnderTest([
            new ModuleB1(),
            new ModuleB2(),
            new ModuleB3(),
            dependent
        ]);

        // Act
        await moduleManager.StartModulesAsync();

        // Assert
        Assert.That(dependent.Facades, Is.Not.Null, "No facade injected");
        Assert.That(dependent.Facades.Length, Is.EqualTo(3), "Faulty number of facades");
    }

    [Test]
    public async Task FacadeCollectionNoEntry()
    {
        // Arrange
        var dependent = new ModuleC();
        var moduleManager = CreateObjectUnderTest([
            dependent
        ]);

        // Act
        await moduleManager.StartModulesAsync();

        // Assert
        Assert.That(dependent.Facades, Is.Not.Null, "No facade injected");
        Assert.That(dependent.Facades.Length, Is.EqualTo(0), "Faulty number of facades");
    }

    [Test]
    public async Task FacadeCollectionSingleEntry()
    {
        // Arrange
        var dependent = new ModuleC();
        var moduleManager = CreateObjectUnderTest([
            new ModuleB1(),
            dependent
        ]);

        // Act
        await moduleManager.StartModulesAsync();

        // Assert
        Assert.That(dependent.Facades, Is.Not.Null, "No facade injected");
        Assert.That(dependent.Facades.Length, Is.EqualTo(1), "Faulty number of facades");
    }

    [Test]
    public async Task FacadeInjection()
    {
        // Arrange
        var dependency = new ModuleA();
        var depend = new ModuleADependent();
        var moduleManager = CreateObjectUnderTest([
            dependency,
            depend
        ]);

        // Act
        await moduleManager.StartModulesAsync();

        // Assert
        Assert.That(depend.Dependency, Is.Not.Null, "Facade not injected correctly");
    }

    [Test]
    public async Task ShouldExcludeMissingFacadeAndItsDependents()
    {
        // Arrange
        var moduleManager = CreateObjectUnderTest([
            new ModuleB1(),
            new ModuleCSingle(),
            new ModuleADependent(),
            new ModuleADependentTransient()
        ]);

        // Act
        await moduleManager.StartModulesAsync();

        // Assert
        Assert.That(moduleManager.AllModules.Count(), Is.EqualTo(4));
        var available = moduleManager.DependencyTree.RootModules
            .Flatten(md => md.Dependents).ToList();
        Assert.That(available.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task ShouldExcludeWhenInCollection()
    {
        // Arrange
        var moduleManager = CreateObjectUnderTest([
            new ModuleB1(),
            new ModuleBUsingA(),
            new ModuleC()
        ]);

        // Act
        await moduleManager.StartModulesAsync();

        // Assert
        Assert.That(moduleManager.AllModules.Count(), Is.EqualTo(3));
        var available = moduleManager.DependencyTree.RootModules
            .Flatten(md => md.Dependents).ToList();
        Assert.That(available.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task ShouldIncludeMissingFacadeInDependencyList()
    {
        // Arrange
        var moduleBUsingA = new ModuleBUsingA();
        var moduleManager = CreateObjectUnderTest([
            new ModuleB1(),
            moduleBUsingA,
            new ModuleC()
        ]);

        // Act
        await moduleManager.StartModulesAsync();

        // Assert
        Assert.That(moduleManager.AllModules.Count(), Is.EqualTo(3));
        var moduleBUsingADependencies = moduleManager.StartDependencies(moduleBUsingA);
        Assert.That(moduleBUsingADependencies.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task ShouldInitializeTheModule()
    {
        // Arrange
        var mockModule = new Mock<IServerModule>();
        var moduleManager = CreateObjectUnderTest([mockModule.Object]);

        // Act
        await moduleManager.InitializeModuleAsync(mockModule.Object);

        // Assert
        mockModule.Verify(mock => mock.InitializeAsync());
    }

    [Test]
    public async Task ShouldStartAllModules()
    {
        // Arrange
        var mockModule1 = new Mock<IServerModule>();
        var mockModule2 = new Mock<IServerModule>();

        var moduleManager = CreateObjectUnderTest([
            mockModule1.Object,
            mockModule2.Object
        ]);

        // Act
        await moduleManager.StartModulesAsync();

        await WaitForConditionAsync(() => mockModule2.Invocations.Any(i => i.Method.Name == nameof(IServerModule.StartAsync)));

        // Assert
        mockModule1.Verify(mock => mock.InitializeAsync(), Times.Once);
        mockModule1.Verify(mock => mock.StartAsync());

        mockModule2.Verify(mock => mock.InitializeAsync(), Times.Once);
        mockModule2.Verify(mock => mock.StartAsync());
    }

    [Test]
    public async Task ShouldStartOneModule()
    {
        // Arrange
        var mockModule = new Mock<IServerModule>();

        var moduleManager = CreateObjectUnderTest([mockModule.Object]);

        // Act
        await moduleManager.StartModuleAsync(mockModule.Object);

        await WaitForConditionAsync(() => mockModule.Invocations.Any(i => i.Method.Name == nameof(IServerModule.StartAsync)));

        // Assert
        mockModule.Verify(mock => mock.InitializeAsync());
        mockModule.Verify(mock => mock.StartAsync());
    }

    [Test]
    public async Task ShouldStopModulesAndDeregisterFromEvents()
    {
        // Arrange
        var mockModule1 = new Mock<IServerModule>();
        var mockModule2 = new Mock<IServerModule>();

        var moduleManager = CreateObjectUnderTest([mockModule1.Object, mockModule2.Object]);
        await moduleManager.StartModulesAsync();

        // Act
        await moduleManager.StopModulesAsync();

        // Assert
        mockModule1.Verify(mock => mock.StopAsync());
        mockModule2.Verify(mock => mock.StopAsync());
    }

    [Test]
    public void ShouldObserveModuleStatesAfterInitialize()
    {
        // Arrange
        var mockModule = new Mock<IServerModule>();
        var eventFired = false;

        var moduleManager = CreateObjectUnderTest([mockModule.Object]);
        moduleManager.ModuleStateChanged += (_, _) => eventFired = true;

        // Act
        mockModule.Raise(mock => mock.StateChanged += null, mockModule.Object, new ModuleStateChangedEventArgs());

        // Assert
        Assert.That(eventFired, "ModuleManager doesn't observe state changed events of modules.");
    }

    [Test]
    public async Task CheckLifeCycleBoundActivatedCountIs1()
    {
        // Arrange
        var module = CreateLifeCycleBoundFacadeTestModuleUnderTest();
        var moduleManager = CreateObjectUnderTest([module]);

        // Act
        await moduleManager.StartModulesAsync();

        await WaitForConditionAsync(() => module.State == ServerModuleState.Running);

        // Assert
        Assert.That(module.ActivatedCount, Is.EqualTo(1));
    }

    [Test]
    public async Task CheckLifeCycleBoundDeactivatedCountIs1()
    {
        // Arrange
        var module = CreateLifeCycleBoundFacadeTestModuleUnderTest();
        var moduleManager = CreateObjectUnderTest([module]);

        // Act
        await moduleManager.StartModulesAsync();

        await WaitForConditionAsync(() => module.State == ServerModuleState.Running);

        await moduleManager.StopModulesAsync();

        await WaitForConditionAsync(() => module.State == ServerModuleState.Stopped);

        // Assert
        Assert.That(module.ActivatedCount, Is.EqualTo(1));
    }

    [Test, Description("Setting a module behaviour should update the config and persist it")]
    public void BehaviourChangeSavesConfig()
    {
        // Arrange
        const string moduleName = "TestModule";
        const ModuleStartBehaviour targetBehaviour = ModuleStartBehaviour.Manual;

        var mockModule = new Mock<IServerModule>();
        mockModule.Setup(m => m.Name).Returns(moduleName);
        var moduleManager = CreateObjectUnderTest([mockModule.Object]);

        // Act
        var startBehaviour = moduleManager.BehaviourAccess<ModuleStartBehaviour>(mockModule.Object);
        startBehaviour.Behaviour = targetBehaviour;

        // Assert
        var managedModuleConfig = _moduleManagerConfig.ManagedModules.Single(m => m.ModuleName == moduleName);
        Assert.That(managedModuleConfig.StartBehaviour, Is.EqualTo(targetBehaviour));
        _mockConfigManager.Verify(cm => cm.SaveConfiguration(_moduleManagerConfig, It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
    }

    [Test(Description = "Dependent modules that were stopped during a dependency shutdown chain must be restarted when the root dependency is started again.")]
    public async Task RestartingDependencyRootRestartsDependentModule()
    {
        // Arrange
        var rootModule = new ServerModuleA(new ModuleContainerFactory(), _mockConfigManager.Object, new NullLoggerFactory());
        var dependentModule = new ServerModuleADependent(new ModuleContainerFactory(), _mockConfigManager.Object, new NullLoggerFactory());

        var moduleManager = CreateObjectUnderTest([rootModule, dependentModule]);

        // Start all modules
        await moduleManager.StartModulesAsync();

        await WaitForConditionAsync(() => rootModule.State == ServerModuleState.Running
                                          && dependentModule.State == ServerModuleState.Running);

        Assert.That(rootModule.State, Is.EqualTo(ServerModuleState.Running));
        Assert.That(dependentModule.State, Is.EqualTo(ServerModuleState.Running));

        var dependentInitializeCalls = dependentModule.InitializeCalls;
        var dependentStartCalls = dependentModule.StartCalls;

        // Act
        // Stop root module -> dependent must be stopped as well
        await moduleManager.StopModuleAsync(rootModule);

        Assert.That(rootModule.State, Is.EqualTo(ServerModuleState.Stopped));
        Assert.That(dependentModule.State, Is.EqualTo(ServerModuleState.Stopped));

        // Restart root module
        await moduleManager.StartModuleAsync(rootModule);

        await WaitForConditionAsync(() => rootModule.State == ServerModuleState.Running
                                          && dependentModule.State == ServerModuleState.Running);

        // Assert
        Assert.That(rootModule.State, Is.EqualTo(ServerModuleState.Running),
            "Root module was not restarted.");
        Assert.That(dependentModule.State, Is.EqualTo(ServerModuleState.Running),
            "Dependent module was not restarted.");
        Assert.That(dependentModule.InitializeCalls, Is.EqualTo(dependentInitializeCalls + 1),
            "Dependent module was not reinitialized.");
        Assert.That(dependentModule.StartCalls, Is.EqualTo(dependentStartCalls + 1),
            "Dependent module was not restarted.");
    }

    [Test(Description = "Reincarnating a module must restart dependent modules that were stopped during the shutdown sequence.")]
    public async Task ShouldReincarnateDependentModules()
    {
        // Arrange
        var rootModule = new ServerModuleA(new ModuleContainerFactory(), _mockConfigManager.Object, new NullLoggerFactory());
        var dependentModule = new ServerModuleADependent(new ModuleContainerFactory(), _mockConfigManager.Object, new NullLoggerFactory());

        var moduleManager = CreateObjectUnderTest([rootModule, dependentModule]);

        // Initial startup
        await moduleManager.StartModulesAsync();

        await WaitForConditionAsync(() => rootModule.State == ServerModuleState.Running
                                          && dependentModule.State == ServerModuleState.Running);

        Assert.That(rootModule.State, Is.EqualTo(ServerModuleState.Running));
        Assert.That(dependentModule.State, Is.EqualTo(ServerModuleState.Running));

        var initialInitializeCalls = dependentModule.InitializeCalls;
        var initialStartCalls = dependentModule.StartCalls;

        // Act
        await moduleManager.ReincarnateModuleAsync(rootModule);

        // Running again
        await WaitForConditionAsync(() => rootModule.State == ServerModuleState.Running
                                          && dependentModule.State == ServerModuleState.Running);

        // Assert
        Assert.That(rootModule.State, Is.EqualTo(ServerModuleState.Running),
            "Root module was not restarted.");
        Assert.That(dependentModule.State, Is.EqualTo(ServerModuleState.Running),
            "Dependent module was not restarted.");
        Assert.That(dependentModule.InitializeCalls, Is.EqualTo(initialInitializeCalls + 1),
            "Dependent module was not reinitialized.");
        Assert.That(dependentModule.StartCalls, Is.EqualTo(initialStartCalls + 1),
            "Dependent module was not restarted.");
    }

    [Test]
    public void ShouldIgnoreUnknownModuleOnStart()
    {
        // Arrange
        var knownModule = new Mock<IServerModule>();
        var unknownModule = new Mock<IServerModule>();
        var moduleManager = CreateObjectUnderTest([knownModule.Object]);

        // Act
        Assert.DoesNotThrowAsync(async () => await moduleManager.StartModuleAsync(unknownModule.Object));

        // Assert
        unknownModule.Verify(m => m.InitializeAsync(), Times.Never);
        unknownModule.Verify(m => m.StartAsync(), Times.Never);
    }

    [Test]
    public void ShouldIgnoreUnknownModuleOnStop()
    {
        // Arrange
        var knownModule = new Mock<IServerModule>();
        var unknownModule = new Mock<IServerModule>();
        var moduleManager = CreateObjectUnderTest([knownModule.Object]);

        // Act
        Assert.DoesNotThrowAsync(async () => await moduleManager.StopModuleAsync(unknownModule.Object));

        // Assert
        unknownModule.Verify(m => m.StopAsync(), Times.Never);
    }

    [Test]
    public void ShouldIgnoreUnknownModuleOnReincarnate()
    {
        // Arrange
        var knownModule = new Mock<IServerModule>();
        var unknownModule = new Mock<IServerModule>();
        var moduleManager = CreateObjectUnderTest([knownModule.Object]);

        // Act
        Assert.DoesNotThrowAsync(async () => await moduleManager.ReincarnateModuleAsync(unknownModule.Object));

        // Assert
        unknownModule.Verify(m => m.InitializeAsync(), Times.Never);
        unknownModule.Verify(m => m.StartAsync(), Times.Never);
        unknownModule.Verify(m => m.StopAsync(), Times.Never);
    }

    private static async Task WaitForConditionAsync(Func<bool> condition)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        while (!condition())
        {
            await Task.Delay(10, cts.Token);
        }
    }

    [Test]
    public async Task ShouldNotStartModuleWhenStoppedDuringInitialization()
    {
        // Arrange

        var initializeStarted = new TaskCompletionSource();
        var continueInitialize = new TaskCompletionSource();

        var mockModule = new Mock<IServerModule>();

        mockModule
            .Setup(m => m.InitializeAsync(It.IsAny<CancellationToken>()))
            .Returns(async (CancellationToken ct) =>
            {
                initializeStarted.SetResult();

                await continueInitialize.Task;
            });

        var manager = CreateObjectUnderTest([mockModule.Object]);

        // Act
        var startTask = manager.StartModulesAsync();

        await initializeStarted.Task;

        await manager.StopModulesAsync();

        continueInitialize.SetResult();

        await startTask;

        // Assert

        mockModule.Verify(
            m => m.StartAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task ShouldNotStartModuleAfterCancellation()
    {
        // Arrange

        var initializeEntered = new TaskCompletionSource();

        var initializeContinue = new TaskCompletionSource();

        var module = new Mock<IServerModule>();

        module.SetupGet(m => m.Name)
            .Returns("TestModule");

        module.Setup(m => m.InitializeAsync(It.IsAny<CancellationToken>()))
            .Returns(async (CancellationToken token) =>
            {
                initializeEntered.TrySetResult();

                await initializeContinue.Task;
            });

        var manager = CreateObjectUnderTest([module.Object]);

        using var cts = new CancellationTokenSource();

        // Act

        var startTask = manager.StartModulesAsync(cts.Token);

        await initializeEntered.Task;

        cts.Cancel();

        initializeContinue.TrySetResult();

        try
        {
            await startTask;
        }
        catch (OperationCanceledException)
        {
        }

        // Assert

        module.Verify(
            m => m.StartAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task ShouldNotStartBlockingModuleAfterCancellation()
    {
        // Arrange
        var initializeEntered = new TaskCompletionSource();
        var continueInitialize = new TaskCompletionSource();

        var module = new Mock<IServerModule>();

        module.Setup(m => m.InitializeAsync(It.IsAny<CancellationToken>()))
            .Returns(async () =>
            {
                initializeEntered.SetResult();
                await continueInitialize.Task;
            });

        module.Setup(m => m.StartAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var state = ServerModuleState.Initializing;

        module.SetupGet(m => m.State)
            .Returns(() => state);

        module.Setup(m => m.StopAsync(It.IsAny<CancellationToken>()))
            .Returns(() =>
            {
                // Shutdown wurde angefordert, aber noch nicht abgeschlossen
                state = ServerModuleState.Stopping;
                return Task.CompletedTask;
            });

        var manager = CreateObjectUnderTest([module.Object]);

        // Act
        var startTask = manager.StartModulesAsync();

        await initializeEntered.Task;

        // Während Initialize noch läuft
        await manager.StopModulesAsync();

        continueInitialize.SetResult();

        await startTask;

        // Assert
        module.Verify(
            m => m.StopAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        module.Verify(
            m => m.StartAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
