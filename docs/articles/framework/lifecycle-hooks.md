# Add Lifecycle Hooks to your application

Lifecycle hooks are a standardized and extensible mechanism to execute custom logic before the MORYX modules of your application are launched.
They are designed to enable reusable, composable startup tasks that help ensure your application starts in a predictable and controlled state.

Originally, lifecycle hooks were introduced to simplify manual testing scenarios by preparing the application with predefined data or state.
However, their usage extends far beyond testing — they are equally useful for setting up infrastructure, validating external dependencies, or initializing required resources.

## Definition

Lifecycle hooks are lightweight components that implement the `ILifecycleHook` interface.

Each hook defines:

- **Priority** – Determines execution order. Hooks with lower priority values are typically executed first.
- **RunAsync** – The asynchronous method where the hook's logic is executed.

This simple contract allows hooks to remain focused, easy to implement, and highly reusable.

## Modify the `Program.cs`

To enable lifecycle hooks in your application, you need to:

1. Register all hook implementations in the DI container
2. Execute them before starting the MORYX modules

The `Moryx.Runtime.Kernel` package provides convenient extension methods to simplify this setup.

```csharp
var builder = WebApplication.CreateBuilder();

// Register application services
builder.Services.AddMoryxLifecycleHooks();

var app = builder.Build();

// Configure middleware, endpoints, etc.

// Execute all registered lifecycle hooks
await app.Services.RunMoryxLifecycleHooksAsync();

app.Run();
```

### What happens under the hood?

- `AddMoryxLifecycleHooks()` scans for all implementations of `ILifecycleHook` and registers them in the service collection.
- `RunMoryxLifecycleHooksAsync()` resolves all registered hooks and executes them in order of their configured priority.

## Predefined Hooks

The MORYX framework already provides ready-to-use hooks:

### ModelLifecycleHook (`Moryx.Model`)

The `ModelLifecycleHook` ensures that all databases required by your application are in the expected state before startup.

Typical use cases include:

- Deleting specific or all databases (e.g., for development resets)
- Ensuring databases are created and ready to use

This hook is particularly useful for local development environments, CI pipelines, and integration testing scenarios.

### OrdersLifecycleHook (`Moryx.Orders.Management`)

The `OrdersLifecycleHook` is a testing-oriented helper designed to ensure that operations are available immediately after application startup.

To make use of this hook, you must configure at least one operation via `OrdersLifecycleHookConfig`.

Each operation configuration requires:

- Order number
- Operation number
- Product identifier
- Product revision
- Quantity to produce

This allows test environments to start with meaningful data already in place.

## Custom Hooks

You can easily extend the system by implementing your own lifecycle hooks.

### Basic example

Create a class that implements `ILifecycleHook`:

```csharp
public class MyCustomHook : ILifecycleHook
{
    public int Priority => 100;

    public async Task RunAsync()
    {
        // Your custom startup logic here
    }
}
```

### Module-aware hooks

If your hook depends on the lifecycle of a specific MORYX module, you can use the `ModuleLifecycleHookBase` base class

This abstraction:

- Handles registration with the `ModuleManager`
- Allows you to target specific modules using their facade and specific states via `TargetStates`
- Provides lifecycle-aware execution points

For a concrete implementation example, refer to the [`OrdersLifecycleHook`](/src/Moryx.Orders.Management/Tools/OrdersLifecycleHook.cs) in the framework.

## Dependency Injection

Lifecycle hooks are instantiated via the global dependency injection container.

This means you can inject:

- ASP.NET Core services
- MORYX runtime services
- Custom services from your application

## Best Practices

To ensure your hooks remain maintainable and reusable, follow these recommendations:

### Opt-in behavior

Hooks should not execute any logic unless explicitly configured.

This ensures:

- Safe defaults
- Better reusability in shared libraries
- Easier control across environments

### Use centralized configuration

Leverage the MORYX `IConfigManager` to define and retrieve configuration.

This keeps your hook configuration consistent with the rest of the ecosystem.

### Environment-specific behavior

For environment-dependent logic (e.g., deleting databases only in development), you can map values from `IConfiguration` into MORYX configuration objects.

Supported configuration sources include:

- `appsettings.json` and environment-specific variants
- Environment variables
- Secret managers
- Command line arguments

This mapping is opt-in and allows you to seamlessly integrate modern configuration practices into your hooks.

For more details, refer to the configuration documentation:

[/docs/articles/framework/configuration.md](/docs/articles/framework/configuration.md#mapping-iconfiguration-keys-to-moryx-configurations)
