// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Operators.Properties;

namespace Moryx.Operators.Exceptions;

/// <summary>
/// Thrown, if an operator with the gibven ID already exists.
/// </summary>
[Serializable]
public class AlreadyExistsException : Exception
{
    /// <summary>
    /// Defalt constructor
    /// </summary>
    public AlreadyExistsException()
    {
    }

    /// <summary>
    /// Creates an exception that includes the operators ID in the exception message
    /// </summary>
    /// <param name="operatorIdentifier">Operator's ID</param>
    public AlreadyExistsException(string operatorIdentifier) : base(string.Format(Strings.AlreadyExistsException_Message, operatorIdentifier))
    {
    }
}
