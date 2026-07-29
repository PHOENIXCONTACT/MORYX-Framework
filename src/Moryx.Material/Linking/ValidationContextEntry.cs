// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Linking;

/// <summary>
/// Single entry contributed to a <see cref="ValidationContext"/>.
/// </summary>
public class ValidationContextEntry
{
    /// <summary>Severity of the entry.</summary>
    public ValidationSeverity Severity { get; }

    /// <summary>Human-readable text.</summary>
    public string Text { get; }

    /// <summary>Originating hook type, if known.</summary>
    public Type? HookType { get; }

    /// <summary>UTC timestamp when the entry was recorded.</summary>
    public DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationContextEntry"/> class.
    /// </summary>
    /// <param name="severity">Severity of the entry.</param>
    /// <param name="text">Human-readable entry text.</param>
    /// <param name="hookType">Originating hook type, if known.</param>
    /// <param name="timestamp">UTC timestamp when the entry was recorded.</param>
    public ValidationContextEntry(ValidationSeverity severity, string text, Type? hookType, DateTimeOffset timestamp)
    {
        Severity = severity;
        Text = text;
        HookType = hookType;
        Timestamp = timestamp;
    }
}

/// <summary>
/// Severity classification of a <see cref="ValidationContextEntry"/>.
/// </summary>
public enum ValidationSeverity
{
    /// <summary>Informational only.</summary>
    Info = 0,

    /// <summary>Non-blocking concern.</summary>
    Warning = 1,

    /// <summary>Blocks the operation.</summary>
    Error = 2
}