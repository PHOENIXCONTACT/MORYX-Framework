# ADR-006: ServiceProvider Bridge for Module Containers

**Date:** 2026-09-30
**Status:** Accepted
**Context:** MORYX 10+ Projects

MORYX uses a two-level DI system: the global `IServiceProvider` and per-module Castle Windsor containers. Services registered in the host `IServiceProvider` (e.g. `IHttpClientFactory`, `TimeProvider`) were not available inside module containers. Module code that needed such services had to rely on workarounds like static `HttpClient` instances.

## Decision

We will bridge the host `IServiceProvider` into Castle Windsor module containers using a custom `ISubDependencyResolver` ([`ServiceProviderSubResolver`](/src/Moryx.Container/ServiceProviderSubResolver.cs)). The resolver is registered last in the Castle Windsor kernel so that Windsor-registered components always take precedence.

Explicit registration in the module container (via `SetInstance`, `LoadComponents` or `RegistrationAttribute`) must always be preferred when the dependency is known to the module. The bridge should only be used for plugins or components that require infrastructure services from the global container which are not part of the module's own domain.

## Motivation

The alternative was to register each service explicitly in the module's container even though it is not part of the module's domain.
That approach requires a code change in the framework for every new service and pulls specific packages into core projects.

The sub-resolver only depends on `System.IServiceProvider`, so `Moryx.Container` does not need additional package references.
Any service that is registered in the `IServiceCollection` at startup is automatically available in all module containers — no framework changes required.

## Exceptions

- Facades are excluded to prevent them from being accidentally resolved through the bridge instead of through the facade mechanism.
- `IModuleManager` is excluded because modules must not access the module manager directly.
- Additional types may be excluded in future versions if they allow circumventing framework features or architectural boundaries. The bridge is not intended as a backdoor to bypass MORYX module isolation.
- Direct `Container.Resolve<T>()` calls are not affected by the bridge. The resolver only participates in constructor and property injection of registered components.
- The bridge does not guarantee that a service is available at runtime. The application developer is responsible for registering required services in the `IServiceCollection` at startup.

## Consequences

- Module plugins can inject host infrastructure services (e.g. `IHttpClientFactory`, `TimeProvider`) via constructor or property injection without additional registration.
- Castle Windsor registrations always win over the `IServiceProvider` for the same type.

## References

- [Dependency Injection documentation](/docs/articles/framework/dependency-injection.md#serviceprovider-bridge)
- [Castle Windsor ISubDependencyResolver](https://github.com/castleproject/Windsor/blob/master/docs/extension-points.md)
