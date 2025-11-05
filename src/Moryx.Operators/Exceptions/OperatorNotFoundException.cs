// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Operators.Localizations;

namespace Moryx.Operators.Exceptions;

/// <summary>
/// Thrown if the given operator isn't found.
/// </summary>
[Serializable]
public class OperatorNotFoundException : Exception
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public OperatorNotFoundException()
    {
    }

    /// <summary>
    /// Creates an exception that includes the operators ID in the exception message
    /// </summary>
    /// <param name="operatorIdentifier">Operator's ID</param>
    public OperatorNotFoundException(string operatorIdentifier) : base(string.Format(Strings.OperatorNotFoundException_Message, operatorIdentifier))
    {
    }
}
