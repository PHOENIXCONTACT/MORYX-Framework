# ADR-005: Date and Time Handling

**Date:** 2026-04-28  
**Status:** Accepted  
**Context:** MORYX 10+ Projects

## Decision

We will use UTC `DateTime` values or UTC-based relative times in the form of `DateTimeOffset` consistently throughout all MORYX modules.  
User interfaces (UIs) and adapters that communicate with external software systems are responsible for converting UTC values to the expected local time or culture if necessary.

For new projects, we will prefer using a [TimeProvider](https://learn.microsoft.com/en-us/dotnet/standard/datetime/timeprovider-overview) instead of static methods and properties such as `DateTime.UtcNow` to improve testability.

## Motivation

1. **Consistency**
   - Mixing UTC timestamps and local timestamps creates unnecessary mental overhead.
   - Comparing UTC timestamps with local timestamps can lead to unexpected behavior and bugs.

2. **Performance**
   - Runtime features used to convert timestamps before serialization can be skipped if UTC is applied consistently.

3. **Testability** (applies only to the `TimeProvider` decision)
   - Using an injected `TimeProvider` instead of the static `DateTime.UtcNow` enables better control over the system under test, resulting in more reliable and less brittle tests.

## Exceptions

Non-UTC-based timestamps should only be used when interacting with systems or users that explicitly require them.  
When receiving timestamps from external systems, they must be converted to UTC `DateTime` or `DateTimeOffset` values before being passed to other MORYX components.

## Consequences

- Developers must default to UTC-based time. `DateTime.Now`, `DateTime.Today`, and similar APIs should be avoided.
- When creating new projects, `TimeProvider` should be used to access the current time whenever possible.
- `DateTimeOffset` should be preferred over `DateTime` to enable smoother integration with `TimeProvider`.
- Teams benefit from more consistent and predictable behavior when working with dates and times.

## References

- [.NET Documentation – Overview: Dates, times, and time zones](https://learn.microsoft.com/en-us/dotnet/standard/datetime/)
- [.NET Documentation – Choosing between different time formats](https://learn.microsoft.com/en-us/dotnet/standard/datetime/choosing-between-datetime)
