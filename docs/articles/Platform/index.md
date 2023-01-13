---
uid: PlatformMain
---
# Platform

The PlatformToolkit is a stack of components and code shared over several platforms. These components can be used at the server side, client side or webserver. The PlatformToolkit provides several features.

- DI Container Structure
- Logging
- Attributes and Base Classes
- WcfFactories
- Configuration Management

## Dependency Injection

The dependency injection in MORYX is basically depending on a Castle Widsor Container and a MORYX specific decorator called CastelContainer.
The CastelContainer will be used for the Component Composition as the DI Container for each level. Please open the following subpage to read more about the dependency injection in MORYX.

- [DependencyInjection](xref:DependencyInjection)

## Configuration

- [DelayQueue](xref:DelayQueue)

## Logging

Throughout the entire application every class created by a DI-container has access to a logger.
The loggers are structured hierarchical. Structures can be instantiated manually or automatically.
See @subpage platform-logging for full documentation

## Collections

- [Collections](xref:Collections)

## Bindings

MORYX has its own binding engine that uses binding strings like `"Object.Foo"` to resolve the value of `Foo`
for a given source object. Such strings might be embedded into texts using brackets `"Hello {Person.Name}, how are you?"`.

For more details see @subpage bindings.

## BinaryConnection

- [BinaryConnection](xref:BinaryConnection)

## Serialization

- [Serialization](xref:Serialization)