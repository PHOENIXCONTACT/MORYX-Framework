# Add Startup Hooks to your application

Startup hooks are a standardized and extensible mechanism to execute custom logic before the MORYX modules of your application are launched.
They are designed to enable reusable, composable startup tasks that help ensure your application starts in a predictable and controlled state.

Originally, startup hooks were introduced to simplify manual testing scenarios by preparing the application with predefined data or state.
However, their usage extends far beyond testing — they are equally useful for setting up infrastructure, validating external dependencies, or initializing required resources.

## Definition

Startup hooks are lightweight components that implement the `IStartupHook` interface.

Each hook defines:

- **Priority** – Determines execution order. Hooks with lower priority values are typically executed first.
- **RunAsync** – The asynchronous method where the hook’s logic is executed.

This simple contract allows hooks to remain focused, easy to implement, and highly reusable.

## Modify the `Program.cs`

To enable startup hooks in your application, you need to:

1. Register all hook implementations in the DI container
2. Execute them before starting the MORYX modules

The `Moryx.Runtime.Kernel` package provides convenient extension methods to simplify this setup.

```csharp
var builder = WebApplication.CreateBuilder();

// Register application services
builder.Services.AddStartupHooks();

var app = builder.Build();

// Configure middleware, endpoints, etc.

// Execute all registered startup hooks
await app.Services.RunHooks();

app.Run();
```

### What happens under the hood?

- `AddStartupHooks()` scans for all implementations of `IStartupHook` and registers them in the service collection.
- `RunHooks()` resolves all registered hooks and executes them in order of their configured priority.

## Predefined Hooks

The MORYX framework already provides ready-to-use hooks in the `Moryx.Startup.Hooks` project.

### DatabaseHook

The `DatabaseHook` ensures that all databases required by your application are in the expected state before startup.

Typical use cases include:

- Deleting specific or all databases (e.g., for development resets)
- Ensuring databases are created and ready to use

This hook is particularly useful for local development environments, CI pipelines, and integration testing scenarios.

### OrdersHook

The `OrdersHook` is a testing-oriented helper designed to ensure that operations are available immediately after application startup.

To make use of this hook, you must configure at least one operation via `OrdersHookConfig`.

Each operation configuration requires:

- Order number
- Operation number
- Product identifier
- Product revision
- Quantity to produce

This allows test environments to start with meaningful data already in place.

## Custom Hooks

You can easily extend the system by implementing your own startup hooks.

### Basic example

Create a class that implements `IStartupHook`:

```csharp
public class MyCustomHook : IStartupHook
{
    public int Priority => 100;

    public async Task RunAsync()
    {
        // Your custom startup logic here
    }
}
```


### Module-aware hooks

If your hook depends on the lifecycle of a specific MORYX module, you can use the following base classes:

- `ModuleHook`
- `ModuleStartHook`

These abstractions:

- Handle registration with the `ModuleManager`
- Allow you to target specific modules using their facade
- Provide lifecycle-aware execution points

For a concrete implementation example, refer to the `OrdersHook` in the framework.

## Dependency Injection

Startup hooks are instantiated via the global dependency injection container.

This means you can inject:

- ASP.NET Core services
- MORYX runtime services
- Custom services from your application

This enables powerful integration scenarios with minimal boilerplate.

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


## Summary

Startup hooks provide a clean and extensible way to prepare your application before it starts:

- Execute pre-start logic in a structured way
- Keep setup code modular and reusable
- Integrate with DI and configuration systems
- Control execution order via priorities

By leveraging startup hooks, you can improve reliability, simplify testing, and ensure your application always starts in the expected state.
