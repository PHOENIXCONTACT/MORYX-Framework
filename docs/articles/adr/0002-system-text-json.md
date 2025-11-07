# ADR-002: Prefer `System.Text.Json` over `Newtonsoft.Json`

**Date:** 2025-11-07 \
**Status:** Accepted \
**Context:** MORYX 8+ Projects

We will use `System.Text.Json` as the default JSON serialization library in all new and existing MORYX projects wherever possible.
`Newtonsoft.Json` (Json.NET) will be used only in exceptional cases where specific functionality is required that `System.Text.Json` does not (yet) provide, such as advanced features like `TypeNameHandling.Full` or custom converters that cannot be easily replicated.

All new development should prefer `System.Text.Json`, and existing usages of `Newtonsoft.Json` should be gradually refactored to `System.Text.Json` where feasible.

## Motivation

1. **Performance and Efficiency**
    - `System.Text.Json` is significantly faster and more memory-efficient in most common serialization scenarios.
    - It is optimized for .NET’s built-in types and integrates tightly with the runtime (e.g. source generation, span-based parsing).

2. **Native Integration**
    - It’s the **default JSON library in .NET** since .NET Core 3.0 and used by many core frameworks such as ASP.NET Core and minimal APIs.
    - Ensures consistency and reduces dependency management issues (no need for third-party packages).

3. **Security**
    - `System.Text.Json` avoids some risky features (e.g., `TypeNameHandling`) that have historically led to **security vulnerabilities** in Newtonsoft.Json when misused.
    - Smaller surface area reduces the risk of deserialization attacks.

4. **Maintenance and Future Proofing**
    - `System.Text.Json` is part of the .NET runtime and maintained directly by Microsoft, ensuring continued updates, performance improvements, and long-term support.

5. **Reduced Dependencies**
    - Removing the dependency on `Newtonsoft.Json` simplifies project dependencies and package management, especially for smaller deployments or serverless scenarios.

## Exceptions

`Newtonsoft.Json` should only be used when a specific feature is required that `System.Text.Json` does not support or would require disproportionate effort to reimplement. Examples include:

- Advanced polymorphic deserialization using `TypeNameHandling.Full`
- Custom contract resolvers and converters with complex reflection-based logic
- Features like `PreserveReferencesHandling`, `IContractResolver`, or `ReferenceLoopHandling`
- Handling of very dynamic or loosely typed data (e.g., JSON-LD or unstructured APIs)
- Backward compatibility in existing codebases that heavily rely on Newtonsoft-specific attributes or behaviors

## Consequences

- Developers must default to `System.Text.Json` for all new code.
- Usage of `Newtonsoft.Json` requires explicit justification and documentation within the affected code or pull request.
- Over time, projects should aim to phase out `Newtonsoft.Json` where it is not strictly necessary.
- Teams gain better performance, security, and maintainability at the cost of temporarily reduced flexibility.

## References

- [.NET Documentation – System.Text.Json Overview](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-overview)
- [Community Discussion – System.Text.Json vs Newtonsoft.Json (Stack Overflow)](https://stackoverflow.com/questions/58138793/system-text-json-vs-newtonsoft-json)
- [Microsoft Developer Blog – Improving System.Text.Json Performance](https://devblogs.microsoft.com/dotnet/system-text-json-in-dotnet-8/)
