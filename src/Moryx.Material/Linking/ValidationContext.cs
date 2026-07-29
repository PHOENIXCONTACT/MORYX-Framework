// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Linking;

/// <summary>
/// Append-only context shared between hooks during a linking operation.
/// </summary>
/// <remarks>
/// Hooks contribute information, warnings, errors and requirements which are then
/// evaluated by the orchestrating manager and surfaced to the container for handling.
/// </remarks>
public class ValidationContext
{
    private readonly List<ValidationContextEntry> _entries = new();
    private readonly List<ILinkingRequirement> _requirements = new();
    private readonly object _lock = new();

    /// <summary>
    /// All recorded entries (info / warning / error).
    /// </summary>
    public IReadOnlyList<ValidationContextEntry> Entries
    {
        get
        {
            lock (_lock)
            {
                return _entries.ToArray();
            }
        }
    }

    /// <summary>
    /// All requirements raised by hooks.
    /// </summary>
    public IReadOnlyList<ILinkingRequirement> Requirements
    {
        get
        {
            lock (_lock)
            {
                return _requirements.ToArray();
            }
        }
    }

    /// <summary>
    /// True if any error entry was added.
    /// </summary>
    public bool HasErrors
    {
        get
        {
            lock (_lock)
            {
                return _entries.Any(e => e.Severity == ValidationSeverity.Error);
            }
        }
    }

    /// <summary>
    /// Adds an informational entry.
    /// </summary>
    /// <param name="text">Human-readable entry text.</param>
    /// <param name="hookType">Originating hook type, if known.</param>
    public void AddInfo(string text, Type? hookType = null) => Add(ValidationSeverity.Info, text, hookType);

    /// <summary>
    /// Adds a warning entry.
    /// </summary>
    /// <param name="text">Human-readable entry text.</param>
    /// <param name="hookType">Originating hook type, if known.</param>
    public void AddWarning(string text, Type? hookType = null) => Add(ValidationSeverity.Warning, text, hookType);

    /// <summary>
    /// Adds an error entry. Errors block the linking operation.
    /// </summary>
    /// <param name="text">Human-readable entry text.</param>
    /// <param name="hookType">Originating hook type, if known.</param>
    public void AddError(string text, Type? hookType = null) => Add(ValidationSeverity.Error, text, hookType);

    /// <summary>
    /// Adds an error entry built from an exception.
    /// </summary>
    /// <param name="exception">Exception whose message is added as an error entry.</param>
    /// <param name="hookType">Originating hook type, if known.</param>
    public void AddError(Exception exception, Type? hookType = null) =>
        Add(ValidationSeverity.Error, exception.Message, hookType);

    /// <summary>
    /// Appends a requirement that must be fulfilled by the container or operator.
    /// </summary>
    /// <param name="requirement">Requirement to append to the context.</param>
    public void AddRequirement(ILinkingRequirement requirement)
    {
        lock (_lock)
        {
            _requirements.Add(requirement);
        }
    }

    private void Add(ValidationSeverity severity, string text, Type? hookType)
    {
        lock (_lock)
        {
            _entries.Add(new ValidationContextEntry(severity, text, hookType, DateTimeOffset.UtcNow));
        }
    }
}
