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
- State Machine Pattern

## Dependency Injection

The dependency injection in MORYX is basically depending on a Castle Windsor Container and a MORYX specific decorator called CastleContainer.
The CastleContainer will be used for the Component Composition as the DI Container for each level. 

## Configuration

- [Configuration](configuration.md)

## Logging

Throughout the entire application every class created by a DI-container has access to a logger.  
The loggers are structured hierarchical. Structures can be instantiated manually or automatically.

## Collections

- [Collections](Collections)
- [DelayQueue](Collections/delay-queue.md)

## Bindings

The MORYX platform has its own binding engine that uses binding strings like `"Object.Foo"` to resolve the value of `Foo`
for a given source object. Such strings might be embedded into texts using brackets `"Hello {Person.Name}, how are you?"`.

For more details see @subpage bindings.

## BinaryConnection

- [BinaryConnection](binary-connection.md)

## Serialization

- [Serialization](Serialization)

## State Machine

The MORYX Framework provides a state machine implementation that can be used to add state machine capabilities to resources, drivers or modules.

- [State Machine](state-machine.md)