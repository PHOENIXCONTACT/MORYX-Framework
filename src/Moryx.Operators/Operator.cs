// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Operators;

/// <summary>
/// Operator Data class with the operator information
/// </summary>
/// <remarks>
/// Creates a new instance of an operator
/// </remarks>
/// <param name="identifier">The identifier of the operator</param>
public class Operator(string identifier)
{
    /// <summary>
    /// Unique identifier of the operator (e.g. a card number or personal number
    /// </summary>
    public virtual string Identifier { get; set; } = identifier;

    /// <summary>
    /// First name of the operator for display purposes
    /// </summary>
    public virtual string? FirstName { get; set; }

    /// <summary>
    /// Last name of the opoerator for display purposes
    /// </summary>
    public virtual string? LastName { get; set; }

    /// <summary>
    /// Pseudonym of the opoerator for display purposes
    /// </summary>
    public virtual string? Pseudonym { get; set; }
}