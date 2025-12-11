# Setup a test environment for integration tests of a module

In order to test a module in its lifecycle with its respective facade we offer the `Moryx.TestTools.IntegrationTest`.
The package brings a `MoryxTestEnvironment<T>`.
With this class you can first create mocks for all module facades your module dependents on using the static `CreateModuleMock<FacadeType>` method.
Afterwards you can create the environment using an implementation of the `ServerModuleBase<T>` class, an instance of the `ConfigBase` and the set of dependency mocks.
The first two parameters are usually your `ModuleController` and your `ModuleConfig`.
The following example shows a setup for the `IShiftManagement` facade interface. The module depends on the `IResourceManagement` and `IOperatorManagement` facades.

```csharp
private ModuleConfig _config;
private Mock<IResourceManagement> _resourceManagementMock;
private Mock<IOperatorManagement> _operatorManagementMock;
private MoryxTestEnvironment _env;

[SetUp]
public void SetUp()
{
    ReflectionTool.TestMode = true;
    _config = new();
    _resourceManagementMock = MoryxTestEnvironment.CreateModuleMock<IResourceManagement>();
    _operatorManagementMock = MoryxTestEnvironment.CreateModuleMock<IOperatorManagement>();
    _env = new MoryxTestEnvironment(typeof(ModuleController),
        new Mock[] { _resourceManagementMock, _operatorManagementMock }, _config);
}
```

Using the created environment you can start and stop the module as you please.
You can also retrieve the facade of the module to test all the functionalities the running module should provide.

```csharp
[Test]
public async Task Start_WhenModuleIsStopped_StartsModule()
{
    // Arrange
    var facade = _env.GetTestModule();

    // Act
    var module = await _env.StartTestModuleAsync();
    var module = await _env.StopTestModuleAsync();

    // Assert
    Assert.That(module.State, Is.EqualTo(ServerModuleState.Stopped));
}
```
