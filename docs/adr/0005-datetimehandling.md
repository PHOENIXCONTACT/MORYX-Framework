# ADR-005: Date and Time Handling

**Date:** 2026-04-28 \
**Status:** Accepted \
**Context:** MORYX 10+ Projects

We will use UTC DateTime values or UTC relative times in form of DateTimeOffset values consistently throughout
all MORYX Modules. UIs and Adapters that talk to other softwaresystems are responsible for converting the UTC values to the expected locale if necessary.

For new Projects we will prefer using a [TimeProvider](https://learn.microsoft.com/en-us/dotnet/standard/datetime/timeprovider-overview) instead static methods and properties like DateTime.UtcNow for increased testability.

## Motivation

1. **Consistency**
    - Mixing UTC timestamps and normal timestamps creates unnecessary mental overhead
    - Comparing UTC Timestamps and locale Timestamps can lead to unexpected behavior and bugs.
2. **Performance**
    - We have runtime features to convert timestamps before serializing them that can be skipped if UTC was applied consistently
3. **Testablility** (Only for the TimeProvider part)
    - Using an injected TimeProvider instead of the static DateTime.UtcNow allows better control over the system under test, leading to less brittle tests

## Exceptions

Non UTC based timestamps should only be used when interacting with systems or users that require that.
When receiving timestamps from outside systems they should be converted to UTC DateTimes or DateTimeOffsets before passing it to other MORYX Components.

## Consequences

- Developers must default to UTC based time. DateTime.Now, DateTime.Today etc. and other functions should be avoided.
- When creating new projects TimeProvider should be used to access time if possible and DateTimeOffset should be prefered over DateTime to smoothly operate with the TimeProvider.
- Teams gain a more consistent and predictable behavior when working with date and time.

## References
- [.NET Documentation - Overview: Dates, times and time zones](https://learn.microsoft.com/en-us/dotnet/standard/datetime/)
- [.NET Documentation - Choosing between different Time formats](https://learn.microsoft.com/en-us/dotnet/standard/datetime/choosing-between-datetime)
